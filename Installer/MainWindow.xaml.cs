
using Microsoft.Win32;
using System.ComponentModel;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using Defolderizer_Installer.Services;
using Defolderizer_Installer.Interfaces;


namespace Defolderizer_Installer;

public partial class MainWindow : Window, INotifyPropertyChanged {

    private string installPath = "";

    public string InstallPath {
        get { return installPath; }
        set {
            installPath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallPath"));
            _installerService.UpdateSettings();
            _installPathStatus = _installerService.CheckInstallValidity();
            InstallPossible = (_installPathStatus == InstallCheckResult.OK);
            UpdateInstallPathStatusFeedback();
        }
    }

    private bool? installForAllUsers;

    public bool? InstallForAllUsers {
        get { return installForAllUsers; }
        set {
            installForAllUsers = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallForAllUsers"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallForCurrentUser"));
        }
    }


    public bool? InstallForCurrentUser {
        get { return !installForAllUsers; }
        set {
            InstallForAllUsers = !value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallForCurrentUsers"));
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

    private bool hasAdminPrivileges;

    public bool HasAdminPrivileges {
        get { return hasAdminPrivileges; }
        set {
            hasAdminPrivileges = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasAdminPrivileges"));
        }
    }

    private bool installPossible;

    public bool InstallPossible {
        get { return installPossible; }
        set { 
            installPossible = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallPossible"));
        }
    }

    private string installPathStatusFeedback = "";

    public string InstallPathStatusFeedback {
        get { return installPathStatusFeedback; }
        set {
            installPathStatusFeedback = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallPathStatusFeedback"));
        }
    } 


    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly InstallerService _installerService;
    private readonly IRegistryService _registryService;

    private readonly string[] _menuPositions = ["Bottom","Top",""];
    private InstallCheckResult _installPathStatus;


    public MainWindow() {
        DataContext = this;
        InitializeComponent();
        _registryService = new RegistryService();
        _installerService = new InstallerService(this, _registryService);
        HasAdminPrivileges = CheckAdminPrivileges();
     
        if (hasAdminPrivileges) {
            SetDefaultAdminSettings();
        }
        else {
            SetDefaultUserSettings();
        }
        UpdateInstallPathStatusFeedback();
        _installerService.UpdateSettings();
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

    private void UpdateInstallPathStatusFeedback() {
        Dictionary<InstallCheckResult, string> statusMessages = new Dictionary<InstallCheckResult, string>() {
            {InstallCheckResult.OK,             "" },
            {InstallCheckResult.PathFaulty,     "Install Path is faulty" },
            {InstallCheckResult.NotWriteable,   "The Directory is not writable" },
            {InstallCheckResult.FolderNotEmpty, "The Directory is not empty" }
        };

        InstallPathStatusFeedback = statusMessages[_installPathStatus];
    }

    private void TbInstallPath_KeyDown(object sender, KeyEventArgs e) { 
        if (e.Key == Key.Enter){
            Keyboard.Focus(TblOutput);
        }
    }

    private void BtnBrowse_Click(object sender, RoutedEventArgs e) {
        OpenFolderDialog dialog = new OpenFolderDialog();
        dialog.InitialDirectory = installPath;
        bool? dialogResult = dialog.ShowDialog();
        if (dialogResult == true) {
            InstallPath = dialog.FolderName + "\\Defolderizer";
        }
    }

    private void BtnInstall_Click(object sender, RoutedEventArgs e) {
        _installerService.UpdateSettings();
        _installerService.Install();
    }

    private void BtnRemoveForThisPC_Click(object sender, RoutedEventArgs e) {
        try {
            _registryService.RemoveRegistryEdits(true);
            Print("Menu Cleared!");
        }
        catch (Exception ex) {
            MessageBox.Show("Menu Clearing failed due to the following error:\n" + ex.Message);
        }
    }

    private void BtnRemoveForUser_Click(object sender, RoutedEventArgs e) {
        try {
            _registryService.RemoveRegistryEdits(false);
            Print("Menu Cleared!");
        }
        catch (Exception ex) {
            MessageBox.Show("Menu Clearing failed due to the following error:\n" + ex.Message);
        }
    }

    private void BtnUpdateMenu_Click(object sender, RoutedEventArgs e) {
        try {
            _registryService.AddRegistryEdits(InstallForAllUsers, InstallPath, _menuPositions[selectedMenuPositionIndex]);
            Print("Menu Updated!");
        }
        catch (Exception ex) {
            MessageBox.Show("Menu Update failed due to the following error:\n" + ex.Message);
        }
    }


    public void Print(string text) {
        Output += text + "\n";
    }


    public void ClearOutput() {
        Output = "";
    }


    private bool CheckAdminPrivileges() {
        bool isElevated;
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        return isElevated;
    }

}





