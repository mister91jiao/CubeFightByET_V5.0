//
// Requests.cs
//
// Author:
//       MoMo的奶爸 <xasset@qq.com>
//
// Copyright (c) 2020 MoMo的奶爸
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation bundles (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace libx
{
    public enum LoadState
    {
        Init,
        Loading,
        Loaded,
        Unload,
    }

    public class AssetRequest : Reference, IEnumerator
    {
        public Type assetType;
        public string url;

        private LoadState _loadState = LoadState.Init;
        public LoadState loadState
        {
            get
            {
                return _loadState;
            }
            protected set
            {
                _loadState = value;
                if (value == LoadState.Loaded)
                {
                    Complete();
                }
            }
        }

        public AssetRequest()
        {
            asset = null;
            loadState = LoadState.Init;
        }

        public virtual bool isDone
        {
            get
            {
                return loadState == LoadState.Unload || loadState == LoadState.Loaded;
            }
        }

        public virtual float progress
        {
            get { return 1; }
        }

        public virtual string error { get; protected set; }

        public string text { get; protected set; }

        public byte[] bytes { get; protected set; }

        public Object asset { get; internal set; }

        internal virtual void Load()
        {
            if (!File.Exists(url))
            {
                error = "error! file not exist:" + url;
                loadState = LoadState.Loaded;
                return;
            }

            if (Assets.development && Assets.assetLoader != null)
                asset = Assets.assetLoader(url, assetType);
            if (asset == null)
            {
                error = "error! file not exist:" + url;
            }
            loadState = LoadState.Loaded;
        }

        internal virtual void Unload()
        {
            if (asset == null)
                return;

            if (!Assets.development)
            {
                if (!(asset is GameObject))
                    Resources.UnloadAsset(asset);
            }

            asset = null;
            loadState = LoadState.Unload;
        }

        internal virtual bool Update()
        {
            if (!isDone)
                return true;
            Complete();
            return false;
        }

        private void Complete()
        {
            if (completed != null)
            {
                try
                {
                    completed.Invoke(this);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                completed = null;
            }
        }

        public Action<AssetRequest> completed;

        #region IEnumerator implementation

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }

        #endregion

        public virtual void LoadImmediate()
        {

        }
    }

    public class BundleAssetRequest : AssetRequest
    {
        protected readonly string assetBundleName;
        protected BundleRequest bundle;
        protected List<BundleRequest> children = new List<BundleRequest>();


        public BundleAssetRequest(string bundle)
        {
            assetBundleName = bundle;
        }

        internal override void Load()
        {
            bundle = Assets.LoadBundle(assetBundleName);
            var bundles = Assets.GetChildren(assetBundleName);
            foreach (var item in bundles)
            {
                children.Add(Assets.LoadBundle(item));
            }
            //var assetName = Path.GetFileName(url);
            asset = bundle.assetBundle.LoadAsset(url, assetType);
            loadState = LoadState.Loaded;
        }

        internal override void Unload()
        {
            if (bundle != null)
            {
                bundle.Release();
                bundle = null;
            }

            foreach (var item in children)
            {
                item.Release();
            }

            children.Clear();
            asset = null;
            loadState = LoadState.Unload;
        }
    }

    public class BundleAssetAsyncRequest : BundleAssetRequest
    {
        private AssetBundleRequest _request;

        public BundleAssetAsyncRequest(string bundle)
            : base(bundle)
        {
        }

        public override float progress
        {
            get
            {
                if (isDone)
                {
                    return 1;
                }

                if (loadState == LoadState.Init)
                {
                    return 0;
                }

                if (_request != null)
                {
                    return _request.progress * 0.7f + 0.3f;
                }

                if (bundle == null)
                {
                    return 1;
                }

                var value = bundle.progress;
                var max = children.Count;
                if (max <= 0)
                    return value * 0.3f;

                for (int i = 0; i < max; i++)
                {
                    var item = children[i];
                    value += item.progress;
                }

                return (value / (max + 1)) * 0.3f;
            }
        }

        bool OnError(BundleRequest bundle)
        {
            error = bundle.error;
            if (!string.IsNullOrEmpty(error))
            {
                loadState = LoadState.Loaded;
                return true;
            }
            return false;
        }

        internal override bool Update()
        {
            if (!base.Update())
            {
                return false;
            }

            if (loadState == LoadState.Init)
            {
                return true;
            }

            if (_request == null)
            {
                if (!bundle.isDone)
                {
                    return true;
                }
                if (OnError(bundle))
                {
                    return false;
                }

                for (int i = 0; i < children.Count; i++)
                {
                    var item = children[i];
                    if (!item.isDone)
                    {
                        return true;
                    }
                    if (OnError(item))
                    {
                        return false;
                    }
                }

                //这里是异步加载
                //var assetName = Path.GetFileName(url);
                _request = bundle.assetBundle.LoadAssetAsync(url, assetType);
                if (_request == null)
                {
                    error = "request == null";
                    loadState = LoadState.Loaded;
                    return false;
                }

                return true;
            }

            if (_request.isDone)
            {
                asset = _request.asset;
                loadState = LoadState.Loaded;
                if (asset == null)
                {
                    error = "asset == null";
                }
                return false;
            }
            return true;
        }

        internal override void Load()
        {
            bundle = Assets.LoadBundleAsync(assetBundleName);
            var bundles = Assets.GetChildren(assetBundleName);
            foreach (var item in bundles)
            {
                children.Add(Assets.LoadBundleAsync(item));
            }
            loadState = LoadState.Loading;
        }

        internal override void Unload()
        {
            _request = null;
            loadState = LoadState.Unload;
            base.Unload();
        }

        public override void LoadImmediate()
        {
            bundle.LoadImmediate();
            foreach (var item in children)
            {
                item.LoadImmediate();
            }
            if (bundle.assetBundle != null)
            {
                var assetName = Path.GetFileName(url);
                asset = bundle.assetBundle.LoadAsset(assetName, assetType);
            }
            loadState = LoadState.Loaded;
        }
    }

    public class SceneAssetRequest : AssetRequest
    {
        public readonly LoadSceneMode loadSceneMode;
        protected readonly string sceneName;

        public string assetBundleName { get; set; }

        public List<SceneAssetRequest> additives { get; set; }

        protected BundleRequest bundle;
        protected List<BundleRequest> children = new List<BundleRequest>();

        public SceneAssetRequest(string path, bool addictive)
        {
            url = path;
            additives = new List<SceneAssetRequest>();
            sceneName = Path.GetFileNameWithoutExtension(url);
            loadSceneMode = addictive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public override float progress
        {
            get { return 1; }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = Assets.LoadBundle(assetBundleName);
                if (bundle != null)
                {
                    var bundles = Assets.GetChildren(assetBundleName);
                    foreach (var item in bundles)
                    {
                        children.Add(Assets.LoadBundle(item));
                    }
                    SceneManager.LoadScene(sceneName, loadSceneMode);
                }
            }
            else
            {
                try
                {
                    SceneManager.LoadScene(sceneName, loadSceneMode);
                    loadState = LoadState.Loading;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    error = e.ToString();

                }
            }
            loadState = LoadState.Loaded;
        }

        internal override void Unload()
        {
            if (bundle != null)
                bundle.Release();

            if (children.Count > 0)
            {
                foreach (var item in children)
                {
                    item.Release();
                }
                children.Clear();
            }

            if (additives.Count > 0)
            {
                for (var i = 0; i < additives.Count; i++)
                {
                    var additive = additives[i];
                    if (!additive.IsUnused())
                    {
                        additive.Release();
                    }
                }
                additives.Clear();
            }

            if (loadSceneMode == LoadSceneMode.Additive)
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                    SceneManager.UnloadSceneAsync(sceneName);
            }

            bundle = null;
            loadState = LoadState.Unload;
        }
    }

    public class SceneAssetAsyncRequest : SceneAssetRequest
    {
        private AsyncOperation _request;

        public SceneAssetAsyncRequest(string path, bool addictive)
            : base(path, addictive)
        {
        }

        public override float progress
        {
            get
            {
                if (isDone)
                {
                    return 1;
                }

                if (loadState == LoadState.Init)
                {
                    return 0;
                }

                if (_request != null)
                {
                    return _request.progress * 0.7f + 0.3f;
                }

                if (bundle == null)
                {
                    return 1;
                }

                var value = bundle.progress;
                var max = children.Count;
                if (max <= 0)
                    return value * 0.3f;

                for (int i = 0; i < max; i++)
                {
                    var item = children[i];
                    value += item.progress;
                }

                return (value / (max + 1)) * 0.3f;
            }
        }

        bool OnError(BundleRequest bundle)
        {
            error = bundle.error;
            if (!string.IsNullOrEmpty(error))
            {
                loadState = LoadState.Loaded;
                return true;
            }
            return false;
        }

        internal override bool Update()
        {
            if (!base.Update())
            {
                return false;
            }

            if (loadState == LoadState.Init)
            {
                return true;
            }

            if (_request == null)
            {
                if (bundle == null)
                {
                    error = "bundle == null";
                    loadState = LoadState.Loaded;
                    return false;
                }

                if (!bundle.isDone)
                {
                    return true;
                }

                if (OnError(bundle))
                {
                    return false;
                }

                for (int i = 0; i < children.Count; i++)
                {
                    var item = children[i];
                    if (!item.isDone)
                    {
                        return true;
                    }
                    if (OnError(item))
                    {
                        return false;
                    }
                }

                LoadSceneAsync();

                return true;
            }

            if (_request.isDone)
            {
                loadState = LoadState.Loaded;
                return false;
            }
            return true;
        }

        private void LoadSceneAsync()
        {
            try
            {
                _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                loadState = LoadState.Loading;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                error = e.ToString();
                loadState = LoadState.Loaded;
            }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = Assets.LoadBundleAsync(assetBundleName);
                var bundles = Assets.GetChildren(assetBundleName);
                foreach (var item in bundles)
                {
                    children.Add(Assets.LoadBundleAsync(item));
                }
                loadState = LoadState.Loading;
            }
            else
            {
                LoadSceneAsync();
            }
        }

        internal override void Unload()
        {
            base.Unload();
            _request = null;
        }
    }

    public class WebAssetRequest : AssetRequest
    {
        private UnityWebRequest _www;

        public override float progress
        {
            get
            {
                if (isDone)
                {
                    return 1;
                }
                if (loadState == LoadState.Init)
                {
                    return 0;
                }

                if (_www == null)
                {
                    return 1;
                }

                return _www.downloadProgress;
            }
        }


        internal override bool Update()
        {
            if (!base.Update())
            {
                return false;
            }

            if (loadState == LoadState.Loading)
            {
                if (_www == null)
                {
                    error = "www == null";
                    return false;
                }

                if (!string.IsNullOrEmpty(_www.error))
                {
                    error = _www.error;
                    loadState = LoadState.Loaded;
                    return false;
                }

                if (_www.isDone)
                {
                    GetAsset();
                    loadState = LoadState.Loaded;
                    return false;
                }

                return true;
            }

            return true;
        }

        private void GetAsset()
        {
            if (assetType == typeof(Texture2D))
            {
                asset = DownloadHandlerTexture.GetContent(_www);
            }
            else if (assetType == typeof(AudioClip))
            {
                asset = DownloadHandlerAudioClip.GetContent(_www);
            }
            else if (assetType == typeof(TextAsset))
            {
                text = _www.downloadHandler.text;
            }
            else
            {
                bytes = _www.downloadHandler.data;
            }
        }

        internal override void Load()
        {
            if (assetType == typeof(AudioClip))
            {
                _www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            }
            else if (assetType == typeof(Texture2D))
            {
                _www = UnityWebRequestTexture.GetTexture(url);
            }
            else
            {
                _www = UnityWebRequest.Get(url);
                _www.downloadHandler = new DownloadHandlerBuffer();
            }
            _www.SendWebRequest();
            loadState = LoadState.Loading;
        }

        internal override void Unload()
        {
            if (asset != null)
            {
                Object.Destroy(asset);
                asset = null;
            }

            if (_www != null)
                _www.Dispose();

            bytes = null;
            text = null;
            loadState = LoadState.Unload;
        }
    }

    public class BundleRequest : AssetRequest
    {
        public AssetBundle assetBundle
        {
            get { return asset as AssetBundle; }
            internal set { asset = value; }
        }

        internal override void Load()
        {
            asset = AssetBundle.LoadFromFile(url);
            if (assetBundle == null)
                error = url + " LoadFromFile failed.";
            loadState = LoadState.Loaded;
        }

        internal override void Unload()
        {
            if (assetBundle == null)
                return;
            assetBundle.Unload(true);
            assetBundle = null;
            loadState = LoadState.Unload;
        }
    }

    public class BundleAsyncRequest : BundleRequest
    {
        private AssetBundleCreateRequest _request;

        public override float progress
        {
            get
            {
                if (isDone)
                {
                    return 1;
                }
                if (loadState == LoadState.Init)
                {
                    return 0;
                }

                if (_request == null)
                {
                    return 1;
                }
                return _request.progress;
            }
        }

        internal override bool Update()
        {
            if (!base.Update())
            {
                return false;
            }

            if (loadState == LoadState.Loading)
            {
                if (_request.isDone)
                {
                    assetBundle = _request.assetBundle;
                    if (assetBundle == null)
                    {
                        error = string.Format("unable to load assetBundle:{0}", url);
                    }
                    loadState = LoadState.Loaded;
                    return false;
                }
            }
            return true;
        }

        internal override void Load()
        {
            if (_request == null)
            {
                _request = AssetBundle.LoadFromFileAsync(url);
                if (_request == null)
                {
                    error = url + " LoadFromFile failed.";
                    return;
                }

                loadState = LoadState.Loading;
            }
        }

        internal override void Unload()
        {
            _request = null;
            loadState = LoadState.Unload;
            base.Unload();
        }

        public override void LoadImmediate()
        {
            Load();
            assetBundle = _request.assetBundle;
            if (assetBundle != null)
            {
                Debug.LogWarning("LoadImmediate:" + assetBundle.name);
            }
            loadState = LoadState.Loaded;
        }
    }

    public class WebBundleRequest : BundleRequest
    {
        private UnityWebRequest _request;

        public override float progress
        {
            get
            {
                if (isDone)
                {
                    return 1;
                }
                if (loadState == LoadState.Init)
                {
                    return 0;
                }

                if (_request == null)
                {
                    return 1;
                }

                return _request.downloadProgress;
            }
        }

        internal override bool Update()
        {
            if (!base.Update())
            {
                return false;
            }

            if (loadState == LoadState.Loading)
            {
                if (_request == null)
                {
                    error = "request = null";
                    loadState = LoadState.Loaded;
                    return false;
                }
                if (_request.isDone)
                {
                    assetBundle = DownloadHandlerAssetBundle.GetContent(_request);
                    if (assetBundle == null)
                    {
                        error = "assetBundle = null";
                    }
                    loadState = LoadState.Loaded;
                    return false;
                }
            }
            return true;
        }

        internal override void Load()
        {
            _request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            _request.SendWebRequest();
            loadState = LoadState.Loading;
        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }
            loadState = LoadState.Unload;
            base.Unload();
        }
    }
}