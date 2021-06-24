using UnityEngine;
using System.IO;

namespace HRYooba.Library
{
    [DefaultExecutionOrder(-11562)]
    public class LocalUnityLogger : MonoBehaviour
    {
        private readonly string NewLine = System.Environment.NewLine;

        [SerializeField] private string _filePrefix = "";
        [SerializeField] private string _localPath = "C:\\unity_log\\";

        private static LocalUnityLogger _instance = null;

        private StreamWriter _sw;
        private string _fileName;
        private string _nowDate;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }

#if !UNITY_EDITOR
        Application.logMessageReceived += LogCallbackHandler;
        Init();
#endif
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
        Application.logMessageReceived -= LogCallbackHandler;

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
        if (!string.Equals(_nowDate, System.DateTime.Now.ToString("yyyy.MM.dd")))
        {
            Init();
        }
#endif
        }

        private void Init()
        {
            _nowDate = System.DateTime.Now.ToString("yyyy.MM.dd");
            _fileName = (_filePrefix == "" ? "" : string.Concat(_filePrefix, "_")) + _nowDate + "_log" + ".txt";

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

        private void LogCallbackHandler(string logString, string stackTrace, LogType type)
        {
            if (_sw == null)
            {
                return;
            }

            var log = string.Format("[{0}][{1}] {2}" + NewLine + "{3}" + NewLine,
                                    type.ToString(),
                                    System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                    logString,
                                    stackTrace);
            _sw.Write(log);
        }
    }
}