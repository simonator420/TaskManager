using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zipper_App___Reawote
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // načtení okna
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // metoda pro výbrání složky ke komprimaci
        private string selectedFolderPath;
        private void button1_Click(object sender, EventArgs e)
        {
            // po stisku tlačítka se otevře průzkumník souborů
            using (var dialog = new FolderBrowserDialog())
            {
                // pokud uživatel stiskne OK
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // tak se do proměnné selectedFolderPath uloží vybraná cesta
                    selectedFolderPath = dialog.SelectedPath;
                    // v textboxu se zobrazí vybraná cesta
                    textBox1.Text = selectedFolderPath;
                }
            }
        }
        
        // useless metoda, která se nedá vymazat
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }


        // metoda pro vymazání složek ze zipu
        public static void DeleteFoldersFromZip(string zipFilePath, List<string> folderNames)
        {
            // otevře zip
            using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
            {
                // najde složky, které obsahují konkrétní znaky
                var entriesToDelete = archive.Entries.Where(e => folderNames.Any(f => e.FullName.StartsWith(f)));

                // vymaže ty složky, které hledaný znak obsahují
                foreach (var entry in entriesToDelete.ToList())
                {
                    entry.Delete();
                }
            }
        }

        public static void DeleteFilesFromFolder(string folderPath, string[] fileExtensions)
        {
            // získání listu s námí definovanými koncovkami
            var filesToDelete = Directory.GetFiles(folderPath)
                                         .Where(f => fileExtensions.Contains(Path.GetExtension(f),
                                                                StringComparer.OrdinalIgnoreCase));

            // vymaže jednotlivé soubory ze souboru
            foreach (var file in filesToDelete)
            {
                File.Delete(file);
            }
        }

        // metoda pro zazipování složky
        private void ZipFolder()
        {
            // zazipovaná složka se bude jmenovat stejně jako původní složka, jen se k němu přidá koncovka
            string zipFileName16 = Path.GetFileName(selectedFolderPath) + "_16K.zip";
            // zkombinování cesty vybrané složky a zipu složky, o který se snažíme
            string zipFilePath16 = Path.Combine(Path.GetDirectoryName(selectedFolderPath), zipFileName16);
            // vytvoření zipu - ze vybran složky se vytvoří zazipovaná složka
            ZipFile.CreateFromDirectory(selectedFolderPath, zipFilePath16);
            var foldersToDelete16 = new List<string> { "2K", "3K", "4K", "6K", "8K" };
            DeleteFoldersFromZip(zipFilePath16, foldersToDelete16);

            // zbytek pro ostatní rozlišení
            string zipFileName8 = Path.GetFileName(selectedFolderPath) + "_8K.zip";
            string zipFilePath8 = Path.Combine(Path.GetDirectoryName(selectedFolderPath), zipFileName8);
            ZipFile.CreateFromDirectory(selectedFolderPath, zipFilePath8);
            var foldersToDelete8 = new List<string> { "2K", "3K", "4K", "6K", "16K" };
            DeleteFoldersFromZip(zipFilePath8, foldersToDelete8);

            string zipFileName6 = Path.GetFileName(selectedFolderPath) + "_6K.zip";
            string zipFilePath6 = Path.Combine(Path.GetDirectoryName(selectedFolderPath), zipFileName6);
            ZipFile.CreateFromDirectory(selectedFolderPath, zipFilePath6);
            var foldersToDelete6 = new List<string> { "2K", "3K", "4K", "8K", "16K" };
            DeleteFoldersFromZip(zipFilePath6, foldersToDelete6);

            string zipFileName4 = Path.GetFileName(selectedFolderPath) + "_4K.zip";
            string zipFilePath4 = Path.Combine(Path.GetDirectoryName(selectedFolderPath), zipFileName4);
            ZipFile.CreateFromDirectory(selectedFolderPath, zipFilePath4);
            var foldersToDelete4 = new List<string> { "2K", "3K", "6K", "8K", "16K" };
            DeleteFoldersFromZip(zipFilePath4, foldersToDelete4);

            string zipFileName3 = Path.GetFileName(selectedFolderPath) + "_3K.zip";
            string zipFilePath3 = Path.Combine(Path.GetDirectoryName(selectedFolderPath), zipFileName3);
            ZipFile.CreateFromDirectory(selectedFolderPath, zipFilePath3);
            var foldersToDelete3 = new List<string> { "2K", "4K", "6K", "8K", "16K" };
            DeleteFoldersFromZip(zipFilePath3, foldersToDelete3);

            string zipFileName2 = Path.GetFileName(selectedFolderPath) + "_2K.zip";
            string zipFilePath2 = Path.Combine(Path.GetDirectoryName(selectedFolderPath), zipFileName2);
            ZipFile.CreateFromDirectory(selectedFolderPath, zipFilePath2);
            var foldersToDelete2 = new List<string> {"3K", "4K", "6K", "8K", "16K" };
            DeleteFoldersFromZip(zipFilePath2, foldersToDelete2);

            // vymazání souborů s danými koncovkami ze složky
            var fileExtensions = new string[] { ".txt", ".rtf"};
            DeleteFilesFromFolder(selectedFolderPath, fileExtensions);
            // vymazání složek z původní složky
            string[] foldersToDeleteSource = { "2K", "3K", "4K", "6K", "8K", "16K", "BACKPLATE" };
            // získání informací o selectedFolderPatg a uložení do proměnní DirectoryInfo
            DirectoryInfo directory = new DirectoryInfo(selectedFolderPath);
            // definování for cyklu
            foreach (string folderName in foldersToDeleteSource)
            {
                // projde všechny jména složek ve složce
                foreach (DirectoryInfo subDirectory in directory.GetDirectories(folderName))
                {
                    // vymaže vybraný složky
                    subDirectory.Delete(true);
                }
            }
        }

        // metoda pro provedení zazipování
        private void button2_Click(object sender, EventArgs e)
        {
            // pokud není v proměnné selectedFolderPath nic (nic jsme nevybrali)
            if (selectedFolderPath == null)
            {
                // tak se zobrazí zpráva
                MessageBox.Show("Vyber složku pro zazipování", "Na něco jsi zapomněl");
                return;
            }

            // samotné zavolání metody pro zazipování
            ZipFolder();
            // zpráva, která oslavuje náš úspěch
            MessageBox.Show("Úspěšně zazipováno", "Hotovo");
        }

        // metoda pro ukončení aplikace
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chceš odejít?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
