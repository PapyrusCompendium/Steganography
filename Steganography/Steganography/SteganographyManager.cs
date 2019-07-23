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
        public static bool FitsIntoImage(byte[] data, Bitmap steganograph)
        {
            return data.Length < TotalCapacity(steganograph);
        }

        public static int TotalCapacity(Bitmap steganograph)
        {
            // 1 Pixel can store .5 bytes of data
            // 2 pixels can store 1 byte of data
            // Length of the data is stored in 8 pixels (4 bytes Int32)
            return ((steganograph.Width * steganograph.Height) - 8) / 2;
        }

        public static byte SetLSB(bool bit, byte value)
        {
            if (GetLSB(value) && !bit)
                return (byte)(value ^ 1); //Will flip the LSB to 0 if it is 1
            else if (!GetLSB(value) && bit)
                return (byte)(value | 1); //Will flip the LSB to 1 if it is 0
            return value;
        }

        public static bool GetLSB(byte value)
        {
            return (value & 1) != 0;
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static byte[] GetColourBytes(Color colour)
        {
            return new byte[4] { colour.A, colour.R, colour.G, colour.B };
        }

        public static byte[] DecodeImage(Bitmap image)
        {
            bool[] bits = new bool[32];

            //We only want to read 8 pixels to decode the first 4 bytes, thats contains the length of the total data.
            for (int x = 0; x < Math.Ceiling(8d / image.Width); x++)
                for (int y = 0; y < (image.Height > 8 ? 8 : image.Height); y++)
                {
                    byte[] colourBytes = GetColourBytes(image.GetPixel(x, y));

                    for (int i = 0; i < 4; i++)
                        bits[(((x * image.Width) + y) * 4) + i] = GetLSB(colourBytes[i]);
                }

            byte[] lengthBytes = BitArrayToByteArray(new BitArray(bits));
            int length = BitConverter.ToInt32(lengthBytes, 0);
            return DecodeImageLSB(image, length);
        }

        private static byte[] DecodeImageLSB(Bitmap image, int length)
        {
            int totalPixelsEncoded = length * 2;
            bool[] bits = new bool[length * 8];

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    if ((x * image.Width) + y < 8)
                        continue;

                    if ((x * image.Width) + y - 8 >= totalPixelsEncoded)
                        return BitArrayToByteArray(new BitArray(bits.ToArray()));

                    byte[] colourBytes = GetColourBytes(image.GetPixel(x, y));

                    for (int i = 0; i < 4; i++)
                        bits[(((x * image.Width) + y) * 4) + i - 32] = GetLSB(colourBytes[i]);
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

            BitArray bits = new BitArray(data);

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    if ((((x * image.Width) + y) * 4) >= bits.Length)
                        return bitMap;

                    byte[] colourBytes = GetColourBytes(bitMap.GetPixel(x, y));
                    for (int i = 0; i < 4; i++)
                        colourBytes[i] = SetLSB(bits[(((x * image.Width) + y) * 4) + i], colourBytes[i]);

                    bitMap.SetPixel(x, y, Color.FromArgb(colourBytes[0], colourBytes[1], colourBytes[2], colourBytes[3]));
                }

            return bitMap;
        }
    }
}