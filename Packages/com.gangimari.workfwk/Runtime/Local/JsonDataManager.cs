using UnityEngine;

namespace Fwk.Local
{
    public class JsonDataManager<T> where T : class, new()
    {
        private string _filePath = Application.persistentDataPath + "/" + typeof(T).ToString() + ".json";
        public T Load()
        {
            T data = JsonManager.Load<T>(_filePath);
            if (data == null)
            {
                data = new T();
            }
            return data;
        }
        public void Save(T data)
        {
            JsonManager.Save(data, _filePath);
        }
        public void Delete()
        {
            JsonManager.Delete(_filePath);
        }
        public bool Exists()
        {
            return JsonManager.Exists(_filePath);
        }
    }
}