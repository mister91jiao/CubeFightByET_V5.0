//
// Downloader.cs
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
using UnityEngine;

namespace libx
{
    public class Downloader
    {
        public static string GetDisplaySpeed(float downloadSpeed)
        {
            if (downloadSpeed >= 1024 * 1024)
            {
                return string.Format("{0:f2}MB/s", downloadSpeed * BYTES_2_MB);
            }
            if (downloadSpeed >= 1024)
            {
                return string.Format("{0:f2}KB/s", downloadSpeed / 1024);
            }
            return string.Format("{0:f2}B/s", downloadSpeed);
        }
        
        public static string GetDisplaySize(long downloadSize)
        {
            if (downloadSize >= 1024 * 1024)
            {
                return string.Format("{0:f2}MB", downloadSize * BYTES_2_MB);
            }
            if (downloadSize >= 1024)
            {
                return string.Format("{0:f2}KB", downloadSize / 1024);
            }
            return string.Format("{0:f2}B", downloadSize);
        }
        
        private const float BYTES_2_MB = 1f / (1024 * 1024);
        
        public int maxDownloads = 3;
        
        private readonly List<Download> _downloads = new List<Download>();
        private readonly List<Download> _progressing = new List<Download>(); 
        private readonly List<Download> _prepared = new List<Download>();
        
        public Action<long, long, float> onUpdate;
        public Action onFinished;

        private int _index;
        private float _startTime;
        private float _lastTime;
        private long _lastSize;
        private bool _paused;
        
        public bool isDone { get; private set; }

        public long size { get; private set; }

        public long position { get; private set; }

        public float speed { get; private set; }

        public List<Download> downloads { get { return _downloads; } }

        private long GetDownloadSize()
        {
            var len = 0L;
            var downloadSize = 0L;
            foreach (var download in _downloads)
            {
                downloadSize += download.position;
                len += download.len;
            } 
            return downloadSize - (len - size);
        }

        private bool _started;
        public float sampleTime = 1f;

        public void Start()
        {
            _index = 0; 
            _lastSize = 0L;
            _startTime = Time.realtimeSinceStartup;
            _lastTime = 0;
            isDone = false;
            _prepared.AddRange(_downloads);
        } 

        public void UnPause()
        {
            if (! _paused)
            {
                return; 
            } 
            foreach (var download in _progressing)
            {
                download.UnPause();  
            }  
            _paused = false;
        }

        public void Pause()
        {
            if (_paused)
            {
                return;
            }  
            foreach (var download in _progressing)
            {
                download.Pause();  
            }   
            _paused = true;
        }

        public void Clear()
        {
            size = 0;
            position = 0; 
            _index = 0;
            _lastTime = 0f;
            _lastSize = 0L;
            _startTime = 0;
            
            foreach (var item in _progressing)
            {
                item.Cancel(true);
            }
            
            _progressing.Clear();
            _prepared.Clear();
            _downloads.Clear(); 
        } 

        public void AddDownload(string url, string filename, string hash, long len)
        {
            var download = new Download
            {
                url = url,
                hash = hash,
                len = len,
                filename = filename
            }; 
            _downloads.Add(download);
            var info = new FileInfo(download.tempPath);
            if (info.Exists)
            {
                size += len - info.Length; 
            }
            else
            {
                size += len; 
            }
        } 

        internal void Update()
        {
            if (_paused || isDone)
                return;

            if (_prepared.Count > 0 && _progressing.Count < maxDownloads)
            {
                for (var i = 0; i < Math.Min(maxDownloads - _progressing.Count, _prepared.Count); ++i)
                {
                    var item = _prepared[i];
                    item.Start();
                    Debug.Log("Start Download:" + item.url);
                    _progressing.Add(item);
                    _prepared.RemoveAt(i);
                    --i;
                }
            }

            for (var i = 0; i < _progressing.Count; ++i)
            {
                var download = _progressing[i];
                if (download.isFinished) 
                { 
                    if (!string.IsNullOrEmpty(download.error))
                    {
                        Debug.LogError(string.Format("Download Error:{0}, {1}", download.url, download.error));
                        download.Retry();
                        Debug.Log("Retry Download：" + download.url);   
                    }
                    else
                    {
                        download.Finish(); 
                        if (download.IsValid())
                        {
                            download.Copy();
                            _progressing.RemoveAt(i);  
                            _index++; 
                            --i;
                            Debug.Log("Finish Download：" + download.url); 
                        }
                        else
                        {
                            Debug.LogError(string.Format("Download Error:{0}, {1}", download.url, download.error));
                            download.Retry();
                            Debug.Log("Retry Download：" + download.url);   
                        } 
                    } 
                } 
            }
            
            if (!isDone && _index == downloads.Count)
            {
                Finish();
            } 

            position = GetDownloadSize(); 
            
            var elapsed = Time.realtimeSinceStartup - _startTime;
            if (elapsed - _lastTime < sampleTime)
                return;
            
            var deltaTime = elapsed - _lastTime; 
            speed = (position - _lastSize) / deltaTime;
            if (onUpdate != null)
            {
                onUpdate(position, size, speed);
            } 
            _lastTime = elapsed;  
            _lastSize = position;
        }

        private void Finish()
        {
            if (onFinished != null)
            {
                onFinished.Invoke();
            }

            isDone = true;
        }
    }
}