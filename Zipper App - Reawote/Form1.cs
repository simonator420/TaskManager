using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace Zipper_App___Reawote
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        // vytvoření list, do kterého se uloží všechny vybrané cesty
        public static List<string> selectedFolderPaths = new List<string>();

        // metoda pro výbrání složek, které se uloží do listu
        private void vyberButton_Click(object sender, EventArgs e)
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

        // button, pro vybrání cesty ke zkopírování složek a souborů bez složky SOURCE
        private void presunSlozkuButton_Click(object sender, EventArgs e)
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
                    dialog.IsFolderPicker = true;
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
                    Application.Restart();
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
                if (new DirectoryInfo(subDirPath).Name == "SOURCE")
                {
                    // pokud nějakou najde, tak ji přeskočí a nebude se kopírovat
                    continue;
                }
                // získání jména složky
                string subDirName = new DirectoryInfo(subDirPath).Name;
                // vytvoření cílové cesty pro složky
                string destSubDirPath = Path.Combine(destDirPath, subDirName);
                // rekurzivni volání funkce sama sebe a opakování procesu pro všechny složky které uloženy v listu
                CopyDirectory(subDirPath, destSubDirPath);
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
            var filesToDelete = Directory.GetFiles(folderPath).Where(f => fileExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));

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

        private void zazipujHDRButton_Click(object sender, EventArgs e)
        {
            {
                if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
                {
                    MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
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

        private void zazipujModelyButton_Click(object sender, EventArgs e)
        {
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
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

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chceš odejít?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void najdiPreviewButton_Click(object sender, EventArgs e)
        {
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
            }
            else
            {
                var previewName = previewNameBox.Text;
                foreach (var folderPath in selectedFolderPaths)
                {
                    var imageFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                        .Where(file => file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png"))
                        .Where(file => Path.GetFileNameWithoutExtension(file).StartsWith(Path.GetFileName(folderPath)))
                        .ToList();
                    foreach (var imageFile in imageFiles)
                    {
                        var imageName = Path.GetFileNameWithoutExtension(imageFile);
                        var imageFolder = Directory.GetDirectories(folderPath, imageName, SearchOption.AllDirectories).FirstOrDefault();
                        if (imageFolder != null)
                        {
                            var previewFolder = Path.Combine(imageFolder, "Preview");
                            Directory.CreateDirectory(previewFolder);
                            var extension = Path.GetExtension(imageFile).ToLower();
                            //var destinationPath = Path.Combine(previewFolder, Path.GetFileName(imageFile));
                            var destinationPath = Path.Combine(previewFolder, previewName + extension);
                            if (!File.Exists(destinationPath))
                            {
                                File.Copy(imageFile, destinationPath);
                            }
                        }
                    }
                }

                if (MessageBox.Show("Operace byla dokončena!", "Hotovo", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        // funkce pro vyhledavani map z cesty a ukladani jich do neove slozky
        private void najdiMapy(string mapName, List<string> selectedFolderpaths, string destPath)
        {
            // projde vsechny vybrane cesty
            foreach (var folderPath in selectedFolderPaths)
            {
                // vyhleda vsechny soubory se zadanym jmenem
                string[] files = Directory.GetFiles(folderPath, $"*_{mapName}_*.jpg", SearchOption.AllDirectories);
                // projde vsechny nalezene soubory s vybranyn jmenem
                foreach (string file in files)
                {
                    // jmeno souboru
                    string name = Path.GetFileName(file);
                    // cesta vybraneho souboru
                    string destFile = Path.Combine(destPath, name);
                    // pokud soubor uz neexistuje, tak se nakopiruje do nami vybrane slozky
                    try
                    {
                        File.Copy(file, destFile);
                    }
                    catch (IOException)
                    {
                        continue;
                    }
                }
            }
        }

        private void najdiMapyButton_Click(object sender, EventArgs e)
        {
            // pokud je vybrana alespon jedna cesta
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                // tak se zobrazi chybove hlaseni
                MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
            }
            // pokud neni vybran ani jeden checkbox
            else if ( !ao_cb.Checked && !nrm_cb.Checked && !disp_cb.Checked && !diff_cb.Checked && !col_cb.Checked && !gloss_cb.Checked && !metal_cb.Checked && !spec_cb.Checked && !sss_cb.Checked && !sssabsorb_cb.Checked && !opac_cb.Checked && !anis_cb.Checked && !sheen_cb.Checked)
            {
                // tak se zobrazi chybove hlaseni
                MessageBox.Show("Nebyly vybrány žádné mapy.", "Chyba");
            }
            else
            {
                // vytvoření dialogového ok
                using (var dialog = new CommonOpenFileDialog("Vyber cílovou složku"))
                {
                    dialog.IsFolderPicker = true;
                    dialog.Multiselect = false;
                    // po stisknutí tlačítka pro potvrzení cesty
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        // ulozeni cesty do promenne
                        string destPath = dialog.FileName;
                        // projde vsechny vybrane cesty
                        foreach (var folderPath in selectedFolderPaths)
                        {
                            // zkontroluje vsechny checkboxy a provede operaci
                            if (ao_cb.Checked)
                            {
                                najdiMapy("AO", selectedFolderPaths, destPath);
                            }

                            if (nrm_cb.Checked)
                            {
                                najdiMapy("NRM", selectedFolderPaths, destPath);
                            }
                            if (disp_cb.Checked)
                            {
                                najdiMapy("DISP", selectedFolderPaths, destPath);
                            }
                            if (diff_cb.Checked)
                            {
                                najdiMapy("DIFF", selectedFolderPaths, destPath);
                            }
                            if (col_cb.Checked)
                            {
                                najdiMapy("COL", selectedFolderPaths, destPath);
                            }
                            if (gloss_cb.Checked)
                            {
                                najdiMapy("GLOSS", selectedFolderPaths, destPath);
                            }
                            if (metal_cb.Checked)
                            {
                                najdiMapy("METAL", selectedFolderPaths, destPath);
                            }
                            if (spec_cb.Checked)
                            {
                                najdiMapy("SPEC", selectedFolderPaths, destPath);
                            }
                            if (sss_cb.Checked)
                            {
                                najdiMapy("SSS", selectedFolderPaths, destPath);
                            }
                            if (sssabsorb_cb.Checked)
                            {
                                najdiMapy("SSSABSORB", selectedFolderPaths, destPath);
                            }
                            if (opac_cb.Checked)
                            {
                                najdiMapy("OPAC", selectedFolderPaths, destPath);
                            }
                            if (anis_cb.Checked)
                            {
                                najdiMapy("ANIS", selectedFolderPaths, destPath);
                            }
                            if (sheen_cb.Checked)
                            {
                                najdiMapy("SHEEN", selectedFolderPaths, destPath);
                            }
                        }
                        if (MessageBox.Show("Operace byla dokončena!", "Hotovo", MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            Application.Restart();
                        }
                    }
                }
            }
        }

        // button pro zaskrtnuti vsech checkboxu
        private void selectAllButton_Click(object sender, EventArgs e)
        {
            if (!ao_cb.Checked && !nrm_cb.Checked && !disp_cb.Checked && !diff_cb.Checked && !col_cb.Checked && !gloss_cb.Checked && !metal_cb.Checked && !spec_cb.Checked && !sss_cb.Checked && !sssabsorb_cb.Checked && !opac_cb.Checked && !anis_cb.Checked && !sheen_cb.Checked)
            {
                ao_cb.Checked = true;
                nrm_cb.Checked = true;
                disp_cb.Checked = true;
                diff_cb.Checked = true;
                col_cb.Checked = true;
                gloss_cb.Checked = true;
                metal_cb.Checked = true;
                spec_cb.Checked = true;
                sss_cb.Checked = true;
                sssabsorb_cb.Checked = true;
                opac_cb.Checked = true;
                anis_cb.Checked = true;
                sheen_cb.Checked = true;
                rough_cb.Checked = true;
            }
            else
            {
                ao_cb.Checked = false;
                nrm_cb.Checked = false;
                disp_cb.Checked = false;
                diff_cb.Checked = false;
                col_cb.Checked = false;
                gloss_cb.Checked = false;
                metal_cb.Checked = false;
                spec_cb.Checked = false;
                sss_cb.Checked = false;
                sssabsorb_cb.Checked = false;
                opac_cb.Checked = false;
                anis_cb.Checked = false;
                sheen_cb.Checked = false;
            }
        }

        // funkce pro otočení všech vybraných bitmap
        // není potřeba klikat na vyber, ale soubory se použijí z dialogu, který se otevře po stisknutí tlačítka.
        private void RotateImages(RotateFlipType rotateType)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    selectedFolderPaths.Clear();
                    selectedFolderPaths.AddRange(dialog.FileNames);
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(selectedFolderPaths.ToArray());

                    foreach (var bitmapPath in selectedFolderPaths)
                    {
                        using (Bitmap originalBitmap = new Bitmap(bitmapPath))
                        {
                            originalBitmap.RotateFlip(rotateType);
                            originalBitmap.Save(bitmapPath);
                        }
                    }
                    MessageBox.Show("Soubory byly úspěšně otočeny.", "Hotovo");
                }
                else
                {
                    MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
                }
            }
        }

        private void otocDolevaButton_Click(object sender, EventArgs e)
        {
            RotateImages(RotateFlipType.Rotate270FlipNone);
        }

        private void otocDopravaButton_Click(object sender, EventArgs e)
        {
            RotateImages(RotateFlipType.Rotate90FlipNone);
        }

        private void rotateHelp_Click(object sender, EventArgs e)
        {
            string message = "1. Klikni na OTOČ DOPRAVA/DOLEVA (není třeba klikat na VYBER nahoře).\n" +
                     "2. Vyber všechny soubory, které chceš otočit.\n" +
                     "3. Klikni na Otevřít";

            MessageBox.Show(message, "Help");
        }

        private void vygenerujCheckboxyButton_Click(object sender, EventArgs e)
        {
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
            }
            else
            {
                var checkboxForm = new CheckboxForm();
                List<string> allItems = new List<string>();

                foreach (var folder in selectedFolderPaths)
                {
                    string[] items = Directory.GetFileSystemEntries(folder); // Get folders and files
                    foreach (var item in items)
                    {
                        string itemName = Path.GetFileName(item);
                        allItems.Add(itemName);
                    }
                }

                // Sort the items alphabetically
                allItems.Sort();

                // Clear the checkedListBox1
                checkboxForm.checkedListBox1.Items.Clear();

                // Add the sorted items back to checkedListBox1
                foreach (var item in allItems)
                {
                    Console.WriteLine("Tohle je item z allItems: " + item);
                    checkboxForm.checkedListBox1.Items.Add(item);
                }

                checkboxForm.ShowDialog();
            }
        }

        /*private void vygenerujCheckboxyButton_Click(object sender, EventArgs e)
        {
          if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
        {
          MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
        }
        else
        {
        var checkboxForm = new CheckboxForm();
        List<string> allItems = new List<string>();

        foreach (var folder in selectedFolderPaths)
        {
        string[] items = Directory.GetFileSystemEntries(folder); // Get folders and files
        foreach (var item in items)
        {
          string itemName = Path.GetFileName(item);
            allItems.Add(itemName);
          }
        }

        // Sort the items alphabetically
        allItems.Sort();

        // Clear the checkedListBox1
        checkboxForm.checkedListBox1.Items.Clear();

        // Add the sorted items back to checkedListBox1
        foreach (var item in allItems)
        {
          checkboxForm.checkedListBox1.Items.Add(item);
          }

            checkboxForm.ShowDialog();
          }
        }*/


        private void convertHelp_Click(object sender, EventArgs e)
        {
            string message = "1. Vyber složku/y, ve kterých se nachází složky s materiály pro převod (např. ADO).\n" +
                     "2. Klikni na VYGENERUJ CHECKBOXY PRO PŘEVOD DO .JPG.\n" +
                     "3. V okně vyber checkboxy materiálů, které chceš převést.\n" +
                     "4. Klikni na PŘEVEĎ DO .JPG";

            MessageBox.Show(message, "Help");
        }

        public List<string> getSelectedFolderPaths()
        {
            return selectedFolderPaths;
        }

        private void zkopirujMapyButton_Click(object sender, EventArgs e)
        {
            if (selectedFolderPaths == null || selectedFolderPaths.Count == 0)
            {
                MessageBox.Show("Nebyly vybrány žádné soubory.", "Chyba");
                return;
            }

            if (!ao_cb.Checked && !nrm_cb.Checked && !disp_cb.Checked && !diff_cb.Checked && // ... other checks
                !spec_cb.Checked && !sss_cb.Checked && !sssabsorb_cb.Checked &&
                !opac_cb.Checked && !anis_cb.Checked && !sheen_cb.Checked)
            {
                MessageBox.Show("Nebyly vybrány žádné mapy.", "Chyba");
                return;
            }

            string sourceResolutionFolder = FindResolutionFolder(selectedFolderPaths[0]);
            if (string.IsNullOrEmpty(sourceResolutionFolder))
            {
                MessageBox.Show("Složka s rozlišením nebyla nalezena.", "Chyba");
                return;
            }

            List<string> filesToCopy = FindFilesBasedOnCheckbox(sourceResolutionFolder);

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (var folder in dialog.FileNames)
                    {
                        string destinationFolderPath = folder;
                        string destinationResolutionFolder = FindOrCreateResolutionFolder(destinationFolderPath, sourceResolutionFolder);
                        string folderName = new DirectoryInfo(folder).Name;

                        foreach (var file in filesToCopy)
                        {
                            string newFileName = CreateNewFileName(file, folderName);
                            string destFile = Path.Combine(destinationResolutionFolder, newFileName);
                            File.Copy(file, destFile, overwrite: true); // Set overwrite based on your requirements
                        }
                    }

                    MessageBox.Show("Soubory byly úspěšně zkopírovány.", "Úspěch");
                }
            }
        }


        private string FindResolutionFolder(string basePath)
        {
            var directories = Directory.GetDirectories(basePath);
            return directories.FirstOrDefault(dir => dir.EndsWith("K"));
        }

        private List<string> FindFilesBasedOnCheckbox(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            List<string> selectedFiles = new List<string>();

            foreach (var file in files)
            {
                var parts = Path.GetFileName(file).Split('_');
                if (parts.Length < 4) continue;

                string tag = parts[3];
                if ((ao_cb.Checked && tag == "AO") ||
                    (nrm_cb.Checked && (tag == "NRM" || tag == "NRM16")) ||
                    (disp_cb.Checked && (tag == "DISP" || tag == "DISP16")) ||
                    (rough_cb.Checked && tag == "ROUGH") ||
                    (diff_cb.Checked && tag == "DIFF") ||
                    (gloss_cb.Checked && tag == "GLOSS") ||
                    (metal_cb.Checked && tag == "METAL") ||
                    (spec_cb.Checked && tag == "SPEC") ||
                    (sss_cb.Checked && tag == "SSS") ||
                    (sssabsorb_cb.Checked && tag == "SSSABSORB") ||
                    (opac_cb.Checked && tag == "OPAC") ||
                    (anis_cb.Checked && tag == "ANIS") ||
                    (sheen_cb.Checked && tag == "SHEEN"))
                {
                    selectedFiles.Add(file);
                }
            }

            return selectedFiles;
        }

        private string FindOrCreateResolutionFolder(string basePath, string sourceResolutionFolder)
        {
            string resolutionFolderName = Path.GetFileName(sourceResolutionFolder);
            string destinationResolutionFolder = Path.Combine(basePath, resolutionFolderName);

            if (!Directory.Exists(destinationResolutionFolder))
            {
                Directory.CreateDirectory(destinationResolutionFolder);
            }

            return destinationResolutionFolder;
        }

        private string CreateNewFileName(string originalFilePath, string folderName)
        {
            // Extracting the file extension (e.g., ".jpg")
            string extension = Path.GetExtension(originalFilePath);

            // Splitting the original file name into parts
            string[] parts = Path.GetFileNameWithoutExtension(originalFilePath).Split('_');

            // Extracting the map type (assuming it is the last two parts of the file name)
            string mapType = parts.Length >= 2 ? parts[parts.Length - 2] + "_" + parts[parts.Length - 1] : "";

            // Modifying the folder name to remove everything after the last underscore
            int lastIndex = folderName.LastIndexOf('_');
            string modifiedFolderName = lastIndex > 0 ? folderName.Substring(0, lastIndex) : folderName;

            // Constructing the new file name
            return $"{modifiedFolderName}_{mapType}{extension}";
        }



        private void zkopirujMapyHelp_Click(object sender, EventArgs e)
        {
            string message = "1. Pomocí tlačítka VYBER zvol zdrojovou složku, ze které se bude kopírovat.\n" +
                     "2. Pomocí checkboxů vyber mapy, které chceš, aby se zkopírovaly.\n" +
                     "3. Klikni na ZKOPÍRUJ MAPY a vyber složky, do kterých chceš mapy zkopírovat.\n";

            MessageBox.Show(message, "Help");
        }
    }
}    