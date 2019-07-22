using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    public static class SteganographyManager
    {
        public static bool FitsIntoImage(byte[] data, Image steganograph)
        {
            return data.Length + 1 < TotalCapacity(steganograph);
        }

        public static int TotalCapacity(Image steganograph)
        {
            //Take the aera (Total number of pixels) * by 3 for each byte in a pixel (RGB)
            //Divide by 8 to get total capacity (1 bit per byte LSB)
            return ((steganograph.Height * steganograph.Width) * 3) / 8;
        }

        public static byte SetLSB(bool bit, byte colour)
        {
            if (bit)
                colour = (byte)(colour | (1 << 0));
            else
                colour = (byte)(colour & ~(1 << 0));

            return colour;
        }

        public static bool GetLSB(byte colour)
        {
            return (colour & 1) != 0;
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static byte[] DecodeImage(Image image)
        {
            Bitmap bitMap = new Bitmap(image);
            bool[] bits = new bool[32];

            //We only want to read 8 pixels to decode the first 4 bytes, thats contains the length of the total data.
            for (int x = 0; x < Math.Ceiling(8d / image.Width); x++)
                for (int y = 0; y < (image.Height > 8 ? 8 : image.Height); y++)
                {
                    Color colour = bitMap.GetPixel(x, y);

                    byte[] colourBytes = new byte[4] { colour.A, colour.R, colour.G, colour.B };
                    for (int i = 0; i < 4; i++)
                        bits[x + y] = GetLSB(colourBytes[i]);
                }

            byte[] lengthBytes = BitArrayToByteArray(new BitArray(bits.ToArray()));
            int length = BitConverter.ToInt32(lengthBytes, 0);
            return DecodeImageLSB(image, length);
        }

        private static byte[] DecodeImageLSB(Image image, int length)
        {
            int totalPixelsEncoded = ((length * 8) / 4);
            bool[] bits = new bool[length * 8];

            Bitmap bitMap = new Bitmap(image);

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    if (x + y < 32)
                        continue;

                    if(x + y > totalPixelsEncoded)
                        return BitArrayToByteArray(new BitArray(bits.ToArray()));

                    Color colour = bitMap.GetPixel(x, y);
                    byte[] colourBytes = new byte[4] { colour.A, colour.R, colour.G, colour.B };

                    for (int i = 0; i < 4; i++)
                        bits[x + y] = (GetLSB(colourBytes[i]));
                }
            return BitArrayToByteArray(new BitArray(bits.ToArray()));
        }

        public static Bitmap EncodeImage(Image image, byte[] data)
        {
            byte[] formattedData = new byte[data.Length + 4];
            BitConverter.GetBytes((Int32)data.Length).CopyTo(formattedData, 0);
            data.CopyTo(formattedData, 4);

            return EncodeImageLSB(image, formattedData);
        }

        private static Bitmap EncodeImageLSB(Image image, byte[] data)
        {
            Bitmap bitMap = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(bitMap))
                graphics.DrawImage(image, 0, 0);

            BitArray dataBits = new BitArray(data);

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    if (x + y >= dataBits.Length)
                        return bitMap;

                    Color colour = bitMap.GetPixel(x, y);

                    byte[] colourBytes = new byte[4] { colour.A, colour.R, colour.G, colour.B};
                    for (int i = 0; i < 4; i++)
                            colourBytes[i] = SetLSB(dataBits[x + y], colourBytes[i]);

                    bitMap.SetPixel(x, y, Color.FromArgb(colourBytes[0], colourBytes[1], colourBytes[2], colourBytes[3]));
                }

            return bitMap;
        }
    }
}