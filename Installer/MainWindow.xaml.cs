
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Defolderizer_Installer;


public partial class MainWindow : Window, INotifyPropertyChanged {
    
    private string installPath = "";

    public string InstallPath {
        get { return installPath; }
        set { 
            installPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallPath"));
        }
    }


    private bool? installForAllUsers;

    public bool? InstallForAllUsers {
        get { return installForAllUsers; }
        set { 
            installForAllUsers = value; 
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("InstallForAllUsers"));
        }
    }


    private bool? addToRightClick;

    public bool? AddToRightClick {
        get { return addToRightClick; }
        set { 
            addToRightClick = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AddToRightClick"));
        }
    }


    private string position = "";

    public string Position {
        get { return position; }
        set { 
            position = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
        }
    }



    private string output = "";

    public string Output {
        get { return output; }
        set { 
            output = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Output"));
        }
    }


    private readonly Installer installer;


    public event PropertyChangedEventHandler? PropertyChanged;


    public MainWindow() {
        DataContext = this;
        InitializeComponent();
        installer = new Installer(this);
        InstallPath = "C:\\Program Files\\Defolderizer";
        InstallForAllUsers = true;
        AddToRightClick = true; 
        Output = "Initialized!\n";
        

    }

    private void tbInstallPath_LostFocus(object sender, RoutedEventArgs e) {

        btnInstall.IsEnabled = IsInstallPathValid(tbInstallPath.Text);
    }



    public bool IsInstallPathValid(string path) {
        Regex badChars = new Regex(".*([<>\"|?*]|:[^\\\\]).*"); //good enough for now bruv
        return System.IO.Path.IsPathFullyQualified(path) && !badChars.IsMatch(path);
    }


    private void btnBrowse_Click(object sender, RoutedEventArgs e) {
        OpenFolderDialog dialog = new OpenFolderDialog();
        dialog.InitialDirectory = "C:\\Program Files";
        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult == true) {
            InstallPath = dialog.FolderName+"\\Defolderizer";
        }

    }

    private void btnInstall_Click(object sender, RoutedEventArgs e) {
        installer.SetSettings(InstallPath,AddToRightClick,InstallForAllUsers,"top");
        installer.Install();
    }


    public void Print(string text) {
        Output += text+"\n"; 
    }


    public void ClearOutput() {
        Output = "";   
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
        installer.AddRegistryEdits();
    }
}

public class Installer {

    private string installPath = "";
    private bool? addToRightClick;
    private bool? forAllUsers;
    private string menuPosition = "";
    private MainWindow mainWindow;

    public Installer(MainWindow mainWindow) {
        this.mainWindow = mainWindow;
    }


    public void SetSettings(string installPath, bool? addToRightClick, bool? forAllUsers, string menuPosition) {
        this.installPath = installPath;
        this.addToRightClick = addToRightClick;
        this.forAllUsers = forAllUsers;
        this.menuPosition = menuPosition;
    }


    public void Install() {

        mainWindow.ClearOutput();
        mainWindow.Print("Installation started...");
        mainWindow.Print("Creating Folder..");

        DirectoryInfo installDirectory;
        try {
            installDirectory = Directory.CreateDirectory(installPath);
        }
        catch (Exception e) {
            MessageBox.Show("Failed to Create Directory at \n" + installPath + " due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            mainWindow.Print("Failed to Create Directory at " + installPath + " due to the following error: \n" + e.Message);
            return;
        }

        mainWindow.Print("Directory creation successful!");
        mainWindow.Print("Copying files...");

        try {
            CopyFiles();
        }
        catch (Exception e) {
            MessageBox.Show("Failed to copy files to \n" + installPath + " due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            mainWindow.Print("Failed to copy files to \n" + installPath + " due to the following error: \n" + e.Message);
            return;
        }

        mainWindow.Print("Copying files successful");
        mainWindow.Print("Adding registry entries...");

        if (addToRightClick == true) {
            try {
                AddRegistryEdits();
            }
            catch (Exception e) {
                MessageBox.Show("Registry edits failed due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                mainWindow.Print("Registry edits failed due to the following error: \n" + e.Message);
                return;
            }
            mainWindow.Print("Registry Edits Successful!");
        }
    }


    public void CopyFiles() {
        FileInfo applicationFile = new FileInfo("defolderizer.exe");
        FileInfo configFile = new FileInfo("config.ini");
        applicationFile.CopyTo(Path.Combine(installPath, applicationFile.Name));
        configFile.CopyTo(Path.Combine(installPath, configFile.Name));
    }


    public void AddRegistryEdits() {
        string keyRoot = "";
        string subKey = "";
        string position = "top";
        if (forAllUsers == true) {
            keyRoot = "HKEY_CLASSES_ROOT";
            subKey = "Directory";
        }
        else if (forAllUsers == false) {
            keyRoot = "HKEY_CURRENT_USER";
            subKey = "Software\\Classes\\Directory";
        }
        string backgroundKey = keyRoot + "\\" + subKey + "\\Background\\shell\\";
        string iconKey = keyRoot + "\\" + subKey + "\\shell\\";

        Registry.SetValue(backgroundKey + "\\Defolderize\\command", "", installPath + "\\defolderizer.exe \"%V\" \"defolderize\"");
        Registry.SetValue(backgroundKey + "\\Defolderize", "position", position);
        Registry.SetValue(iconKey + "\\Defolderize\\command", "", installPath + "\\defolderizer.exe \"%V\" \"defolderize\"");
        Registry.SetValue(iconKey + "\\Defolderize", "position", position);

        Registry.SetValue(backgroundKey + "\\Unfold\\command", "", installPath + "\\defolderizer.exe \"%V\" \"unfold\"");
        Registry.SetValue(backgroundKey + "\\Unfold", "position", position);
        Registry.SetValue(iconKey + "\\Unfold\\command", "", installPath + "\\defolderizer.exe \"%V\" \"unfold\"");
        Registry.SetValue(iconKey + "\\Unfold", "position", position);
            
        Registry.SetValue(backgroundKey + "\\Defolderize Recursive\\command", "", installPath + "\\defolderizer.exe \"%V\" \"recursive\"");
        Registry.SetValue(backgroundKey + "\\Defolderize Recursive", "position", position);
        Registry.SetValue(iconKey + "\\Defolderize Recursive\\command", "", installPath + "\\defolderizer.exe \"%V\" \"recursive\"");
        Registry.SetValue(iconKey + "\\Defolderize Recursive", "position", position);
    }

}
