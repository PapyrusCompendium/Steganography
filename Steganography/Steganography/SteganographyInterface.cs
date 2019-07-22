using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steganography
{
    public partial class SteganographyInterface : Form
    {
        private Image InputImage { get; set; }
        private Image OutputImage { get; set; }

        public SteganographyInterface()
        {
            InitializeComponent();
        }

        public void CalculateImageCapacity() => imageCapacity.Text = $"Max Capacity: {SteganographyManager.TotalCapacity(inputImage.Image) - 4} bytes";
        private void SecretMessageText_TextChanged(object sender, EventArgs e) => secretMessageSize.Text = $"Data Size: {Encoding.UTF8.GetBytes(secretMessageText.Text).Length} bytes";
        private void SteganographyInterface_Shown(object sender, EventArgs e) => secretMessageSize.Text = $"Data Size: {Encoding.UTF8.GetBytes(secretMessageText.Text).Length} bytes";

        private void ImageSelectionLink_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                InputImage = Image.FromFile(openFileDialog.FileName);

            inputImage.Image = InputImage;
            CalculateImageCapacity();
        }

        private void ConvertImageButton_Click(object sender, EventArgs e)
        {
            if (encodeRadio.Checked)
            {
                if (SteganographyManager.FitsIntoImage(Encoding.UTF8.GetBytes(secretMessageText.Text), inputImage.Image))
                    outputImage.Image = SteganographyManager.EncodeImage(InputImage, Encoding.UTF8.GetBytes(secretMessageText.Text));
            }
            else if (decodeRadio.Checked)
            {
                MessageBox.Show(Encoding.UTF8.GetString(SteganographyManager.DecodeImage(outputImage.Image)));
            }
        }
    }
}