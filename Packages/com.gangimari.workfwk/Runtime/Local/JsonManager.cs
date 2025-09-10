using UnityEngine;
using System.IO;

namespace Fwk.Local
{
    public static class JsonManager
    {
        public static bool Exists(string path)
        {
            if (File.Exists(path)) return true;
            return false;
        }
        public static void Save<T>(T data, string path)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
            Debug.Log($"Data saved to {path}");
        }

        public static T Load<T>(string path) where T : new()
        {
            if (Exists(path))
            {
                string json = File.ReadAllText(path);
                T data = JsonUtility.FromJson<T>(json);
                Debug.Log($"Data loaded from {path}");
                return data;
            }
            else
            {
                Debug.LogWarning($"File at {path} not found. Returning new instance of {typeof(T).Name}.");
                return new T();
            }
        }

        public static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"File at {path} deleted.");
            }
            else
            {
                Debug.LogWarning($"File at {path} not found.");
            }
        }
    }
}
