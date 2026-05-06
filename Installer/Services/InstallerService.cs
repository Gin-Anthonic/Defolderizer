using Defolderizer_Installer.Interfaces;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Defolderizer_Installer.Services;

public enum InstallCheckResult { 
    OK,
    FolderNotEmpty,
    NotWriteable,
    PathFaulty
}

public class InstallerService {

    private string _installPath = "";
    private bool? _addToRightClick;
    private bool? _forAllUsers;
    private string _menuPosition = "";
    private readonly MainWindow _mainWindow;
    private readonly IRegistryService _registryService;
    private readonly ILoggingService _loggingService = new FileLoggingService();
    
    private readonly string[] menuPositions = ["Bottom", "Top", ""];
    private readonly string[] installFiles = [
        "defolderizer.exe",
        "config.ini"
        ];


    public InstallerService(MainWindow mainWindow, IRegistryService registryService) {
        _mainWindow = mainWindow;
        _registryService = registryService;
    }


    public void UpdateSettings() {
        _installPath = _mainWindow.InstallPath;
        _addToRightClick = _mainWindow.AddToRightClick;
        _forAllUsers = _mainWindow.InstallForAllUsers;
        _menuPosition = menuPositions[_mainWindow.SelectedMenuPositionIndex];
    }


    public void Install() {

        _mainWindow.ClearOutput();
        _mainWindow.Print("Installation started...");
        _mainWindow.Print("Creating Folder..");

        DirectoryInfo installDirectory;
        try {
            installDirectory = Directory.CreateDirectory(_installPath);
        }
        catch (Exception e) {
            MessageBox.Show("Failed to Create Directory at \n" + _installPath + " due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            _mainWindow.Print("Failed to Create Directory at " + _installPath + " due to the following error: \n" + e.Message);
            _loggingService.CreateInstallFailureLog(_mainWindow.Output,e,_mainWindow.HasAdminPrivileges,_forAllUsers);
            return;
        }

        _mainWindow.Print("Directory creation successful!");
        _mainWindow.Print("Copying files...");

        try {
            CopyFiles();
        }
        catch (Exception e) {
            MessageBox.Show("Failed to copy files to \n" + _installPath + " due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            _mainWindow.Print("Failed to copy files to \n" + _installPath + " due to the following error: \n" + e.Message);
            _loggingService.CreateInstallFailureLog(_mainWindow.Output, e, _mainWindow.HasAdminPrivileges, _forAllUsers);
            return;
        }

        _mainWindow.Print("Copying files successful");
        _mainWindow.Print("Adding registry entries...");

        if (_addToRightClick == true) {
            try {
                _registryService.AddRegistryEdits(_forAllUsers, _installPath,_menuPosition);
            }
            catch (Exception e) {
                MessageBox.Show("Registry edits failed due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                _mainWindow.Print("Registry edits failed due to the following error: \n" + e.Message);
                _loggingService.CreateInstallFailureLog(_mainWindow.Output, e, _mainWindow.HasAdminPrivileges, _forAllUsers);
                return;
            }
            _mainWindow.Print("Registry Edits Successful!");
        }
    }

    //kinda yucky but works for now
    public InstallCheckResult CheckInstallValidity() {
        Regex badChars = new Regex(".*([<>\"|?*]|:[^\\\\]).*"); //good enough for now bruv
        if (Path.IsPathFullyQualified(_installPath) && !badChars.IsMatch(_installPath) == false) {
            return InstallCheckResult.PathFaulty;
        }

        string pathToTry = _installPath;

        if (Directory.Exists(_installPath)) {
            if (Directory.EnumerateFileSystemEntries(_installPath).Count() != 0) {
                return InstallCheckResult.FolderNotEmpty;
            }
            pathToTry = Path.Combine(_installPath, "test");
        }
        try {
            Directory.CreateDirectory(pathToTry);
            Directory.Delete(pathToTry, true);
        }
        catch {
            return InstallCheckResult.NotWriteable;
        }
        return InstallCheckResult.OK;
    }

    
    public bool InstallExists() {
        foreach (string fileName in installFiles) {
            if (File.Exists(Path.Combine(_installPath, fileName)) == false) {
                return false;
            }
        }
        return true;
    }


    private void CopyFiles() {
        foreach (string fileName in installFiles) {
            FileInfo currentFile = new FileInfo(fileName);
            currentFile.CopyTo(Path.Combine(_installPath,currentFile.Name));
        }
    }


    public static string DefaultInstallPath = "C:\\Program Files\\Defolderizer";


    public static string GetDefaultUserInstallPath() {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "programs", "Defolderizer");
    }

}
