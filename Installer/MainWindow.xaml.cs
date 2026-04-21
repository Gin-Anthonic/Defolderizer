
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Defolderizer_Installer;


public partial class MainWindow : Window {
    public string InstallPath { get; set; } = "C:\\Program Files\\Defolderizer";
    public bool? InstallForAllUsers { get; set; } = true;
    public bool? AddToRightClick { get; set; } = true;
    public bool? AddToPath { get; set; } = false;

    public string Output { get; set; } = "";


    public MainWindow() {
        InitializeComponent();

        tbInstallPath.Text = InstallPath;
        cbInstallForAllUsers.IsChecked = InstallForAllUsers;
        cbAddToRightClick.IsChecked = AddToRightClick;

        Output = "Initialized!";
        tblOutput.Text = "Baba is you!\nBaba is you!\nBaba is you!\nBaba is you!\nBaba is you!\nBaba is you!\nBaba is you!\n";

    }

    private void tbInstallPath_LostFocus(object sender, RoutedEventArgs e) {

        btnInstall.IsEnabled = IsInstallPathValid(tbInstallPath.Text);

    }

    private void tbInstallPath_GotFocus(object sender, RoutedEventArgs e) {
      
    }



    public bool IsInstallPathValid(string path) {
        Regex badChars = new Regex(".*([<>\"|?*]|:[^\\\\]).*"); //good enough for now bruv
        return System.IO.Path.IsPathFullyQualified(path) && !badChars.IsMatch(path);
    }

    private void cbInstallForAllUsers_Checked(object sender, RoutedEventArgs e) {

    }

    private void cbAddToRightClick_Unchecked(object sender, RoutedEventArgs e) {
        cbInstallForAllUsers.IsEnabled = false;
    }

    private void cbAddToRightClick_Checked(object sender, RoutedEventArgs e) {
        cbInstallForAllUsers.IsEnabled = true;
    }

    private void btnBrowse_Click(object sender, RoutedEventArgs e) {
        OpenFolderDialog dialog = new OpenFolderDialog();
        dialog.InitialDirectory = "C:\\Program Files";
        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult == true) {
            tbInstallPath.Text = dialog.FolderName+"\\Defolderizer";
        }

    }

    private void btnInstall_Click(object sender, RoutedEventArgs e) {
        Install();
    }


    public void Print(string text) {
        Output += text+"\n";
        tblOutput.Text = Output;
    }


    public void ClearOutput() {
        Output = "";
        tblOutput.Text = Output;
    }


    public void Install() {
        InstallPath = tbInstallPath.Text;
        InstallForAllUsers = cbInstallForAllUsers.IsChecked;
        AddToRightClick = cbAddToRightClick.IsChecked;


        ClearOutput(); 
        Print("Installation started...");
        Print("Creating Folder..");
        
        DirectoryInfo installDirectory;
        try {
            installDirectory = Directory.CreateDirectory(InstallPath);
        }
        catch (Exception e) {
            MessageBox.Show("Failed to Create Directory at \n" + InstallPath + " due to the following error: \n" + e.Message,"Error!",MessageBoxButton.OK,MessageBoxImage.Error);
            Print("Failed to Create Directory at " + InstallPath + " due to the following error: \n" + e.Message);
            return;
        }

        Print("Directory creation successful!");
        Print("Copying files...");

        try {
            CopyFiles();
        }
        catch (Exception e) {
            MessageBox.Show("Failed to copy files to \n" + InstallPath + " due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            Print("Failed to copy files to \n" + InstallPath + " due to the following error: \n" + e.Message);
            return;
        }

        Print("Copying files successful");
        Print("Adding registry entries...");

        if (AddToRightClick == true) {
            try {
                AddRegistryEdits();
            }
            catch(Exception e) {
                MessageBox.Show("Registry edits failed due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                Print("Registry edits failed due to the following error: \n" + e.Message);
                return;
            }
            Print("Registry Edits Successful!");
        }
    }


    public void CopyFiles() {
        FileInfo applicationFile = new FileInfo("defolderizer.exe");
        FileInfo configFile = new FileInfo("config.ini");
        applicationFile.CopyTo(Path.Combine(InstallPath, applicationFile.Name));
        configFile.CopyTo(Path.Combine(InstallPath, configFile.Name));
    }


    public void AddRegistryEdits() {
        string keyRoot = "";
        string subKey = "";
        string position = "top";
        if (InstallForAllUsers == true) {
            keyRoot = "HKEY_CLASSES_ROOT";
            subKey = "Directory";
        }
        else if (InstallForAllUsers == false) {
            keyRoot = "HKEY_CURRENT_USER";
            subKey = "Software\\Classes\\Directory";
        }
        string backgroundKey = keyRoot + "\\" + subKey + "\\Background\\shell\\";
        string iconKey = keyRoot + "\\" + subKey + "\\shell\\";

        Registry.SetValue(backgroundKey + "\\Defolderize\\command", "", InstallPath + "\\defolderizer.exe \"%V\" \"defolderize\"");
        Registry.SetValue(backgroundKey + "\\Defolderize","position",position);
        Registry.SetValue(iconKey + "\\Defolderize\\command", "", InstallPath + "\\defolderizer.exe \"%V\" \"defolderize\"");
        Registry.SetValue(iconKey + "\\Defolderize","position",position);

        Registry.SetValue(backgroundKey + "\\Unfold\\command", "", InstallPath + "\\defolderizer.exe \"%V\" \"unfold\"");
        Registry.SetValue(backgroundKey + "\\Unfold", "position", position);
        Registry.SetValue(iconKey + "\\Unfold\\command", "", InstallPath + "\\defolderizer.exe \"%V\" \"unfold\"");
        Registry.SetValue(iconKey + "\\Unfold", "position", position);

        Registry.SetValue(backgroundKey + "\\Defolderize Recursive\\command", "", InstallPath + "\\defolderizer.exe \"%V\" \"recursive\"");
        Registry.SetValue(backgroundKey + "\\Defolderize Recursive", "position", position);
        Registry.SetValue(iconKey + "\\Defolderize Recursive\\command", "", InstallPath + "\\defolderizer.exe \"%V\" \"recursive\"");
        Registry.SetValue(iconKey + "\\Defolderize Recursive", "position", position);
    }


    private void Button_Click(object sender, RoutedEventArgs e) {
        try {
            Registry.ClassesRoot.DeleteSubKeyTree("Directory\\Background\\shell\\Defolderize");
            Registry.ClassesRoot.DeleteSubKeyTree("Directory\\Background\\shell\\unfold");
            Registry.ClassesRoot.DeleteSubKeyTree("Directory\\Background\\shell\\Defolderize Recursive");

            Registry.ClassesRoot.DeleteSubKeyTree("Directory\\shell\\Defolderize");
            Registry.ClassesRoot.DeleteSubKeyTree("Directory\\shell\\unfold");
            Registry.ClassesRoot.DeleteSubKeyTree("Directory\\shell\\Defolderize Recursive");
        } catch (Exception ex) {
            Print(ex.ToString());
            return;
        }
        Print("Successful destruction!");
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
        AddRegistryEdits();
    }
}


