using System;
using UnityEngine;

namespace libx
{
    public class NetworkMonitor : MonoBehaviour
    {
        public Action<NetworkReachability> onReachabilityChanged;

        private static NetworkMonitor instance;

        public static NetworkMonitor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("NetworkMonitor").AddComponent<NetworkMonitor>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        public NetworkReachability reachability { get; private set; }

        public float sampleTime = 0.5f;
        private float _time;
        private bool _paused;
        private void Start()
        {
            reachability = Application.internetReachability;
            UnPause();
        }
        public void UnPause()
        {
            _time = Time.timeSinceLevelLoad;
            _paused = false;
        }
        
        public void Pause()
        {
            _paused = true;
        }
        private void Update()
        {
            if (_paused )
            {
                return;
            }

            if (!(Time.timeSinceLevelLoad - _time >= sampleTime)) return;
            
            var state = Application.internetReachability;
            
            if (reachability != state)
            {
                if (onReachabilityChanged != null)
                {
                    onReachabilityChanged(state);
                }
                reachability = state;
            } 
            _time = Time.timeSinceLevelLoad;
        }
    }
}