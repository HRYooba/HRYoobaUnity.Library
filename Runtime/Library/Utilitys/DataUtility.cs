using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace HRYooba.Library
{
    public static class DataUtility
    {
        public static T LoadDataFromString<T>(string strJson) where T : new()
        {
            var json = JsonConvert.DeserializeObject<T>(strJson);
            return json;
        }

        public static void SaveDataToJson<T>(T obj, string savePath)
        {
            string dirPath = Path.GetDirectoryName(savePath);

            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string json = JsonConvert.SerializeObject(obj);
            FileStream fs = File.Create(savePath);
            var utf8_encoding = new System.Text.UTF8Encoding(false);
            StreamWriter sw = new StreamWriter(fs, utf8_encoding);
            sw.WriteLine(json);
            sw.Close();
            fs.Close();
        }

        public static T LoadDataFromJson<T>(string loadPath) where T : new()
        {
            if (!File.Exists(loadPath))
            {
                return new T();
            }

            FileStream fs = File.Open(loadPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            var json = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            sr.Close();
            fs.Close();
            return json;
        }

        public static string ReadFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            string result = null;

            try
            {
                using (StreamReader sr = new StreamReader(fi.OpenRead(), System.Text.Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            return result;
        }

        public static void SaveDataToCSV(List<Vector2> list, string savePath)
        {
            string dirPath = Path.GetDirectoryName(savePath);

            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            FileStream fs = File.Create(savePath);
            var utf8_encoding = new System.Text.UTF8Encoding(false);
            StreamWriter sw = new StreamWriter(fs, utf8_encoding);
            foreach (var obj in list)
            {
                sw.WriteLine(obj.ToString());
            }
            sw.Flush();
            sw.Close();
        }
    }
}