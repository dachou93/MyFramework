using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TextureUtils
{
    static Dictionary<string, Sprite> pathTextureMap = new Dictionary<string, Sprite>();

    static List<string> downloadList = new List<string>();

    static Dictionary<string, List<Action<Sprite>>> callbackMap = new Dictionary<string, List<Action<Sprite>>>();
    public static Sprite LoadSprite(string file)//从本地加载图片
    {
        if (pathTextureMap.ContainsKey(file)) return pathTextureMap[file];
        string fullPath = GlobalDefine.DataPath + file;
        if (File.Exists(fullPath))
        {
            byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D outImg = new Texture2D(2, 2);
            outImg.LoadImage(bytes);
            
            Sprite sprite = Sprite.Create(outImg, new Rect(0, 0, outImg.width, outImg.height), Vector2.one * 0.5f);
            pathTextureMap.Add(file, sprite);
            return sprite;
        }
        pathTextureMap.Add(file, null);
        return null;
    }
    public static void LoadSpriteSycn(string path, Action<Sprite> textureLoadedCallback)
    {
        if (pathTextureMap.ContainsKey(path)) 
        {
            textureLoadedCallback(pathTextureMap[path]);
        }
        // 没有图幅在下载path路径纹理
       else if (!downloadList.Contains(path))
        {
            downloadList.Add(path);

            Load(path, textureLoadedCallback);
        }
        // 已有图幅在下载path路径纹理，加到下载完成回调队列中
        else
        {
            if (callbackMap.TryGetValue(path, out List<Action<Sprite>> callbacks))
            {
                bool isAdded = false;
                //for (int i = 0; i < callbacks.Count; i++)
                //{
                //    if (textureLoadedCallback.mapTileID == callbacks[i].mapTileID)
                //    {
                //        isAdded = true;
                //        break;
                //    }
                //}

                if (!isAdded)
                {
                    callbacks.Add(textureLoadedCallback);
                }
            }
            else
            {
                callbacks = new List<Action<Sprite>>();
                callbacks.Add(textureLoadedCallback);
                callbackMap.Add(path, callbacks);
            }
        }
    }

    static void Load(string texturePath, Action<Sprite> textureLoadedCallback)
    {
        // 添加纹理下载任务
        DEDownloadUtil.Instance.AddDownloadTask(texturePath, (UnityWebRequest www) =>
        {
            if (www.isNetworkError || www.isHttpError)
            {
                // do nothing
            }
            else
            {
                DEDownloadUtil.Instance.AddTask(() =>
                {
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
                    tex.wrapMode = TextureWrapMode.Clamp;
                    ImageConversion.LoadImage(tex, www.downloadHandler.data, true);
                    Sprite sprite= Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
                    pathTextureMap.Add(texturePath, sprite);
                    downloadList.Remove(texturePath);
                    //Retain(texturePath);

                    textureLoadedCallback?.Invoke(sprite);

                    if (callbackMap.TryGetValue(texturePath, out List<Action<Sprite>> callbacks))
                    {
                        for (int i = 0; i < callbacks.Count; i++)
                        {
                            //if (callbacks[i].mapTileID != textureLoadedCallback.mapTileID)
                            {
                                callbacks[i](sprite);
                            }
                        }
                        callbackMap.Remove(texturePath);
                    }
                });
            }
        });
    }
}

public class DEDownloadUtil : MonoSingleton<DEDownloadUtil> 
{
    int currentDownloadCount;
    // 每帧最多6ms用来创建纹理
    static float MaxCostPerFrame = 6f;
    // 最大纹理资源同时下载数
    static int MaxDownloadCount = 4;
    List<System.Action> downloadTasks = new List<System.Action>();
    List<System.Action> createTasks = new List<System.Action>();
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    void Update() 
    {
        while (downloadTasks.Count > 0 && currentDownloadCount < MaxDownloadCount)
        {
            downloadTasks[0]();
            downloadTasks.RemoveAt(0);
        }
        float totalCost = 0;
        while (createTasks.Count > 0 && totalCost < MaxCostPerFrame)
        {
            stopwatch.Restart();
            createTasks[0]();
            createTasks.RemoveAt(0);
            stopwatch.Stop();

            totalCost += stopwatch.ElapsedMilliseconds;
        }
    }
    public void AddTask(System.Action task)
    {
        createTasks.Add(task);
    }
    public void AddDownloadTask(string texPath, System.Action<UnityWebRequest> callback)
    {
        downloadTasks.Insert(0, () =>
        {
            StartCoroutine(Download(texPath, callback));
        });
    }
    IEnumerator Download(string texturePath, System.Action<UnityWebRequest> callback)
    {
        currentDownloadCount++;

        string texPath = GlobalDefine.DataPath + texturePath;

        UnityWebRequest www = UnityWebRequest.Get(texPath);
        yield return www.SendWebRequest();

        currentDownloadCount--;

        callback(www);
    }
}
