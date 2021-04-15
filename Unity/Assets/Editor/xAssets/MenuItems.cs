//
// AssetsMenuItem.cs
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
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace libx
{
    public static class MenuItems
    {
        [MenuItem("XASSET/Copy Bundles")]
        private static void CopyBundles()
        {
            BuildScript.CopyAssets();
        }

        [MenuItem("XASSET/Build/Bundles")]
        private static void BuildBundles()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildRules();
            BuildScript.BuildAssetBundles();
            watch.Stop();
            Debug.Log("BuildBundles " + watch.ElapsedMilliseconds + " ms.");
        }

        [MenuItem("XASSET/Build/Player")]
        private static void BuildPlayer()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildPlayer();
            watch.Stop();
            Debug.Log("BuildPlayer " + watch.ElapsedMilliseconds + " ms.");
        }

        [MenuItem("XASSET/Build/Rules")]
        private static void BuildRules()
        {
            var watch = new Stopwatch();
            watch.Start();
            BuildScript.BuildRules();
            watch.Stop();
            Debug.Log("BuildRules " + watch.ElapsedMilliseconds + " ms.");
        }

        [MenuItem("XASSET/View/Versions")]
        private static void ViewVersions()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path)) return;
            BuildScript.ViewVersions(path);
        }

        [MenuItem("XASSET/View/Bundles")]
        private static void ViewBundles()
        {
            EditorUtility.OpenWithDefaultApp(Assets.Bundles);
        }

        [MenuItem("XASSET/View/Download")]
        private static void ViewDownload()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }

        [MenuItem("XASSET/View/Temp")]
        private static void ViewTemp()
        {
            EditorUtility.OpenWithDefaultApp(Application.temporaryCachePath);
        }

        [MenuItem("XASSET/View/CRC")]
        private static void GetCRC()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path)) return;

            using (var fs = File.OpenRead(path))
            {
                var crc = Utility.GetCRC32Hash(fs);
                Debug.Log(crc);
            }
        }

        [MenuItem("XASSET/View/MD5")]
        private static void GetMD5()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path)) return;

            using (var fs = File.OpenRead(path))
            {
                var crc = Utility.GetMD5Hash(fs);
                Debug.Log(crc);
            }
        }

        [MenuItem("XASSET/Dump Assets")]
        private static void DumpAssets()
        {
            var path = EditorUtility.SaveFilePanel("DumpAssets", null, "dump", "txt");
            if (string.IsNullOrEmpty(path)) return;
            var s = Assets.DumpAssets();
            File.WriteAllText(path, s);
            EditorUtility.OpenWithDefaultApp(path);
        }

        [MenuItem("XASSET/Take Screenshot")]
        private static void Screenshot()
        {
            var path = EditorUtility.SaveFilePanel("截屏", null, "screenshot_", "png");
            if (string.IsNullOrEmpty(path)) return;

            ScreenCapture.CaptureScreenshot(path);
        }

        [MenuItem("Assets/ToJson")]
        private static void ToJson()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var json = JsonUtility.ToJson(Selection.activeObject);
            File.WriteAllText(path.Replace(".asset", ".json"), json);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Copy Path")]
        private static void CopyPath()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = path;
        }

        [MenuItem("Assets/GroupBy/None")]
        private static void GroupByNone()
        {
            GroupAssets(GroupBy.None);
        }

        [MenuItem("Assets/GroupBy/Filename")]
        private static void GroupByFilename()
        {
            GroupAssets(GroupBy.Filename);
        }

        [MenuItem("Assets/GroupBy/Directory")]
        private static void GroupByDirectory()
        {
            GroupAssets(GroupBy.Directory);
        }

        [MenuItem("Assets/GroupBy/Explicit/shaders")]
        private static void GroupByExplicitShaders()
        {
            GroupAssets(GroupBy.Explicit, "shaders");
        }

        [MenuItem("Assets/PatchBy/CurrentScene")]
        private static void PatchAssets()
        {
            var selection = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var rules = BuildScript.GetBuildRules();
            foreach (var o in selection)
            {
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path) || Directory.Exists(path)) continue;
                rules.PatchAsset(path);
            }

            EditorUtility.SetDirty(rules);
            AssetDatabase.SaveAssets();
        }

        private static void GroupAssets(GroupBy nameBy, string bundle = null)
        {
            var selection = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var rules = BuildScript.GetBuildRules();
            foreach (var o in selection)
            {
                var path = AssetDatabase.GetAssetPath(o);
                if (string.IsNullOrEmpty(path) || Directory.Exists(path)) continue;
                rules.GroupAsset(path, nameBy, bundle);
            }

            EditorUtility.SetDirty(rules);
            AssetDatabase.SaveAssets();
        }
    }
}