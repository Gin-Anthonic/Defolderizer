namespace Defolderizer_Installer.Interfaces;

public interface IRegistryService{

    public void AddRegistryEdits(bool? forAllUsers, string installPath, string position);

    public void RemoveRegistryEdits(bool? forAllUsers);

    public bool HKLMKeysExist();

    public bool HKCUKeysExist();

}
