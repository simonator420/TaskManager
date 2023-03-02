using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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

        // vytvoření listu, do kterého se uloží všechny vybrané cesty
        private List<string> selectedFolderPaths = new List<string>();

        // metoda pro výbrání složek, které se uloží do listu
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    // přidá všechny cesta složek do listu
                    selectedFolderPaths.AddRange(dialog.FileNames);

                    // update ListBoxu s dalšíma cestama
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(selectedFolderPaths.ToArray());
                }
            }
        }

        // čudlík, pro vybrání cesty ke zkopírování složek a souborů bez složky SOURCE
        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
            }
            else
            {
                // vytvoření průzkumníka souborů
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker= true;
                    dialog.Multiselect = false;
                    // po stisknutí tlačítka pro potvrzení cesty
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        // se uloží vybraná cesta do proměnné destinationPath
                        string destinationPath = dialog.FileName;
                        // procházení všech složek uložených v listu
                        foreach (string folderPath in selectedFolderPaths)
                        {
                            // získání jména složky
                            string folderName = new DirectoryInfo(folderPath).Name;
                            // vytvoření cesty pro finální složku
                            string destinationFolderPath = Path.Combine(destinationPath, folderName);
                            // volání fuknce pro zkopírování složky
                            CopyDirectory(folderPath, destinationFolderPath);
                        }
                    }
                    MessageBox.Show("Operace dokončena!", "Hotovo");
                }
            }
        }

        // funkce pro zkopírování všech souborů a složek kromě SOURCE složky pomocí rekurze
        private static void CopyDirectory(string sourceDirPath, string destDirPath)
        {
            // vytvoření cílové složky
            Directory.CreateDirectory(destDirPath);
            // procházení všech souborů v původní složce
            foreach (string filePath in Directory.GetFiles(sourceDirPath))
            {
                // získání jména souboru
                string fileName = Path.GetFileName(filePath);
                // vytvoření cílové cesty pro soubor
                string destFilePath = Path.Combine(destDirPath, fileName);
                // zkopírování souboru z původní cesty
                File.Copy(filePath, destFilePath);
            }
            // procházení všech podsložek v původní složce
            foreach (string subDirPath in Directory.GetDirectories(sourceDirPath))
            {
                // hledání složky se jménem SOURCE
                if(new DirectoryInfo (subDirPath).Name == "SOURCE")
                {
                    // pokud nějakou najde, tak ji přeskočí a nebude se kopírovat
                    continue;
                }
                // získání jména složky
                string subDirName = new DirectoryInfo(subDirPath).Name;
                // vytvoření cílové cesty pro složky
                string destSubDirPath = Path.Combine(destDirPath, subDirName);
                // rekurzivni volání funkce sama sebe a opakování procesu pro všechny složky které uloženy v listu
                CopyDirectory (subDirPath, destSubDirPath);
            }
        }

        // funkce pro vymazaní všech itemů z listu
        private void CLEAR_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            selectedFolderPaths.Clear();
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

        // funkce pro vymazání souborů ze složky
        public static void DeleteFilesFromFolder(string folderPath, string[] fileExtensions)
        {
            // získání listu s námí definovanými koncovkami
            var filesToDelete = Directory.GetFiles(folderPath)
                                         .Where(f => fileExtensions.Contains(Path.GetExtension(f),
                                                                StringComparer.OrdinalIgnoreCase));

            // vymaže jednotlivé soubory ze souboru
            foreach (var file in filesToDelete)
            {
                System.IO.File.Delete(file);
            }
        }

        private void ZipFolder()
        {
            foreach (string selectedFolderPath in selectedFolderPaths)
            {
                // Vytvoření zip souboru se stejným názvem jako původní složky
                string parentFolderPath1 = Path.GetDirectoryName(selectedFolderPath);
                string fullZipFileName1 = Path.GetFileName(selectedFolderPath) + "_16K.zip";
                string fullZipFilePath1 = Path.Combine(parentFolderPath1, fullZipFileName1);
                // Vytvoří zip soubor ze zvolené složky
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePath1);
                // Vymaže ze zip souboru složky, které obsahují zadané znaky
                var foldersToDelete1 = new List<string> { "2K", "3K", "4K", "6K", "8K" };
                DeleteFoldersFromZip(fullZipFilePath1, foldersToDelete1);

                string parentFolderPath2 = Path.GetDirectoryName(selectedFolderPath);
                string fullZipFileName2 = Path.GetFileName(selectedFolderPath) + "_8K.zip";
                string fullZipFilePath2 = Path.Combine(parentFolderPath2, fullZipFileName2);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePath2);
                var foldersToDelete2 = new List<string> { "2K", "3K", "4K", "6K", "16K" };
                DeleteFoldersFromZip(fullZipFilePath2, foldersToDelete2);

                string parentFolderPath3 = Path.GetDirectoryName(selectedFolderPath);
                string fullZipFileName3 = Path.GetFileName(selectedFolderPath) + "_6K.zip";
                string fullZipFilePath3 = Path.Combine(parentFolderPath3, fullZipFileName3);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePath3);
                var foldersToDelete3 = new List<string> { "2K", "3K", "4K", "8K", "16K" };
                DeleteFoldersFromZip(fullZipFilePath3, foldersToDelete3);

                string parentFolderPath4 = Path.GetDirectoryName(selectedFolderPath);
                string fullZipFileName4 = Path.GetFileName(selectedFolderPath) + "_4K.zip";
                string fullZipFilePath4 = Path.Combine(parentFolderPath4, fullZipFileName4);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePath4);
                var foldersToDelete4 = new List<string> { "2K", "3K", "6K", "8K", "16K" };
                DeleteFoldersFromZip(fullZipFilePath4, foldersToDelete4);

                string parentFolderPath5 = Path.GetDirectoryName(selectedFolderPath);
                string fullZipFileName5 = Path.GetFileName(selectedFolderPath) + "_3K.zip";
                string fullZipFilePath5 = Path.Combine(parentFolderPath5, fullZipFileName5);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePath5);
                var foldersToDelete5 = new List<string> { "2K", "4K", "6K", "8K", "16K" };
                DeleteFoldersFromZip(fullZipFilePath5, foldersToDelete5);

                string parentFolderPath6 = Path.GetDirectoryName(selectedFolderPath);
                string fullZipFileName6 = Path.GetFileName(selectedFolderPath) + "_2K.zip";
                string fullZipFilePath6 = Path.Combine(parentFolderPath6, fullZipFileName6);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePath6);
                var foldersToDelete6 = new List<string> { "3K", "4K", "6K", "8K", "16K" };
                DeleteFoldersFromZip(fullZipFilePath6, foldersToDelete6);

                // Přesunutí zip souboru do původní složky
                string[] fullZipFilePaths = { fullZipFilePath1, fullZipFilePath2, fullZipFilePath3, fullZipFilePath4, fullZipFilePath5, fullZipFilePath6 };
                string[] fullZipFileNames = { fullZipFileName1, fullZipFileName2, fullZipFileName3, fullZipFileName4, fullZipFileName5, fullZipFileName6 };

                for (int i = 0; i < fullZipFilePaths.Length; i++)
                {
                    File.Move(fullZipFilePaths[i], Path.Combine(selectedFolderPath, fullZipFileNames[i]));
                }

                var fileExtensions = new string[] { ".txt", ".rtf" };
                DeleteFilesFromFolder(selectedFolderPath, fileExtensions);

                string[] foldersToDeleteSource = { "2K", "3K", "4K", "6K", "8K", "16K", "BACKPLATE", "BACKPLATES" };
                DirectoryInfo directory = new DirectoryInfo(selectedFolderPath);
                foreach (string folderName in foldersToDeleteSource)
                {
                    // Projde všechny jména složek ve složce
                    foreach (DirectoryInfo subDirectory in directory.GetDirectories(folderName))
                    {
                        // Vymaže vybraný složky
                        subDirectory.Delete(true);
                    }
                }
            }
        }

        private void ZAZIPUJ_Click(object sender, EventArgs e)
        {
            {
                if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
                {
                    MessageBox.Show("Nebyly vybrány žádné soubory.");
                }
                else
                {
                    ZipFolder();
                    if (MessageBox.Show("Operace byla dokončena!", "Hotovo", MessageBoxButtons.OK) == DialogResult.OK)
                    {
                        Application.Restart();
                    }
                }
            }
        }

        // funkce pro zipování modelů
        private void ZipHDRFile()
        {
            foreach (string selectedFolderPath in selectedFolderPaths)
            {
                /// 1. TASK (3DSmax+corona + tex folder + preview folder)
                // Vytvoření názvu stejný jako původní složka + 3DSMAX+CORONA.zip
                string parentFolderPathHDR = Path.GetDirectoryName(selectedFolderPath);
                // Vezme vše co je za podtržítkem a odstraní to (např. F01)
                int lastIndex = selectedFolderPath.LastIndexOf('_');

                string fullZipFileNameHDR1 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_3DSMAX+CORONA.zip";
                string fullZipFilePathHDR1 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR1);
                // Zazipovani slozky s nazvem a cestou jak jsme si zvolili
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR1);
                // Vymaze soubory a slozky, ktere obsahuji urcite znaky
                string[] searchStrings1 = { "BLENDER", "3DSMAX+VRAY", "CINEMA4D", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR1, searchStrings1);

                /// 2. TASK (3DSmax+vray + tex folder + preview folder)
                //string parentFolderPathHDR2 = Path.GetDirectoryName(selectedFolderPath);
                //int lastIndex2 = selectedFolderPath.LastIndexOf('_');
                string fullZipFileNameHDR2 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_3DSMAX+VRAY.zip";
                string fullZipFilePathHDR2 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR2);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR2);
                string[] searchStrings2 = { "BLENDER", "3DSMAX+CORONA", "CINEMA4D", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR2, searchStrings2);

                /// 3. TASK (Blender + tex folder + preview folder)
                string fullZipFileNameHDR3 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_BLENDER.zip";
                string fullZipFilePathHDR3 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR3);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR3);
                string[] searchString3 = { "3DSMAX", "CINEMA4D", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR3, searchString3);

                /// 4. TASK (Cinema4d + tex folder + preview folder)
                string fullZipFileNameHDR4 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_CINEMA4D.zip";
                string fullZipFilePathHDR4 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR4);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR4);
                string[] searchString4 = { "BLENDER", "3DSMAX", "CINEMA4D+CORONA", "CINEMA4D+VRAY", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR4, searchString4);

                /// 5. TASK (Cinema4d+corona + tex folder + preview folder)
                string fullZipFileNameHDR5 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_CINEMA4D+CORONA.zip";
                string fullZipFilePathHDR5 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR5);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR5);
                string[] searchString5 = { "BLENDER", "3DSMAX", "CINEMA4D.c4d", "CINEMA4D+VRAY", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR5, searchString5);

                /// 6. TASK (Cinema4d+vray + tex folder + preview folder)
                string fullZipFileNameHDR6 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_CINEMA4D+VRAY.zip";
                string fullZipFilePathHDR6 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR6);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR6);
                string[] searchString6 = { "BLENDER", "3DSMAX", "CINEMA4D.c4d", "CINEMA4D+CORONA", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR6, searchString6);

                /// 7. TASK (Sketchup folder + tex folder + preview folder)
                string fullZipFileNameHDR7 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_SKETCHUP.zip";
                string fullZipFilePathHDR7 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR7);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR7);
                string[] searchString7 = { "BLENDER", "3DSMAX", "CINEMA4D", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP+VRAY" };
                RemoveEntriesFromZip(fullZipFilePathHDR7, searchString7);

                /// 8. TASK (Sketchup+vray zip + tex folder + preview folder)
                string fullZipFileNameHDR8 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_SKETCHUP+VRAY.zip";
                string fullZipFilePathHDR8 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR8);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR8);
                string[] searchString8 = { "BLENDER", "3DSMAX", "CINEMA4D", "COLLADA", "FBX", "GLTF", "OBJ", "SKETCHUP/" };
                RemoveEntriesFromZip(fullZipFilePathHDR8, searchString8);

                /// 9. TASK (FBX + tex folder + preview folder + .fbm folder )
                string fullZipFileNameHDR9 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_FBX.zip";
                string fullZipFilePathHDR9 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR9);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR9);
                string[] searchString9 = { "BLENDER", "3DSMAX", "CINEMA4D", "COLLADA", "GLTF", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR9, searchString9);

                /// 10. TASK (Obj + tex folder + preview folder + .mtl file)
                string fullZipFileNameHDR10 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_OBJ.zip";
                string fullZipFilePathHDR10 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR10);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR10);
                string[] searchStrings10 = { "BLENDER", "3DSMAX", "CINEMA4D", "COLLADA", "GLTF", "FBX", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR10, searchStrings10);

                /// 11. TASK (GLTF + tex folder + preview folder)
                string fullZipFileNameHDR11 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_GLTF.zip";
                string fullZipFilePathHDR11 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR11);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR11);
                string[] searchStrings11 = { "BLENDER", "3DSMAX", "CINEMA4D", "COLLADA", "FBX", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR11, searchStrings11);

                /// 12. TASK (Collada + tex folder + preview folder)
                string fullZipFileNameHDR12 = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_COLLADA.zip";
                string fullZipFilePathHDR12 = Path.Combine(parentFolderPathHDR, fullZipFileNameHDR12);
                ZipFile.CreateFromDirectory(selectedFolderPath, fullZipFilePathHDR12);
                string[] searchStrings12 = { "BLENDER", "3DSMAX", "CINEMA4D", "GLTF", "FBX", "OBJ", "SKETCHUP" };
                RemoveEntriesFromZip(fullZipFilePathHDR12, searchStrings12);

                // Vymazani souboru a slozek z primarni slozky
                var fileExtensions = new string[] { ".c4d", ".max", ".zip", ".gltf", ".dae", ".fbx", ".mtl", ".blend", ".obj" };
                DeleteFilesFromFolder(selectedFolderPath, fileExtensions);
                string folderSketchup = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_SKETCHUP";
                string folderFBX = Path.GetFileName(selectedFolderPath.Substring(0, lastIndex)) + "_FBX.fbm";
                string[] foldersToDeleteSource = { "tex", folderFBX, folderSketchup };
                DirectoryInfo directory = new DirectoryInfo(selectedFolderPath);
                foreach (string folderName in foldersToDeleteSource)
                {
                    // projde všechny jména složek ve složce
                    foreach (DirectoryInfo subDirectory in directory.GetDirectories(folderName))
                    {
                        // vymaže vybraný složky
                        subDirectory.Delete(true);
                    }
                }

                // Presune vsechny zipy do puvodni slozky
                string[] fullZipFilePaths = { fullZipFilePathHDR1, fullZipFilePathHDR2, fullZipFilePathHDR3, fullZipFilePathHDR4, fullZipFilePathHDR5, fullZipFilePathHDR6, fullZipFilePathHDR7, fullZipFilePathHDR8, fullZipFilePathHDR9, fullZipFilePathHDR10, fullZipFilePathHDR11, fullZipFilePathHDR12 };
                string[] fullZipFileNames = { fullZipFileNameHDR1, fullZipFileNameHDR2, fullZipFileNameHDR3, fullZipFileNameHDR4, fullZipFileNameHDR5, fullZipFileNameHDR6, fullZipFileNameHDR7, fullZipFileNameHDR8, fullZipFileNameHDR9, fullZipFileNameHDR10, fullZipFileNameHDR11, fullZipFileNameHDR12 };
                for (int i = 0; i < fullZipFilePaths.Length; i++)
                {
                    File.Move(fullZipFilePaths[i], Path.Combine(selectedFolderPath, fullZipFileNames[i]));
                }
            }
        }

        // funkce pro vymazání souborů ze zipu
        private void RemoveEntriesFromZip(string zipFilePath, string[] searchStrings)
        {
            // Vytvoreni prazdneho listu
            List<ZipArchiveEntry> entriesToDelete = new List<ZipArchiveEntry>();

            // Otevre zip soubor s moznosti upravovat ho
            using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
            {
                // Projde vsechny vstupy v zip archivu
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Projde pole stringu, ktere je ulozene ve funkci
                    foreach (string searchString in searchStrings)
                    {
                        // pokud najde string, ktery je ulozen v poli
                        if (entry.FullName.Contains(searchString))
                        {
                            Console.WriteLine(entry.FullName);
                            // Prida hledany vstup na seznam k vymazani
                            entriesToDelete.Add(entry);
                            break;
                        }
                    }
                }

                // Projde kazdy prvek v seznamu vymazani
                foreach (ZipArchiveEntry entry in entriesToDelete)
                {
                    // vymaze postupne kazdy prvek
                    entry.Delete();
                }
            }
        }

        public void DeleteFolderFromZip(string zipPath, string folderName)
        {
            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
            {
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.Contains(folderName) && entry.FullName.EndsWith("/"))
                    {
                        entry.Delete();
                    }
                }
            }
        }

        private void ZAZIPUJ_HDR_Click(object sender, EventArgs e)
        {
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné soubory.");
            }
            else
            {
                ZipHDRFile();
                if (MessageBox.Show("Operace byla dokončena!", "Hotovo", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Application.Restart();
                }
            }
        }

        private void EXIT_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chceš odejít?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

    }
}