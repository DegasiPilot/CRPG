using UnityEngine;
using System.IO;
using CRPG.DataSaveSystem.SaveData;

public static class LocalCashManager
{
    private const string _cashFileName = "UserInfoCash.bin";

    private readonly static char _pathSeparator = Path.DirectorySeparatorChar;
    private readonly static string _localCashPath = Application.persistentDataPath + _pathSeparator + _cashFileName;

    public static void SaveUserCash(string login, string password)
    {
        using (FileStream stream = File.Open(_localCashPath, FileMode.OpenOrCreate))
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(login);
                writer.Write(password);
            }
        }
    }

    public static User LoadUserCash()
    {
        User user = null;
        if (File.Exists(_localCashPath))
        {
            user = new User();
            using (FileStream stream = File.Open(_localCashPath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    user.Login = reader.ReadString();
                    user.Password = reader.ReadString();
                }
            }
        }
        return user;
    }

    public static void CleanCash()
    {
        if (File.Exists(_localCashPath))
        {
            File.Delete(_localCashPath);
        }
    }
}