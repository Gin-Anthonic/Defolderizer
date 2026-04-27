using System.IO;
using System.Windows;

namespace Defolderizer_Installer;

public class Installer {

    private string installPath = "";
    private bool? addToRightClick;
    private bool? forAllUsers;
    private string menuPosition = "";
    private MainWindow mainWindow;
    private RegistryService registryService;

    public Installer(MainWindow mainWindow, RegistryService registryService) {
        this.mainWindow = mainWindow;
        this.registryService = registryService;
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
                registryService.AddRegistryEdits(forAllUsers, installPath);
            }
            catch (Exception e) {
                MessageBox.Show("Registry edits failed due to the following error: \n" + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                mainWindow.Print("Registry edits failed due to the following error: \n" + e.Message);
                return;
            }
            mainWindow.Print("Registry Edits Successful!");
        }
    }


    public void AddRegEdits() {
        registryService.AddRegistryEdits(forAllUsers, installPath, menuPosition);
    }


    private void CopyFiles() {
        FileInfo applicationFile = new FileInfo("defolderizer.exe");
        FileInfo configFile = new FileInfo("config.ini");
        applicationFile.CopyTo(Path.Combine(installPath, applicationFile.Name));
        configFile.CopyTo(Path.Combine(installPath, configFile.Name));
    }


}
