using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIDModelMapper
{
    public static Dictionary<string, string> uidToModelMap = new Dictionary<string, string>()
        {
          { "04AE370A405980", "CatModel" },
          { "041B200A405980", "duckModel" },
          { "04CC88DD123456", "DogPolyart" },
          { "04867AFA3F5980", "moleModel" },
          { "04B3550A405980", "flowerModel" },
          { "0499C6FA3F5980", "PenguinModel" },
          { "0412D9FA3F5980", "SheepModel" },

          { "056FE37C", "CatModel" },
          { "C9FD5174", "PenguinModel" },
          { "C9712C74", "SheepModel" },
          { "16A9B375", "CatModel" },
          { "A9593774", "PenguinModel" },
          { "16A6F675", "SheepModel" }
    };

    public static string GetModelNameFromUID(string uid)
    {
        // UIDを正規化（大文字・コロンやスペース除去）
        uid = uid.ToUpper().Replace(":", "").Replace(" ", "");

        Debug.Log("Looking up UID: " + uid);

        if (uidToModelMap.TryGetValue(uid, out string modelName))
        {
            Debug.Log("Mapped model: " + modelName);
            return modelName;
        }
        else
        {
            Debug.LogWarning("UID not mapped: " + uid);
            return null;
        }
    }
}