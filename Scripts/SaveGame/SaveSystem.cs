using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static SaveData saveData;

    //Save Data
    private static void SaveFile(string JsonString)
    {
        File.WriteAllText(Application.persistentDataPath + "/SaveFile1.json",JsonString);
    }
    private static string ChangeToJson(SaveData saveData)
    {
        return JsonUtility.ToJson(saveData,true);
    }

    private static SaveData LoadFile()
    {
        if(File.Exists(Application.persistentDataPath + "/SaveFile1.json"))
        {
            string JsonString = File.ReadAllText(Application.persistentDataPath + "/SaveFile1.json");
            return JsonUtility.FromJson<SaveData>(JsonString);
        }
        return null;
    }

    public static void DeleteFile(int SaveSlot)
    {
        if(File.Exists(Application.persistentDataPath + "/SaveFile1.json"))
        {
            File.Delete(Application.persistentDataPath + "/SaveFile1.json");
            File.Delete(Application.persistentDataPath + "/SaveFile1.json.meta");
        }
    }

    #region Save & Load
    public static void SaveGameData()
    {
        SaveData saveData = new()
        {
            ShuffleBuff = GameData.ShuffleBuff,
            AddTimeBuff = GameData.AddTimeBuff,
            HintBuff = GameData.HintBuff,
            Coins = GameData.Coins,
            Gem = GameData.Gem,
            GameStreak = GameData.GameStreak,
            HighestGameStreak = GameData.HighestGameStreak,
            GameMode = GameData.GameMode,
            SFX = GameData.SFX,
            BGM = GameData.BGM
        };
        string json = ChangeToJson(saveData);
        SaveFile(json);
    }
    public static void LoadGameData()
    {
        if(!File.Exists(Application.persistentDataPath + "/SaveFile1.json")) return;
        SaveData loadData = LoadFile();
        GameData.ShuffleBuff= loadData.ShuffleBuff;
        GameData.AddTimeBuff= loadData.AddTimeBuff;
        GameData.HintBuff= loadData.HintBuff;
        GameData.Coins = loadData.Coins;    
        GameData.Gem = loadData.Gem;
        GameData.GameStreak = loadData.GameStreak;
        GameData.HighestGameStreak = loadData.HighestGameStreak;
        //Debug.Log(GameData.HighestGameStreak);
        GameData.GameMode = loadData.GameMode;
        GameData.SFX = loadData.SFX;
        GameData.BGM = loadData.BGM;
    }
    #endregion

}
