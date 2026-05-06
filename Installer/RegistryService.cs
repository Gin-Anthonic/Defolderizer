using Microsoft.Win32;

namespace Defolderizer_Installer;

public class RegistryService {

    private readonly Dictionary<string,string> _registryEdits = new Dictionary<string, string>() {

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
        foreach (KeyValuePair<string, string> registryEdit in _registryEdits) {
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

        foreach (string path in _registryEdits.Keys) {
            if (key.OpenSubKey(path) == null)
                continue;
            key.DeleteSubKeyTree(path);

        }
    }


    public bool HKLMKeysExist() {
        bool result = true;
        RegistryKey? key;

        foreach (string path in _registryEdits.Keys) {
            key = Registry.LocalMachine.OpenSubKey(path);
            if (key == null) result = false;
        }

        return result;
    }


    public bool HKCUKeysExist() {
        bool result = true;
        RegistryKey? key;

        foreach (string path in _registryEdits.Keys) {
            key = Registry.CurrentUser.OpenSubKey(path);
            if (key == null) result = false;
        }

        return result;
    }
}