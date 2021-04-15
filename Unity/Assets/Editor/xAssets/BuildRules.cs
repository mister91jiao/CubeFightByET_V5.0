//
// BuildRules.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace libx
{
    public enum GroupBy
    {
        None,
        Explicit,
        Filename,
        Directory,
    }

    [Serializable]
    public class AssetBuild
    {
        public string name;
        public string group;
        public string bundle = string.Empty;
        public int id;
        public GroupBy groupBy = GroupBy.Filename;
    }

    [Serializable]
    public class PatchBuild
    {
        public string name;
        public List<string> assets = new List<string>();
    }

    [Serializable]
    public class BundleBuild
    {
        public string assetBundleName;
        public List<string> assetNames = new List<string>();
        public AssetBundleBuild ToBuild()
        {
            return new AssetBundleBuild()
            {
                assetBundleName = assetBundleName,
                assetNames = assetNames.ToArray(),
            };
        }
    }

    public class BuildRules : ScriptableObject
    {
        private readonly List<string> _duplicated = new List<string>();
        private readonly Dictionary<string, HashSet<string>> _tracker = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, string> _asset2Bundles = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _unexplicits = new Dictionary<string, string>();

        [Header("版本号")]
        [Tooltip("构建的版本号")] public int build;
        public int major;
        public int minor;
        [Tooltip("场景文件夹")]
        public string[] scenesFolders = new string[] { "Assets/XAsset" };

        [Header("自动分包分组配置")]
        [Tooltip("是否自动记录资源的分包分组")]
        public bool autoRecord = false;
        [Tooltip("按目录自动分组")]
        public string[] autoGroupByDirectories = new string[0];

        [Header("编辑器提示选项")]
        [Tooltip("检查加载路径大小写是否存在")]
        public bool validateAssetPath;

        [Header("首包内容配置")]
        [Tooltip("是否整包")] public bool allAssetsToBuild;
        [Tooltip("首包包含的分包")] public string[] patchesInBuild = new string[0];
        [Tooltip("BuildPlayer的时候被打包的场景")] public SceneAsset[] scenesInBuild = new SceneAsset[0];

        [Header("AB打包配置")]
        [Tooltip("AB的扩展名")] public string extension = "";
        public bool nameByHash;
        [Tooltip("打包AB的选项")] public BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;

        [Header("缓存数据")]
        [Tooltip("所有要打包的资源")] public List<AssetBuild> assets = new List<AssetBuild>();
        [Tooltip("所有分包")] public List<PatchBuild> patches = new List<PatchBuild>();
        [Tooltip("所有打包的资源")] public List<BundleBuild> bundles = new List<BundleBuild>();

        public string currentScene;

        public void BeginSample()
        {
        }

        public void EndSample()
        {

        }


        public void OnLoadAsset(string assetPath)
        {
            if (autoRecord && Assets.development)
            {
                GroupAsset(assetPath, GetGroup(assetPath));
            }
            else
            {
                if (validateAssetPath)
                {
                    if (assetPath.Contains("Assets"))
                    {
                        if (File.Exists(assetPath))
                        {
                            if (!assets.Exists(asset => asset.name.Equals(assetPath)))
                            {
                                EditorUtility.DisplayDialog("文件大消息不匹配", assetPath, "确定");
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("资源不存在", assetPath, "确定");
                        }
                    }
                }
            }
        }

        private GroupBy GetGroup(string assetPath)
        {
            var groupBy = GroupBy.Filename;
            var dir = Path.GetDirectoryName(assetPath).Replace("\\", "/");
            if (autoGroupByDirectories.Length > 0)
            {
                foreach (var groupWithDir in autoGroupByDirectories)
                {
                    if (groupWithDir.Contains(dir))
                    {
                        groupBy = GroupBy.Directory;
                        break;
                    }
                }
            }

            return groupBy;
        }

        public void OnUnloadAsset(string assetPath)
        {
        }

        #region API

        public AssetBuild GroupAsset(string path, GroupBy groupBy = GroupBy.Filename, string group = null)
        {
            var value = assets.Find(assetBuild => assetBuild.name.Equals(path));
            if (value == null)
            {
                value = new AssetBuild();
                value.name = path;
                assets.Add(value);
            }
            if (groupBy == GroupBy.Explicit)
            {
                value.group = group;
            }
            if (IsScene(path))
            {
                currentScene = Path.GetFileNameWithoutExtension(path);
            }
            value.groupBy = groupBy;
            if (autoRecord && Assets.development)
            {
                PatchAsset(path);
            }
            return value;
        }

        public void PatchAsset(string path)
        {
            var patchName = currentScene;
            var value = patches.Find(patch => patch.name.Equals(patchName));
            if (value == null)
            {
                value = new PatchBuild();
                value.name = patchName;
                patches.Add(value);
            }
            if (File.Exists(path))
            {
                if (!value.assets.Contains(path))
                {
                    value.assets.Add(path);
                }
            }
        }

        public string AddVersion()
        {
            build = build + 1;
            return GetVersion();
        }

        public string GetVersion()
        {
            var ver = new Version(major, minor, build);
            return ver.ToString();
        }

        public void Build()
        {
            Clear();
            CollectAssets();
            AnalysisAssets();
            OptimizeAssets();
            Save();
        }

        public AssetBundleBuild[] GetBuilds()
        {
            return bundles.ConvertAll(delegate (BundleBuild input) { return input.ToBuild(); }).ToArray();
        }

        #endregion

        #region Private

        private string GetGroupName(AssetBuild assetBuild)
        {
            return GetGroupName(assetBuild.groupBy, assetBuild.name, assetBuild.group);
        }

        internal bool ValidateAsset(string asset)
        {
            if (!asset.StartsWith("Assets/")) return false;

            var ext = Path.GetExtension(asset).ToLower();
            return ext != ".dll" && ext != ".cs" && ext != ".meta" && ext != ".js" && ext != ".boo";
        }

        private bool IsScene(string asset)
        {
            return asset.EndsWith(".unity");
        }

        private string GetGroupName(GroupBy groupBy, string asset, string group = null, bool isChildren = false, bool isShared = false)
        {
            if (asset.EndsWith(".shader"))
            {
                group = "shaders";
                groupBy = GroupBy.Explicit;
                isChildren = false;
            }

            else if (IsScene(asset))
            {
                groupBy = GroupBy.Filename;
            }

            switch (groupBy)
            {
                case GroupBy.Explicit:
                    break;
                case GroupBy.Filename:
                    {
                        var assetName = Path.GetFileNameWithoutExtension(asset);
                        var directoryName = Path.GetDirectoryName(asset);
                        var sb = new StringBuilder(assetName + "_");
                        var dir = new DirectoryInfo(directoryName);
                        int max = 1, i = 0;
                        while (i < max)
                        {
                            if (dir == null)
                            {
                                break;
                            }

                            sb.Insert(0, dir.Name + "_");
                            dir = dir.Parent;
                            ++i;
                        }

                        group = sb.ToString();
                    }
                    break;
                case GroupBy.Directory:
                    {
                        var directoryName = Path.GetDirectoryName(asset);
                        var sb = new StringBuilder();
                        var dir = new DirectoryInfo(directoryName);
                        int max = 3, i = 0;
                        while (i < max)
                        {
                            if (dir == null)
                            {
                                break;
                            }

                            sb.Insert(0, dir.Name + "_");
                            dir = dir.Parent;
                            if (dir.FullName == System.Environment.CurrentDirectory)
                            {
                                break;
                            }
                            ++i;
                        }

                        group = sb.ToString();
                        break;
                    }
            }
            if (isChildren)
            {
                return "children_" + group;
            }

            if (isShared)
            {
                group = "shared_" + group;
            }
            return (nameByHash ? Utility.GetMD5Hash(group) : group.TrimEnd('_').ToLower()) + extension;
        }

        private void Track(string asset, string bundle)
        {
            HashSet<string> hashSet;
            if (!_tracker.TryGetValue(asset, out hashSet))
            {
                hashSet = new HashSet<string>();
                _tracker.Add(asset, hashSet);
            }
            hashSet.Add(bundle);
            string bundleName;
            _asset2Bundles.TryGetValue(asset, out bundleName);
            if (string.IsNullOrEmpty(bundleName))
            {
                _unexplicits[asset] = GetGroupName(GroupBy.Explicit, asset, bundle, true);
                if (hashSet.Count > 1)
                {
                    _duplicated.Add(asset);
                }
            }
        }

        private void BundleAsset(string assetName, string assetBundleName)
        {
            if (IsScene(assetName))
            {
                assetBundleName = GetGroupName(GroupAsset(assetName));
            }

            _asset2Bundles[assetName] = assetBundleName;
        }

        private void Clear()
        {
            _unexplicits.Clear();
            _tracker.Clear();
            _duplicated.Clear();
            _asset2Bundles.Clear();
        }

        private Dictionary<string, List<string>> GetBundles()
        {
            var dictionary = new Dictionary<string, List<string>>();
            foreach (var item in _asset2Bundles)
            {
                var bundle = item.Value;
                List<string> list;
                if (!dictionary.TryGetValue(bundle, out list))
                {
                    list = new List<string>();
                    dictionary[bundle] = list;
                }
                if (!list.Contains(item.Key)) list.Add(item.Key);
            }
            return dictionary;
        }

        private void Save()
        {
            bundles.Clear();
            var map = GetBundles();
            foreach (var item in map)
            {
                var bundle = new BundleBuild()
                {
                    assetBundleName = item.Key,
                    assetNames = item.Value,
                };
                bundles.Add(bundle);
            }

            foreach (var patch in patches)
            {
                for (var i = 0; i < patch.assets.Count; ++i)
                {
                    var asset = patch.assets[i];
                    if (!File.Exists(asset))
                    {
                        patch.assets.RemoveAt(i);
                        --i;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private void OptimizeAssets()
        {
            foreach (var item in _unexplicits)
            {
                if (_tracker[item.Key].Count < 2)
                {
                    _asset2Bundles[item.Key] = item.Value;
                }
            }

            for (int i = 0, max = _duplicated.Count; i < max; i++)
            {
                var item = _duplicated[i];
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("优化冗余{0}/{1}", i, max), item,
                    i / (float)max)) break;
                OptimizeAsset(item);
            }
        }

        private void AnalysisAssets()
        {
            var getBundles = GetBundles();
            var i = 0;
            foreach (var item in getBundles)
            {
                var bundle = item.Key;
                var tips = string.Format("分析依赖{0}/{1}", i, getBundles.Count);
                if (EditorUtility.DisplayCancelableProgressBar(tips, bundle, i / (float)getBundles.Count))
                    break;
                var assetPaths = item.Value.ToArray();
                var dependencies = AssetDatabase.GetDependencies(assetPaths, true);
                if (dependencies.Length > 0)
                    foreach (var asset in dependencies)
                    {
                        if (Directory.Exists(asset) || asset.EndsWith(".spriteatlas") || asset.EndsWith(".giparams") || asset.EndsWith("LightingData.asset"))
                        {
                            continue;
                        }
                        if (ValidateAsset(asset))
                        {
                            Track(asset, bundle);
                        }
                    }
                i++;
            }
        }

        private void CollectAssets()
        {
            var list = new List<AssetBuild>();
            var len = Environment.CurrentDirectory.Length + 1;
            for (var index = 0; index < assets.Count; index++)
            {
                var asset = assets[index];
                var path = new FileInfo(asset.name);
                if (path.Exists && ValidateAsset(asset.name))
                {
                    var relativePath = path.FullName.Substring(len).Replace("\\", "/");
                    if (!relativePath.Equals(asset.name))
                    {
                        Debug.LogWarningFormat("检查到路径大小写不匹配！输入：{0}实际：{1}，已经自动修复。", asset.name, relativePath);
                        asset.name = relativePath;
                    }
                    list.Add(asset);
                }
            }

            for (var i = 0; i < list.Count; i++)
            {
                var asset = list[i];
                if (asset.groupBy == GroupBy.None)
                {
                    continue;
                }

                asset.bundle = GetGroupName(asset);
                BundleAsset(asset.name, asset.bundle);
            }

            assets = list;
        }

        private void OptimizeAsset(string asset)
        {
            _asset2Bundles[asset] = GetGroupName(GroupBy.Directory, asset, null, false, true);
        }

        #endregion
    }
}