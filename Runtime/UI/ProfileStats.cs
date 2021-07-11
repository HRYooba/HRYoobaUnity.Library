using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace HRYooba.UI
{
    public class ProfileStats : MonoBehaviour
    {
        private const uint SizeMB = 1024 * 1024;
        
        private Text _text;
        private string _osInfo;
        private string _cpuInfo;
        private string _gpuInfo;
        private string _resolutionInfo;
        private string _audioInfo;
        private int _frameCounts;
        private float _prevTime;
        private float _fps;

        private void Start()
        {
            _text = gameObject.GetComponent<Text>();

            _osInfo = string.Format("OS: {0}", SystemInfo.operatingSystem);
            _cpuInfo = string.Format("CPU: {0} / {1}cores", SystemInfo.processorType, SystemInfo.processorCount);
            _gpuInfo = string.Format("GPU: {0} / {1}MB API: {2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize, SystemInfo.graphicsDeviceType);
            _resolutionInfo = string.Format("Resolution: {0} x {1} RefreshRate: {2}Hz", Screen.currentResolution.width, Screen.currentResolution.height, Screen.currentResolution.refreshRate);

            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
            OnAudioConfigurationChanged(true);
        }

        private void OnDestroy()
        {
            AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChanged;
        }

        private void Update()
        {
            CountFPS();
            string memory = string.Format("Memory: {0:####.0} / {1}.0MB GCCount: {2}", Profiler.usedHeapSizeLong / (float)SizeMB, SystemInfo.systemMemorySize, System.GC.CollectionCount(0));
            string performance = string.Format("Performance: {0:#0.#}fps", _fps);
            if (_text != null)
            {
                _text.text = _osInfo + System.Environment.NewLine +
                _cpuInfo + System.Environment.NewLine +
                _gpuInfo + System.Environment.NewLine +
                _resolutionInfo + System.Environment.NewLine +
                _audioInfo + System.Environment.NewLine +
                memory + System.Environment.NewLine +
                performance;
            }
        }

        private void CountFPS()
        {
            ++_frameCounts;
            float time = Time.realtimeSinceStartup - _prevTime;

            if (time > 0.5f)
            {
                _fps = _frameCounts / time;
                _frameCounts = 0;
                _prevTime = Time.realtimeSinceStartup;
            }
        }

        private void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            int bufferLength, numBuffers;
            AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
            AudioConfiguration config = AudioSettings.GetConfiguration();
            _audioInfo = string.Format("Audio: {0:#,#}Hz {1} {2}samples {3}buffers", config.sampleRate, config.speakerMode.ToString(), config.dspBufferSize, numBuffers);
        }
    }
}