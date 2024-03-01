using UnityEngine;
using System.IO;

namespace HRYooba.Library
{
    public static class CustomLog
    {
        public static event System.Action<string> OnLogMessageReceived;
        public static void WriteLog(string message)
        {
            OnLogMessageReceived?.Invoke(message);
        }
    }

    [DefaultExecutionOrder(-11560)]
    public class LocalCustomLogger : MonoBehaviour
    {
        private readonly string NewLine = System.Environment.NewLine;

        [SerializeField] private string _localPath = "C:\\log\\";

        private static LocalCustomLogger _instance = null;

        private StreamWriter _sw;
        private string _fileName;
        private string _nowDate;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }

#if !UNITY_EDITOR
        CustomLog.OnLogMessageReceived += LogCallbackHandler;
        Init();
#endif
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
        CustomLog.OnLogMessageReceived -= LogCallbackHandler;
        StopAllCoroutines();
        if (_sw != null)
        {
            _sw.Write("----------------------------------------------------------------------------------------------------------------------------------------------" + NewLine);
            _sw.Close();
            _sw = null;
        }
#endif
        }

        private void Update()
        {
#if !UNITY_EDITOR
        if (!string.Equals(_nowDate, System.DateTime.Now.ToString("yyyyMMdd")))
        {
            Init();
        }
#endif
        }

        private void Init()
        {
            _nowDate = System.DateTime.Now.ToString("yyyyMMdd");
            _fileName = _nowDate + ".txt";

            if (!Directory.Exists(_localPath))
            {
                Directory.CreateDirectory(_localPath);
            }

            if (!File.Exists(_localPath + _fileName))
            {
                var fs = File.Create(_localPath + _fileName);
                fs.Close();
            }

            _sw = new StreamWriter(_localPath + _fileName, true);
            _sw.AutoFlush = true;
        }

        private void LogCallbackHandler(string logString)
        {
            if (_sw == null)
            {
                return;
            }

            var log = string.Format("[{0}] {1}" + NewLine,
                                    System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                    logString);
            _sw.Write(log);
        }
    }
}