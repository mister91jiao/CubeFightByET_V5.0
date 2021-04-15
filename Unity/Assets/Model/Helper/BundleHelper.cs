using System;
using System.IO;
using System.Threading.Tasks;
using libx;
using UnityEngine;

namespace ETModel
{
    public static class BundleHelper
    {
        /// <summary>
        /// 下载AB包
        /// </summary>
        public static ETTask<bool> DownloadBundle()
        {
            ETTaskCompletionSource<bool> tcs = new ETTaskCompletionSource<bool>();

            Log.Debug("初始化成功, 下载最新AB包资源");
            //注册网络检测事件
            NetworkMonitor.Instance.onReachabilityChanged += (reachability) =>
            {
                if (reachability == NetworkReachability.NotReachable)
                {
                    Log.Error("网络错误");
                    tcs.SetResult(false);
                }
            };

            Log.Debug("网络正常");

            Assets.DownloadVersions((DownloadError) =>
            {
                if (!string.IsNullOrEmpty(DownloadError))
                {
                    Log.Error("获取服务器版本失败：" + DownloadError);
                    tcs.SetResult(false);
                }
                else
                {
                    Downloader handler;
                    // 按分包下载版本更新，返回true的时候表示需要下载，false的时候，表示不需要下载
                    if (Assets.DownloadAll(out handler))
                    {
                        long totalSize = handler.size;
                        Debug.Log("发现内容更新，总计需要下载 " + Downloader.GetDisplaySize(totalSize) + " 内容");
                        handler.onUpdate += new Action<long, long, float>((progress, size, speed) =>
                        {
                            //刷新进度
                            Log.Debug("下载中..." +
                                      Downloader.GetDisplaySize(progress) + "/" + Downloader.GetDisplaySize(size) +
                                      ", 速度：" + Downloader.GetDisplaySpeed(speed));
                        });
                        handler.onFinished += new Action(() =>
                        {
                            Log.Debug("下载完成");
                            tcs.SetResult(true);
                        });

                        //开始下载
                        handler.Start();
                    }
                    else
                    {
                        Log.Debug("版本最新无需下载");
                        tcs.SetResult(true);
                    }
                }
            });

            return tcs.Task;
        }
    }
}
