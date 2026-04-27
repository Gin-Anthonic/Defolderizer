
using Microsoft.Win32;
using System.ComponentModel;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallForAllUsers"));
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
    private readonly RegistryService registryService;

    public event PropertyChangedEventHandler? PropertyChanged;


    public MainWindow() {
        DataContext = this;
        InitializeComponent();
        registryService = new RegistryService();
        installer = new Installer(this, registryService);
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
            InstallPath = dialog.FolderName + "\\Defolderizer";
        }

    }

    private void btnInstall_Click(object sender, RoutedEventArgs e) {
        installer.SetSettings(InstallPath, AddToRightClick, InstallForAllUsers, "top");
        installer.Install();
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
        registryService.RemoveRegistryEdits();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
        installer.SetSettings(InstallPath, AddToRightClick, InstallForAllUsers, "top");
        installer.AddRegEdits();
    }


    public void Print(string text) {
        Output += text + "\n";
    }


    public void ClearOutput() {
        Output = "";
    }

}





