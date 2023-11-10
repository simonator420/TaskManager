using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zipper_App___Reawote
{
    public partial class CheckboxForm : Form
    {
        Form1 f = new Form1();

        public CheckboxForm()
        {
            InitializeComponent();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CheckboxForm_Load(object sender, EventArgs e)
        {

        }

        private void prevedDoJPG_Click(object sender, EventArgs e)
        {
            List<string> checkedPaths = new List<string>();
            List<string> selectedFolderPaths = f.getSelectedFolderPaths();


            Console.WriteLine("Tohle je pocet " + checkedListBox1.Items.Count);

            foreach (var item in selectedFolderPaths)
            {
                Console.WriteLine("Tohle je item z checkedPaths: " + item);
            }

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                Console.WriteLine(" ");
                if (checkedListBox1.GetItemChecked(i))
                {
                    string item = checkedListBox1.Items[i].ToString();
                    Console.WriteLine("Tohle je item " + item);
                    Console.WriteLine("Tohle jsou selectedFolderPaths " + selectedFolderPaths);

                    // You need to find the full path based on the item's name
                    foreach (var folder in selectedFolderPaths)
                    {
                        Console.WriteLine("Tohle je folder " + folder);
                        string fullPath = Path.Combine(folder, item);
                        Console.WriteLine("Tohle je fullPath " + fullPath);
                        if (File.Exists(fullPath) || Directory.Exists(fullPath))
                        {
                            checkedPaths.Add(fullPath);
                            break; // Exit the loop when a match is found
                        }
                    }
                }
            }

            if (checkedPaths == null || checkedPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné checkboxy.", "Chyba");
            }

            else
            {
                foreach (var folderPath in checkedPaths)
                {
                    foreach (var subfolder in Directory.GetDirectories(folderPath))
                    {
                        string folderName = Path.GetFileName(subfolder);
                        if (folderName.Contains("1K") || folderName.Contains("2K") ||
                            folderName.Contains("3K") || folderName.Contains("4K") ||
                            folderName.Contains("5K") || folderName.Contains("6K") || folderName.Contains("7K") || folderName.Contains("8K") || folderName.Contains("9K") || folderName.Contains("10K") || folderName.Contains("11K") || folderName.Contains("12K") || folderName.Contains("13K") || folderName.Contains("14K") || folderName.Contains("15K") || folderName.Contains("16K")) ;
                        {
                            string[] pngFiles = Directory.GetFiles(subfolder, "*.png");
                            foreach (var pngFile in pngFiles)
                            {
                                if (Path.GetFileName(pngFile).Contains("_DIFF") || Path.GetFileName(pngFile).Contains("_COL"))
                                {
                                    string jpgFile = Path.ChangeExtension(pngFile, ".jpg");
                                    using (Image image = Image.FromFile(pngFile))
                                    {
                                        var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                        var parameters = new EncoderParameters(1);
                                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 24L);

                                        image.Save(jpgFile, encoder, parameters);
                                    }
                                    File.Delete(pngFile);
                                }
                                else if (Path.GetFileName(pngFile).Contains("_NRM16"))
                                {
                                    // Convert the PNG file to JPG with 8-bit depth and rename
                                    //string jpgFile = Path.Combine(subfolder, pngFile.Replace("_NRM16", "_NRM"));
                                    string jpgFile = Path.ChangeExtension(Path.Combine(subfolder, pngFile.Replace("_NRM16", "_NRM")), ".jpg");
                                    using (Image image = Image.FromFile(pngFile, true))
                                    {

                                        var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                                        var parameters = new EncoderParameters(1);
                                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8);

                                        image.Save(jpgFile, encoder, parameters);
                                    }
                                }
                            }
                        }
                    }

                }
                MessageBox.Show("Soubory byly úspěšně převedeny.", "Hotovo");
            }
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            bool areAllChecked = true;

            // Check the current state of checkboxes and toggle it
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (!checkedListBox1.GetItemChecked(i))
                {
                    areAllChecked = false;
                    break;
                }
            }

            // Toggle the state of checkboxes based on the current state
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, !areAllChecked);
            }
        }
    }
}
