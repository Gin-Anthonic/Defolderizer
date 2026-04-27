using Microsoft.Win32;
using System.Windows;

namespace Defolderizer_Installer;

public class RegistryService {



    public void AddRegistryEdits(bool? forAllUsers, string installPath, string position = "top") {
        string keyRoot = "";
        string subKey = "";
        if (forAllUsers == true) {
            keyRoot = "HKEY_LOCAL_MACHINE";
            subKey = "Software\\Classes\\Directory";
        }
        else if (forAllUsers == false) {
            keyRoot = "HKEY_CURRENT_USER";
            subKey = "Software\\Classes\\Directory";
        }
        MessageBox.Show("Adding Regedits at: " + keyRoot);
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


    public void RemoveRegistryEdits(bool? forAllUsers) {
        RegistryKey key = Registry.LocalMachine;
        if (forAllUsers == true) {
            key = Registry.LocalMachine;
        }
        else if (forAllUsers == false) {
            key = Registry.CurrentUser;
        }

        key.DeleteSubKeyTree("Software\\Classes\\Directory\\Background\\shell\\Defolderize");
        key.DeleteSubKeyTree("Software\\Classes\\Directory\\Background\\shell\\Unfold");
        key.DeleteSubKeyTree("Software\\Classes\\Directory\\Background\\shell\\Defolderize Recursive");

        key.DeleteSubKeyTree("Software\\Classes\\Directory\\shell\\Defolderize");
        key.DeleteSubKeyTree("Software\\Classes\\Directory\\shell\\Unfold");
        key.DeleteSubKeyTree("Software\\Classes\\Directory\\shell\\Defolderize Recursive");

    }


    public bool HKLMKeysExist() {
        bool result = true;
        RegistryKey? key;
        string iconPath = "Software\\Classes\\Directory\\shell\\";
        string backgroundPath = "Software\\Classes\\Directory\\Background\\shell\\";

        key = Registry.LocalMachine.OpenSubKey(iconPath + "Defolderize");
        if (key == null) result = false;
        key = Registry.LocalMachine.OpenSubKey(iconPath + "Unfold");
        if (key == null) result = false;
        key = Registry.LocalMachine.OpenSubKey(iconPath + "Defolderize Recursive");
        if (key == null) result = false;

        key = Registry.LocalMachine.OpenSubKey(backgroundPath + "Defolderize");
        if (key == null) result = false;
        key = Registry.LocalMachine.OpenSubKey(backgroundPath + "Unfold");
        if (key == null) result = false;
        key = Registry.LocalMachine.OpenSubKey(backgroundPath + "Defolderize Recursive");
        if (key == null) result = false;

        return result;
    }

    public bool HKCUKeysExist() {
        bool result = true;
        RegistryKey? key;
        string iconPath = "Software\\Classes\\Directory\\shell\\";
        string backgroundPath = "Software\\Classes\\Directory\\Background\\shell\\";

        key = Registry.CurrentUser.OpenSubKey(iconPath + "Defolderize");
        if (key == null) result = false;
        key = Registry.CurrentUser.OpenSubKey(iconPath + "Unfold");
        if (key == null) result = false;
        key = Registry.CurrentUser.OpenSubKey(iconPath + "Defolderize Recursive");
        if (key == null) result = false;

        key = Registry.CurrentUser.OpenSubKey(backgroundPath + "Defolderize");
        if (key == null) result = false;
        key = Registry.CurrentUser.OpenSubKey(backgroundPath + "Unfold");
        if (key == null) result = false;
        key = Registry.CurrentUser.OpenSubKey(backgroundPath + "Defolderize Recursive");
        if (key == null) result = false;

        return result;
    }


}