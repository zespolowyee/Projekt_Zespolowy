using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]

public static class PersistentGrid<T> where T : class
{
    public static void SaveGridData(T positions, string savePath)
    {
        string json = JsonUtility.ToJson(positions, false);
        File.WriteAllText(savePath, json);
        Debug.Log($"Grid data saved to {savePath}: {json}");
    }

    public static T LoadGridData(string savePath)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            return null;
        }
    }
}