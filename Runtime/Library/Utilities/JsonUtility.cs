using System.IO;
using Newtonsoft.Json;

namespace HRYooba.Library
{
    public static class JsonUtility
    {
        public static T ConvertStringToJson<T>(string strJson) where T : new()
        {
            var json = JsonConvert.DeserializeObject<T>(strJson);
            return json;
        }

        public static void Save<T>(string path, T obj)
        {
            var dirPath = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var json = JsonConvert.SerializeObject(obj);
            using var fs = File.Create(path);
            var utf8 = new System.Text.UTF8Encoding(false);
            using var sw = new StreamWriter(fs, utf8);
            sw.WriteLine(json);
        }

        public static T Load<T>(string path) where T : new()
        {
            if (!File.Exists(path))
            {
                return new T();
            }

            using var fs = File.Open(path, FileMode.Open);
            using var sr = new StreamReader(fs);
            return ConvertStringToJson<T>(sr.ReadToEnd());
        }
    }
}