using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public class SaveData
{
    private static SaveDataBase savedatabase = null;

    private static SaveDataBase Savedatabase
    {
        get
        {
            if (savedatabase == null)
            {
                string path = Application.persistentDataPath + "/";
                string fileName = Application.companyName + "." + Application.productName + ".sd";
                savedatabase = new SaveDataBase(path, fileName);
                Debug.Log("path: " + path);
                Debug.Log("filename: " + fileName);
            }
            return savedatabase;
        }
    }

    private SaveData()
    {
    }

    #region Public Static Methods

    public static void SetList<T>(string key, List<T> list)
    {
        Savedatabase.SetList<T>(key, list);
    }

    public static List<T> GetList<T>(string key, List<T> _default)
    {
        return Savedatabase.GetList<T>(key, _default);
    }

    public static T GetClass<T>(string key, T _default) where T : class, new()
    {
        return Savedatabase.GetClass(key, _default);

    }

    public static void SetClass<T>(string key, T obj) where T : class, new()
    {
        Savedatabase.SetClass<T>(key, obj);
    }

    public static TEnum GetEnum<TEnum>(string key, TEnum _default) where TEnum : struct
    {
        return Savedatabase.GetEnum<TEnum>(key, _default);
    }

    public static void SetEnum<TEnum>(string key, TEnum obj) where TEnum : struct
    {
        Savedatabase.SetEnum<TEnum>(key, obj);
    }

    public static void SetString(string key, string value)
    {
        Savedatabase.SetString(key, value);
    }

    public static string GetString(string key, string _default = "")
    {
        return Savedatabase.GetString(key, _default);
    }

    public static void SetInt(string key, int value)
    {
        Savedatabase.SetInt(key, value);
    }

    public static int GetInt(string key, int _default = 0)
    {
        return Savedatabase.GetInt(key, _default);
    }

    public static void SetFloat(string key, float value)
    {
        Savedatabase.SetFloat(key, value);
    }

    public static float GetFloat(string key, float _default = 0.0f)
    {
        return Savedatabase.GetFloat(key, _default);
    }

    public static void Clear()
    {
        Savedatabase.Clear();
    }

    public static void Remove(string key)
    {
        Savedatabase.Remove(key);
    }

    public static bool ContainsKey(string _key)
    {
        return Savedatabase.ContainsKey(_key);
    }

    public static List<string> Keys()
    {
        return Savedatabase.Keys();
    }

    public static void Save()
    {
        Savedatabase.Save();
    }

    #endregion

    #region SaveDatabase Class

    [Serializable]
    private class SaveDataBase
    {
        #region Fields

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private Dictionary<string, string> saveDictionary;

        #endregion

        #region Constructor&Destructor

        public SaveDataBase(string _path, string _fileName)
        {
            path = _path;
            fileName = _fileName;
            saveDictionary = new Dictionary<string, string>();
            Load();

        }

        #endregion

        #region Public Methods

        public void SetList<T>(string key, List<T> list)
        {
            keyCheck(key);
            var serializableList = new Serialization<T>(list);
            string json = JsonUtility.ToJson(serializableList);
            saveDictionary[key] = json;
        }

        public List<T> GetList<T>(string key, List<T> _default)
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
            {
                return _default;
            }
            string json = saveDictionary[key];
            Serialization<T> deserializeList = JsonUtility.FromJson<Serialization<T>>(json);

            return deserializeList.ToList();
        }

        public T GetClass<T>(string key, T _default) where T : class, new()
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
                return _default;

            string json = saveDictionary[key];
            T obj = JsonUtility.FromJson<T>(json);
            return obj;

        }

        public void SetClass<T>(string key, T obj) where T : class, new()
        {
            keyCheck(key);
            string json = JsonUtility.ToJson(obj);
            saveDictionary[key] = json;
        }

        public TEnum GetEnum<TEnum>(string key, TEnum _default) where TEnum : struct
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
                return _default;
            Enum.TryParse<TEnum>(saveDictionary[key], out TEnum ret);// TODO パース失敗時の処理
            return ret;
        }

        public void SetEnum<TEnum>(string key, TEnum obj) where TEnum : struct
        {
            keyCheck(key);
            saveDictionary[key] = obj.ToString();
        }

        public void SetString(string key, string value)
        {
            keyCheck(key);
            saveDictionary[key] = value;
        }

        public string GetString(string key, string _default)
        {
            keyCheck(key);

            if (!saveDictionary.ContainsKey(key))
                return _default;
            return saveDictionary[key];
        }

        public void SetInt(string key, int value)
        {
            keyCheck(key);
            saveDictionary[key] = value.ToString();
        }

        public int GetInt(string key, int _default)
        {
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
                return _default;
            int ret;
            if (!int.TryParse(saveDictionary[key], out ret))
            {
                ret = 0;
            }
            return ret;
        }

        public void SetFloat(string key, float value)
        {
            keyCheck(key);
            saveDictionary[key] = value.ToString();
        }

        public float GetFloat(string key, float _default)
        {
            float ret;
            keyCheck(key);
            if (!saveDictionary.ContainsKey(key))
                ret = _default;

            if (!float.TryParse(saveDictionary[key], out ret))
            {
                ret = 0.0f;
            }
            return ret;
        }

        public void Clear()
        {
            saveDictionary.Clear();

        }

        public void Remove(string key)
        {
            keyCheck(key);
            if (saveDictionary.ContainsKey(key))
            {
                saveDictionary.Remove(key);
            }

        }

        public bool ContainsKey(string _key)
        {

            return saveDictionary.ContainsKey(_key);
        }

        public List<string> Keys()
        {
            return saveDictionary.Keys.ToList<string>();
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(path + fileName, false, Encoding.GetEncoding("utf-8")))
            {
                var serialDict = new Serialization<string, string>(saveDictionary);
                serialDict.OnBeforeSerialize();
                string dictJsonString = JsonUtility.ToJson(serialDict);
                string encrypted = Crypt.Encrypt(dictJsonString);
                writer.WriteLine(encrypted);
            }
        }

        public void Load()
        {
            if (File.Exists(path + fileName))
            {
                using (StreamReader sr = new StreamReader(path + fileName, Encoding.GetEncoding("utf-8")))
                {
                    if (saveDictionary != null)
                    {
                        string str = sr.ReadToEnd();
                        string decrypted = Crypt.Decrypt(str);
                        var sDict = JsonUtility.FromJson<Serialization<string, string>>(decrypted);
                        sDict.OnAfterDeserialize();
                        saveDictionary = sDict.ToDictionary();
                    }
                }
            }
            else { saveDictionary = new Dictionary<string, string>(); }
        }

        public string GetJsonString(string key)
        {
            keyCheck(key);
            if (saveDictionary.ContainsKey(key))
            {
                return saveDictionary[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        private void keyCheck(string _key)
        {
            if (string.IsNullOrEmpty(_key))
            {
                throw new ArgumentException("invalid key!!");
            }
        }

        #endregion
    }

    #endregion

    #region Serialization Class

    [Serializable]
    private class Serialization<T>
    {
        public List<T> target;

        public List<T> ToList()
        {
            return target;
        }

        public Serialization()
        {
        }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }

    [Serializable]
    private class Serialization<TKey, TValue>
    {
        public List<TKey> keys;
        public List<TValue> values;
        private Dictionary<TKey, TValue> target;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return target;
        }

        public Serialization()
        {
        }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            Enumerable.Range(0, count).ToList().ForEach(i => target.Add(keys[i], values[i]));
        }
    }

    #endregion
}