namespace Fwk.Local
{
    public class JsonDataProvider<T> where T : class, new()
    {
        public T Data => _data;
        private T _data = new();
        private JsonDataManager<T> _dataManager = new JsonDataManager<T>();

        public T Load()
        {
            _data = _dataManager.Load();
            return _data;
        }

        public void Save()
        {
            _dataManager.Save(_data);
        }

        public void DeleteFile()
        {
            _dataManager.Delete();
            _data = new();
        }

        public bool FileExists()
        {
            return _dataManager.Exists();
        }
    }
}