using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HRYooba.UI
{
    public class SettingCanvasTabsController : MonoBehaviour
    {
        [SerializeField] Button[] _tabs = null;
        [SerializeField] CanvasGroup[] _windows = null;

        private void Awake()
        {
            for (var i = 0; i < _tabs.Length; i++)
            {
                int tabNum = i;
                _tabs[tabNum].onClick.AddListener(() => { SetActiveWindow(tabNum); });
            }
            SetActiveWindow(0);
        }

        private void OnDestroy()
        {
            foreach (var tab in _tabs)
            {
                tab.onClick.RemoveAllListeners();
            }
        }

        public void SetActiveWindow(int tabNum)
        {
            for (var i = 0; i < _windows.Length; i++)
            {
                _windows[i].alpha = tabNum == i ? 1.0f : 0.0f;
                _windows[i].interactable = tabNum == i ? true : false;
                _windows[i].blocksRaycasts = tabNum == i ? true : false;
            }
        }
    }
}