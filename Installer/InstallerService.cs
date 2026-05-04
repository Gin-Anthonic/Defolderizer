using System.IO;
using System.Printing;
using System.Text.RegularExpressions;
using System.Windows;


namespace Defolderizer_Installer;

public enum InstallCheckResult { 
    OK,
    FolderNotEmpty,
    NotWriteable,
    PathFaulty
}

public class InstallerService {

    private string installPath = "";
    private bool? addToRightClick;
    private bool? forAllUsers;
    private string menuPosition = "";
    private MainWindow mainWindow;
    private RegistryService registryService;
    private readonly string[] menuPositions = ["Bottom", "Top", ""];

    private readonly string[] installFiles = [
        "defolderizer.exe",
        "config.ini"
        ];

    public InstallerService(MainWindow mainWindow, RegistryService registryService) {
        this.mainWindow = mainWindow;
        this.registryService = registryService;
    }


    public void UpdateSettings() {
        installPath = mainWindow.InstallPath;
        addToRightClick = mainWindow.AddToRightClick;
        forAllUsers = mainWindow.InstallForAllUsers;
        menuPosition = menuPositions[mainWindow.SelectedMenuPositionIndex];
       // mainWindow.Print(installPath + "\n" + addToRightClick.ToString() + forAllUsers.ToString() + menuPosition);
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
                registryService.AddRegistryEdits(forAllUsers, installPath,menuPosition);
            }
            catch (Exception e) {
                MessageBox.Show("Registry edits failed due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                mainWindow.Print("Registry edits failed due to the following error: \n" + e.Message);
                return;
            }
            mainWindow.Print("Registry Edits Successful!");
        }
    }

    //kinda yucky but works for now
    public InstallCheckResult CheckInstallValidity() {
        Regex badChars = new Regex(".*([<>\"|?*]|:[^\\\\]).*"); //good enough for now bruv
        if (Path.IsPathFullyQualified(installPath) && !badChars.IsMatch(installPath) == false) {
            return InstallCheckResult.PathFaulty;
        }

        string pathToTry = installPath;

        if (Directory.Exists(installPath)) {
            if (Directory.EnumerateFileSystemEntries(installPath).Count() != 0) {
                return InstallCheckResult.FolderNotEmpty;
            }
            pathToTry = Path.Combine(installPath, "test");
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
            if (File.Exists(Path.Combine(installPath, fileName)) == false) {
                return false;
            }
        }
        return true;
    }


    public void AddRegEdits() {
        try {
            registryService.AddRegistryEdits(forAllUsers, installPath, menuPosition);
        }
        catch (Exception e) {
            MessageBox.Show("Registry edits failed due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            mainWindow.Print("Registry edits failed due to the following error: \n" + e.Message);
            return;
        }
        mainWindow.Print("Registry Edits Successful!");
    }


    private void CopyFiles() {
        foreach (string fileName in installFiles) {
            FileInfo currentFile = new FileInfo(fileName);
            currentFile.CopyTo(Path.Combine(installPath,currentFile.Name));
        }
    }


    public static string DefaultInstallPath = "C:\\Program Files\\Defolderizer";


    public static string GetDefaultUserInstallPath() {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "programs", "Defolderizer");
    }

}
