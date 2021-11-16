using System;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class SceneMgr
{
    // 场景加载的最快速度，每秒加载的进度值，进度值为1时场景加载完毕
    const float SceneLoadSpeed = 10;

    static float currentProgress;

    static IScene currentScene;

    // 进度条
    static Image progressImg;

    static TextMeshProUGUI progressText;

    /// <summary>
    /// 利用反射初始化当前场景
    /// </summary>
    public static void InitScene()
    {
        string currSceneName = SceneManager.GetActiveScene().name;

        Type[] types = AppDomain.CurrentDomain.GetAssemblies()      // 获取当前接口的所有实现类
                    .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IScene))))
                    .ToArray();

        foreach (var v in types)
        {
            IScene scene = Activator.CreateInstance(v) as IScene;

            if (scene.GetSceneName() == currSceneName)
            {
                SceneMgr.LoadAssetsWithoutLoading(scene, () =>
                {
                    scene.OnEnter();

                    // 通知控制器进入场景事件
                    MessageMgr.Instance.SendMsg("ENTER_SCENE", scene.GetSceneName());
                });

                break;
            }
        }
    }

    /// <summary>
    /// 无Loading的场景加载(同步加载)
    /// </summary>
    /// <param name="scene"></param>
    public static void LoadSceneWithoutLoading(IScene scene)
    {
        if (currentScene != null)
        {
            currentScene.OnExit();
        }

        currentScene = scene;

        // 场景切换时回收即将被Unity销毁的GameObject
        AssetsManager.Recycle();

        SceneManager.LoadScene(scene.GetSceneName());

        UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
        onSceneLoaded = (Scene, LoadSceneMode) =>
        {
            if (Scene.name == scene.GetSceneName())
            {
                scene.OnEnter();

                // 通知控制器进入场景事件
                MessageMgr.Instance.SendMsg("ENTER_SCENE", scene.GetSceneName());
            }
            SceneManager.sceneLoaded -= onSceneLoaded;
        };

        SceneManager.sceneLoaded += onSceneLoaded;
    }

    public static void LoadScene(IScene scene)
    {
        if (currentScene != null)
        {
            currentScene.OnExit();
        }

        currentScene = scene;

        // 场景切换时回收即将被Unity销毁的GameObject
        AssetsManager.Recycle();
        //ECSUtility.Instance.EntitiesDispose();

        // 加载过渡场景
        SceneManager.LoadScene("Loading");

        UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
        onSceneLoaded = (Scene, LoadSceneMode) =>
        {
            if (Scene.name == "Loading")
            {
                progressImg = GameObject.Find("TopCanvas/Panel/Progress").GetComponent<Image>();
                progressText = GameObject.Find("TopCanvas/Panel/ProgressText").GetComponent<TextMeshProUGUI>();

                LoadAssets(scene, () => 
                {
                    CorutineInstance.Instance.StartCoroutine(LoadSceneAsync(scene));
                });

                // 加载目标场景
                SceneManager.sceneLoaded -= onSceneLoaded;
            }
        };
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    static IEnumerator LoadSceneAsync(IScene scene)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.GetSceneName());
        asyncOperation.allowSceneActivation = false;

        // 目标场景加载完后的回调
        asyncOperation.completed += (o) =>
        {
            scene.OnEnter();

            // 通知控制器进入场景事件
            MessageMgr.Instance.SendMsg("ENTER_SCENE", scene.GetSceneName());
        };

        while (!asyncOperation.isDone)
        {
            currentProgress += SceneLoadSpeed * Time.deltaTime;

            if (asyncOperation.progress < 0.9f) // 未加载完成
            {
                currentProgress = currentProgress < asyncOperation.progress ? currentProgress : asyncOperation.progress;
            }
            else // 加载完成
            {
                currentProgress = currentProgress < 1f ? currentProgress : 1f;
            }

            UpdateProgress("场景加载进度: " + System.Convert.ToInt32(currentProgress * 100), currentProgress);

            if (currentProgress >= 1f) // 加载完成，进入下个场景
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    static void UpdateProgress(string text, float progress)
    {
        progressImg.fillAmount = progress;
        progressText.text = text;
    }

    static void LoadAssets(IScene scene, Action callBack)
    {
        string[] preloadAssetsPaths = scene.GetPreloadAssetsPath();

        // 预加载场景资源
        if (preloadAssetsPaths != null)
        {
            int count = 0;
            int len = preloadAssetsPaths.Length;

            for (int i = 0; i < len; i++)
            {
                //int index = i;
                var path = preloadAssetsPaths[i];

                string extension = Path.GetExtension(path).ToLower();

                //switch (extension)
                //{
                //    case ".jpg":
                //    case ".png":
                //        AssetsManager.LoadAsync<Texture>(path, OnLoad);
                //        break;
                //    case ".prefab":
                //        AssetsManager.LoadAsync<GameObject>(path, OnLoad);
                //        break;
                //    case ".material":
                //        AssetsManager.LoadAsync<Material>(path, OnLoad);
                //        break;
                //    case "":
                //        AssetsManager.LoadAllAsync(path, () => { OnLoad(new Object()); });
                //        break;
                //    default:
                //        Debug.LogError("当前尚未设置 " + extension + " 类型的加载方式");
                //        break;
                //}

                //void OnLoad<T>(T obj) where T : Object
                //{
                //    UpdateProgress(Path.GetFileName(path), (float)++count / len);

                //    if (++loadCount == len)
                //    {
                //        callBack();
                //    }
                //}

                // 需要重写，实现一个资源加载接口，支持加载多个资产，分析资产所属的ab包，然后按次序加载ab与资产
                // 上面代码异步加载资源，会导致同时加载相同的依赖包，导致报错
                switch (extension)
                {
                    case ".jpg":
                    case ".png":
                        AssetsManager.Load<Texture>(path);
                        break;
                    case ".prefab":
                        AssetsManager.Load<GameObject>(path);
                        break;
                    case ".material":
                        AssetsManager.Load<Material>(path);
                        break;
                    default:
                        Debug.LogError("当前尚未设置 " + extension + " 类型的加载方式");
                        break;
                }

                UpdateProgress(path, (float)i / (len - 1));
            }

            callBack();
        }
        else
        {
            callBack();
        }
    }

    static void LoadAssetsWithoutLoading(IScene scene, Action callBack)
    {
        string[] preloadAssetsPaths = scene.GetPreloadAssetsPath();

        // 预加载场景资源
        if (preloadAssetsPaths != null)
        {
            int len = preloadAssetsPaths.Length;

            for (int i = 0; i < len; i++)
            {
                var path = preloadAssetsPaths[i];

                string extension = Path.GetExtension(path).ToLower();

                switch (extension)
                {
                    case ".jpg":
                    case ".png":
                        AssetsManager.Load<Texture>(path);
                        break;
                    case ".prefab":
                        AssetsManager.Load<GameObject>(path);
                        break;
                    case ".material":
                        AssetsManager.Load<Material>(path);
                        break;
                    default:
                        Debug.LogError("当前尚未设置 " + extension + " 类型的加载方式");
                        break;
                }
            }

            callBack();
        }
        else
        {
            callBack();
        }
    }
}
