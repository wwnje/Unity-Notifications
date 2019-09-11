using System;
using UnityEngine;

namespace PuzzlesKingdom.Notifications.Android
{
    public class UnityAndroidHelper : MonoBehaviour
    {
        private static UnityAndroidHelper _instance;

        public static UnityAndroidHelper Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = GameObject.FindObjectOfType<UnityAndroidHelper>();
                    if (null == _instance)
                    {
                        _instance = new GameObject("NotificationsMonoBehaviour").AddComponent<UnityAndroidHelper>();
                    }
                }

                return _instance;
            }
        }

        public void Init()
        {
            
        }

        public bool IsRunningInForeground { get; set; } = true;

        private void Awake()
        {
            IsRunningInForeground = true;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause:" + pauseStatus);
            IsRunningInForeground = !pauseStatus;
        }

        private void OnApplicationQuit()
        {
            IsRunningInForeground = false;
        }
    }
}