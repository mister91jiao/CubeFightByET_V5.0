using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace libx
{
    public class Initializer : MonoBehaviour
    {
        public bool splash;
        public bool loggable;
        public VerifyBy verifyBy = VerifyBy.CRC;
        public string downloadURL;
        public bool development;  
        public bool dontDestroyOnLoad = true;
        public string launchScene;
        public string[] searchPaths;
        public string[] patches4Init;
        public bool updateAll;
        private void Start()
        {
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
            EditorInit();  
            Assets.updateAll = updateAll;
            Assets.downloadURL = downloadURL;
            Assets.verifyBy = verifyBy;
            Assets.searchPaths = searchPaths;
            Assets.patches4Init = patches4Init;
            Assets.Initialize(error =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError(error);
                    return;
                }

                if (splash)
                {
                    Assets.LoadSceneAsync(R.GetScene("Splash"));
                }
                else
                {
                    Assets.LoadSceneAsync(R.GetScene(launchScene)); 
                }
            });   
        }  

        [Conditional("UNITY_EDITOR")] 
        private void EditorInit()
        {
            Assets.development = development; 
            Assets.loggable = loggable;
        }

        [Conditional("UNITY_EDITOR")]
        private void Update()
        {
            Assets.loggable = loggable; 
        }
    }
}
