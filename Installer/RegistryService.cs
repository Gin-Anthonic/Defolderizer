using Microsoft.Win32;

namespace Defolderizer_Installer;

public class RegistryService {

    public void AddRegistryEdits(bool? forAllUsers, string installPath, string position = "top") {
        string keyRoot = "";
        string subKey = "";
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


    public void RemoveRegistryEdits() {

        Registry.ClassesRoot.DeleteSubKeyTree("Directory\\Background\\shell\\Defolderize");
        Registry.ClassesRoot.DeleteSubKeyTree("Directory\\Background\\shell\\unfold");
        Registry.ClassesRoot.DeleteSubKeyTree("Directory\\Background\\shell\\Defolderize Recursive");

        Registry.ClassesRoot.DeleteSubKeyTree("Directory\\shell\\Defolderize");
        Registry.ClassesRoot.DeleteSubKeyTree("Directory\\shell\\unfold");
        Registry.ClassesRoot.DeleteSubKeyTree("Directory\\shell\\Defolderize Recursive");

    }

}