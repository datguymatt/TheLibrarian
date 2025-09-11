using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "playerData.json");

    public static void Save(PlayerPersistentData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static PlayerPersistentData Load()
    {
        if (!File.Exists(SavePath)) return new PlayerPersistentData();
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<PlayerPersistentData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}