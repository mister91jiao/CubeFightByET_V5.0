//
// EditorInitializer.cs
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
using UnityEditor;
using UnityEngine;

namespace libx
{
    public static class EditorInitializer
    {
        
        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize()
        { 
            var sceneAssets = new List<string>();
            var rules = BuildScript.GetBuildRules();

            foreach (var guid in AssetDatabase.FindAssets("t:Scene", rules.scenesFolders))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid); 
                sceneAssets.Add(assetPath);  
            } 
            
            var patches = new List<Patch>();
            var assets = new List<AssetRef>();
            var searchPaths = new List<string>();  
            var dirs = new Dictionary<string, int>();
            foreach (var asset in rules.assets)
            { 
                if (! File.Exists(asset.name))
                {
                    continue;
                }
                var dir = Path.GetDirectoryName(asset.name);
                if (! string.IsNullOrEmpty(dir))
                {
                    if (! searchPaths.Contains(dir))
                    {
                        dirs[dir] = searchPaths.Count;
                        searchPaths.Add(dir);
                    }   
                } 
                var ar = new AssetRef {name = Path.GetFileName(asset.name), bundle = -1, dir = dirs[dir] };  
                assets.Add(ar);
            }  
            
            var scenes = new EditorBuildSettingsScene[sceneAssets.Count];
            for (var index = 0; index < sceneAssets.Count; index++)
            {
                var asset = sceneAssets[index]; 
                scenes[index] = new EditorBuildSettingsScene(asset, true);
                var dir = Path.GetDirectoryName(asset);
                if (! searchPaths.Contains(dir))
                {
                    searchPaths.Add(dir);
                }
            }

            for (var i = 0; i < rules.patches.Count; i++)
            {
                var item = rules.patches[i];
                var patch = new Patch();
                patch.name = item.name;
                patches.Add(patch);
            }
            
            var developVersions = new Versions();
            developVersions.dirs = searchPaths.ToArray();
            developVersions.assets = assets;
            developVersions.patches = patches;
            Assets.platform = BuildScript.GetPlatformName();
            //Assets.basePath = Environment.CurrentDirectory.Replace("\\", "/") + "/" + BuildScript.outputPath + "/";
            Assets.assetLoader = AssetDatabase.LoadAssetAtPath; 
            Assets.versionsLoader += () => developVersions;
            Assets.onAssetLoaded += rules.OnLoadAsset;
            Assets.onAssetUnloaded += rules.OnUnloadAsset;   
            rules.BeginSample();
            EditorBuildSettings.scenes = scenes; 
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged; 
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                Assets.onAssetLoaded = null;
                Assets.onAssetUnloaded = null; 
                var rules = BuildScript.GetBuildRules(); 
                rules.EndSample();
                EditorUtility.SetDirty(rules);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [InitializeOnLoadMethod]
        private static void OnEditorInitialize()
        {
            EditorUtility.ClearProgressBar(); 
        }
    }
}