using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour {
    private static Dictionary<string, LocalizationItem> localizedText;
    private static readonly string missingTextString = "Localized text not found";
    private static readonly string missingFormatString = "{0}";

    public static bool IsReady { get; private set; } = false;

    public static SystemLanguage lang;

    void Awake()
    {
        lang = SaveData.GetEnum<SystemLanguage>("lang", Application.systemLanguage);
        LoadLocalizedText();
    }

    public static void LoadLocalizedText()
    {
        if (IsReady) return;
        LoadAsset();
    }

    public static void LoadAsset()
    {
        TextAsset textAsset = Resources.Load ("Texts/LocalizeData") as TextAsset;
        string jsonString = "{\"items\":" + textAsset.text + "}";
        string jsonStringEscape = jsonString.Replace(System.Environment.NewLine, "\\n");
        CreateDictionary(jsonStringEscape);  
    }

    private static void CreateDictionary(string dataAsJson)
    {
        localizedText = new Dictionary<string, LocalizationItem>();
        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        for (int i = 0; i < loadedData.items.Length; i++)
        {
             localizedText.Add(loadedData.items[i].key, loadedData.items[i]);
        }

        Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");

        IsReady = true;
    }

    public static string GetLocalizedValue(string key)
    {
        string str = "";

#if UNITY_EDITOR
        if (string.IsNullOrEmpty(key)) return missingTextString;
        if (!UnityEditor.EditorApplication.isPlaying
        && localizedText == null) LoadAsset();
#endif
        if (!localizedText.ContainsKey(key)) return missingTextString;
        localizedText.TryGetValue(key, out LocalizationItem result);
        str = result.english;// 英語版のみ
        return str;
    }

    public static string GetLocalizedFormat(string key)
    {
        localizedText.TryGetValue(key, out LocalizationItem result);

        if (string.IsNullOrEmpty(result?.format)) return missingFormatString;
        return result.format;
    }
}
