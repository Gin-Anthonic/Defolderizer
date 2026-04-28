using Microsoft.Win32;
using System.Windows;

namespace Defolderizer_Installer;

public class RegistryService {

    private readonly Dictionary<string,string> RegistryEdits = new Dictionary<string, string>() {

        {"Software\\Classes\\Directory\\shell\\Defolderize",                        "defolderize" },
        {"Software\\Classes\\Directory\\shell\\Unfold",                             "unfold" },
        {"Software\\Classes\\Directory\\shell\\Defolderize Recursive",              "recursive" },

        {"Software\\Classes\\Directory\\Background\\shell\\Defolderize",            "defolderize" },
        {"Software\\Classes\\Directory\\Background\\shell\\Unfold",                 "unfold" },
        {"Software\\Classes\\Directory\\Background\\shell\\Defolderize Recursive",  "recursive" },
    };


    public void AddRegistryEdits(bool? forAllUsers, string installPath, string position) {
        string keyRoot = "";
        if (forAllUsers == true) {
            keyRoot = "HKEY_LOCAL_MACHINE\\";
        }
        else if (forAllUsers == false) {
            keyRoot = "HKEY_CURRENT_USER\\";
        }
        MessageBox.Show("Adding Regedits at: " + position);
        foreach (KeyValuePair<string, string> registryEdit in RegistryEdits) {
            Registry.SetValue(keyRoot + registryEdit.Key + "\\command", "", $"""{installPath}\defolderizer.exe "%V" "{registryEdit.Value}" """);
            Registry.SetValue(keyRoot + registryEdit.Key, "position", position);
        }
    }


    public void RemoveRegistryEdits(bool? forAllUsers) {
        RegistryKey key = Registry.LocalMachine;
        if (forAllUsers == true) {
            key = Registry.LocalMachine;
        }
        else if (forAllUsers == false) {
            key = Registry.CurrentUser;
        }

        foreach (string path in RegistryEdits.Keys) {
            key.DeleteSubKeyTree(path);

        }
    }


    public bool HKLMKeysExist() {
        bool result = true;
        RegistryKey? key;

        foreach (string path in RegistryEdits.Keys) {
            key = Registry.LocalMachine.OpenSubKey(path);
            if (key == null) result = false;
        }

        return result;
    }


    public bool HKCUKeysExist() {
        bool result = true;
        RegistryKey? key;

        foreach (string path in RegistryEdits.Keys) {
            key = Registry.CurrentUser.OpenSubKey(path);
            if (key == null) result = false;
        }

        return result;
    }


}