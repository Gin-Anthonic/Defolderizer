
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
            UpdateForAllUsers();
        }
    }

    private int selectedMenuPositionIndex;

    public int SelectedMenuPositionIndex {
        get { return selectedMenuPositionIndex; }
        set { 
            selectedMenuPositionIndex = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedMenuPositionIndex"));
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly InstallerService installerService;
    private readonly RegistryService registryService;

    private readonly string[] menuPositions = ["Bottom","Top",""];

    public MainWindow() {
        DataContext = this;
        InitializeComponent();
        registryService = new RegistryService();
        installerService = new InstallerService(this, registryService);
     
        if (HasAdminPrivileges()) {
            SetDefaultAdminSettings();
        }
        else {
            SetDefaultUserSettings();
        }

        installerService.UpdateSettings();
        Output = "Initialized!\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\nBaba is you\n";
    }


    private void SetDefaultAdminSettings() {
        InstallPath = InstallerService.DefaultInstallPath;
        InstallForAllUsers = true;
        AddToRightClick = true;
        SelectedMenuPositionIndex = 1;
    }

    private void SetDefaultUserSettings() {
        InstallPath = InstallerService.GetDefaultUserInstallPath();
        InstallForAllUsers = false;
        AddToRightClick = true;
        SelectedMenuPositionIndex = 1;
    }

    private void TbInstallPath_LostFocus(object sender, RoutedEventArgs e) {
        var binding = TbInstallPath.GetBindingExpression(TextBox.TextProperty); //force the DataBinding to Update
        binding?.UpdateSource();

        BtnInstall.IsEnabled = IsInstallPathValid(TbInstallPath.Text);
        
        installerService.UpdateSettings();

    }


    private void BtnBrowse_Click(object sender, RoutedEventArgs e) {
        OpenFolderDialog dialog = new OpenFolderDialog();
        dialog.InitialDirectory = "C:\\Program Files";
        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult == true) {
            InstallPath = dialog.FolderName + "\\Defolderizer";
        }
    }

    private void BtnInstall_Click(object sender, RoutedEventArgs e) {
        installerService.UpdateSettings();
        installerService.Install();
    }

    private void BtnRemoveRegistryEdits_Click(object sender, RoutedEventArgs e) {
        registryService.RemoveRegistryEdits(InstallForAllUsers);
    }

    private void BtnAddRegistryEdits_Click(object sender, RoutedEventArgs e) {
        installerService.UpdateSettings();
        installerService.AddRegEdits();
    }



    public void Print(string text) {
        Output += text + "\n";
    }


    public void ClearOutput() {
        Output = "";
    }


    private void UpdateForAllUsers() {
        if (addToRightClick == false) { 
            InstallForAllUsers = AddToRightClick; 
        }
    }


    public bool IsInstallPathValid(string path) {
        Regex badChars = new Regex(".*([<>\"|?*]|:[^\\\\]).*"); //good enough for now bruv
        return System.IO.Path.IsPathFullyQualified(path) && !badChars.IsMatch(path);
    }


    private bool HasAdminPrivileges() {
        bool isElevated;
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        return isElevated;
    }

}





