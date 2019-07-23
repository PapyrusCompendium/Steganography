using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steganography
{
    public partial class SteganographyInterface : Form
    {
        private SteganographicImage Stegnograph { get; set; }

        public SteganographyInterface()
        {
            InitializeComponent();
        }

        public void CalculateImageCapacity() => imageCapacity.Text = $"Max Capacity: {Stegnograph.TotalCapacity()} bytes";
        private void SecretMessageText_TextChanged(object sender, EventArgs e) => secretMessageSize.Text = $"Data Size: {Encoding.UTF8.GetBytes(secretMessageText.Text).Length} bytes";
        private void SteganographyInterface_Shown(object sender, EventArgs e) => secretMessageSize.Text = $"Data Size: {Encoding.UTF8.GetBytes(secretMessageText.Text).Length} bytes";

        private void ImageSelectionLink_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                Stegnograph = new SteganographicImage(new Bitmap(Image.FromFile(openFileDialog.FileName)));

            inputImage.Image = Stegnograph.Stegnograph;
            CalculateImageCapacity();
        }

        private void SaveImageLink_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                Stegnograph.Stegnograph.Save(saveFileDialog.FileName, ImageFormat.Png);
        }

        private void ConvertImageButton_Click(object sender, EventArgs e)
        {
            if (Stegnograph == null)
            {
                MessageBox.Show("Load an image!");
                return;
            }

            if (encodeRadio.Checked)
                if (Stegnograph.FitsIntoImage(Encoding.UTF8.GetBytes(secretMessageText.Text)))
                    Task.Run(() =>
                    {
                        outputImage.Image = Stegnograph.EncodeImage(Encoding.UTF8.GetBytes(secretMessageText.Text));
                        MessageBox.Show("Encoded!");
                    });
                else
                    MessageBox.Show("Data too large!");

            if (decodeRadio.Checked)
                Task.Run(() =>
                {
                    string decoded = Encoding.UTF8.GetString(Stegnograph.DecodeImage());
                    secretMessageText.Invoke(new MethodInvoker(() => secretMessageText.Text = decoded));
                    MessageBox.Show(decoded);
                });
        }
    }
}