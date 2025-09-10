using Fwk;

namespace Fwk.Local
{
    public class LocalDataManager<T> : SingletonGeneric<LocalDataManager<T>> where T : class, new()
    {
        public T Data => _provider.Data;
        private JsonDataProvider<T> _provider = new();

        public void Save()
        {
            _provider.Save();
        }

        public void Load()
        {
            _provider.Load();
        }

        public void DeleteFile()
        {
            _provider.DeleteFile();
        }

        public bool FileExists()
        {
            return _provider.FileExists();
        }
    }
}