using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetsManager
{
#if !UNITY_EDITOR
   static string AssetBundlePostfix = ".ab";
#endif
    static string DataPath = System.Environment.CurrentDirectory + "/RawData";

    // assetPath: 相对于Assets的路径,需要带后缀
    public static T Load<T>(string assetPath, Lifecycle lifecycle = Lifecycle.Current_Scene) where T : Object
    {
        return RM_AssetBundle.Load<T>(assetPath, lifecycle);
    }

    public static T LoadFromAB<T>(string assetPath,string prefabName, Lifecycle lifecycle = Lifecycle.Current_Scene) where T : Object
    {
        return RM_AssetBundle.LoadAB<T>(assetPath, prefabName, lifecycle);
    }
    public static void LoadAsync<T>(string assetPath, System.Action<T> onLoaded, Lifecycle lifecycle = Lifecycle.Current_Scene) where T : Object
    {
        CorutineInstance.Instance.StartCoroutine(RM_AssetBundle.LoadAsync<T>(assetPath, onLoaded, lifecycle));
    }

    public static void LoadAllAsync(string assetPath, Action onLoaded, Lifecycle lifecycle = Lifecycle.Current_Scene)
    {
        CorutineInstance.Instance.StartCoroutine(RM_AssetBundle.LoadAllAsync(assetPath, onLoaded, lifecycle));
    }

    // GameObjec实例化
    public static T Instantiate<T>(T original, bool isDontDestroyOnLoad = false) where T : Object
    {
        return RM_Object.Instantiate<T>(original, isDontDestroyOnLoad);
    }

    // GameObjec销毁
    public static void Destroy<T>(T obj) where T : Object
    {
        RM_Object.Destroy(obj);
    }

    // 用于场景切换时回收即将被Unity销毁的GameObject
    public static void Recycle()
    {
        RM_Object.Recycle();
        ObjectPool.Instance.Recycle();
    }

    public static string ReadTextFromRawData(string path)
    {
        path = Path.Combine(DataPath, path);

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        return null;
    }

    public static List<KeyValuePair<string, string>> ReadAllTextFromRawData(string dirPath, string postfix = "*")
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(DataPath, dirPath));
        FileInfo[] fileInfos = fileInfos = directoryInfo.GetFiles(postfix);

        List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
        foreach (var fileInfo in fileInfos)
        {
            ret.Add(new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(fileInfo.Name), File.ReadAllText(fileInfo.FullName)));
        }

        return ret;
    }

    public static void WriteTextToRawData(string path, string text)
    {
        path = Path.Combine(GlobalDefine.DataPath, path);
        
        if (!File.Exists(path))
        {
            FileStream fileStream = File.Create(path);
            fileStream.Close();
        }

        File.WriteAllText(path, text);
    }

    // 内存管理，当内存超过阀值时，释放对象池中生命周期为zero的对象
    public static void Clear()
    {

    }

    // 显示当前资源状态
    public static string Dump()
    {
        return "[测试版]dump：" +
            "\n 分配出去的GameObject个数: " + RM_Object.objAssetIDDic.Count +
            "\n " + ObjectPool.Instance.Dump();
    }

    static class RM_AssetBundle
    {
        public static T Load<T>(string assetPath, Lifecycle lifecycle) where T : Object
        {
            assetPath = assetPath.Replace('\\', '/');
            T asset = ObjectPool.Instance.Peek<T>(assetPath, lifecycle);

            if (asset != null)
            {
                return asset;
            }
            else
            {
#if UNITY_EDITOR
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(Path.Combine("Assets", assetPath));
#else
                string abPath = Path.Combine(System.Environment.CurrentDirectory, 
                    "AssetBundles/" + assetPath.Substring(0, assetPath.LastIndexOf('/')).ToLower() + AssetBundlePostfix);
                AssetBundle ab = ObjectPool.Instance.Peek<AssetBundle>(abPath, Lifecycle.Current_Scene);

                if (!ab)
                {
                    ab = AssetBundle.LoadFromFile(abPath);
                   
                    ObjectPool.Instance.Push(abPath, ab, Lifecycle.Current_Scene, false);

                    // 加载依赖ab
                    LoadAllDependenciesAssetBundles(ab.name);
                }

                asset = ab.LoadAsset<T>(assetPath.Substring(assetPath.LastIndexOf('/') + 1));
#endif

                Debug.Assert(asset, assetPath);
                ObjectPool.Instance.Push(assetPath, asset, lifecycle, false);

                return asset as T;
            }
        }

        public static T LoadAB<T>(string assetPath, string prefabName, Lifecycle lifecycle) where T : Object
        {
            assetPath = assetPath.Replace('\\', '/');
            T asset = ObjectPool.Instance.Peek<T>(assetPath, lifecycle);

            if (asset != null)
            {
                return asset;
            }
            else
            {

                //string abPath = Path.Combine(System.Environment.CurrentDirectory, 
                //    "AssetBundles/" + assetPath.Substring(0, assetPath.LastIndexOf('/')).ToLower() + AssetBundlePostfix);
                string abPath = Path.Combine(System.Environment.CurrentDirectory, "AssetBundles/" + assetPath);

                AssetBundle ab = ObjectPool.Instance.Peek<AssetBundle>(abPath, Lifecycle.Current_Scene);

                if (!ab)
                {
                    ab = AssetBundle.LoadFromFile(abPath);
                   
                    ObjectPool.Instance.Push(abPath, ab, Lifecycle.Current_Scene, false);

                    // 加载依赖ab
                   // LoadAllDependenciesAssetBundles(ab.name);
                }
                string[]names=  ab.GetAllAssetNames();
                for (int i = 0; i < names.Length; i++) 
                {
                  Debug.Log(names[i]);
                }

                //asset = ab.LoadAsset<T>(assetPath.Substring(assetPath.LastIndexOf('/') + 1));
                //asset = ab.LoadAsset<T>("assets/ab_resourses/prefab/t96a/fbx/t96a.fbx");
                //asset = ab.LoadAsset<T>("assets/ab_resourses/prefab/df-16/df-16r.fbx");
                asset = ab.LoadAsset<T>(prefabName);
                Debug.Assert(asset, assetPath);
                ObjectPool.Instance.Push(assetPath, asset, lifecycle, false);

                return asset as T;
            }
        }
        public static IEnumerator LoadAsync<T>(string assetPath, System.Action<T> onLoaded, Lifecycle lifecycle) where T : Object
        {
            T asset = ObjectPool.Instance.Peek<T>(assetPath, lifecycle);

            if (asset != null)
            {
                onLoaded(asset);

                yield break;
            }
            else
            {
#if UNITY_EDITOR
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(Path.Combine("Assets", assetPath));
#else
                string abPath = Path.Combine(System.Environment.CurrentDirectory,
                    "AssetBundles/" + assetPath.Substring(0, assetPath.LastIndexOf('/')).ToLower() + AssetBundlePostfix);
                AssetBundle ab = ObjectPool.Instance.Peek<AssetBundle>(abPath, Lifecycle.Current_Scene);

                if (!ab)
                {
                    AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(abPath);
                    yield return assetBundleCreateRequest;

                    ab = assetBundleCreateRequest.assetBundle;
                    ObjectPool.Instance.Push(abPath, ab, Lifecycle.Current_Scene, false);

                    // 加载依赖ab
                    yield return CorutineInstance.Instance.StartCoroutine(LoadAllDependenciesAssetBundlesAsync(ab.name));
                }

                AssetBundleRequest assetBundleRequest = ab.LoadAssetAsync<T>(assetPath.Substring(assetPath.LastIndexOf('/') + 1));
                yield return assetBundleRequest;

                asset = assetBundleRequest.asset as T;
#endif
                Debug.Assert(asset, assetPath);
                ObjectPool.Instance.Push(assetPath, asset, lifecycle, false);

                onLoaded(asset);
            }
        }

        public static IEnumerator LoadAllAsync(string assetPath, Action onLoaded, Lifecycle lifecycle, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(DataPath, assetPath));
            foreach (var item in info.GetFiles("*", searchOption))
            {
                string path = Path.Combine(assetPath, item.Name);
                string extension = Path.GetExtension(path).ToLower();
                switch (extension)
                {
                    case ".jpg":
                    case ".png":
                        LoadAsync<Texture>(path, (v) => { }, lifecycle);
                        break;
                    case ".prefab":
                        LoadAsync<GameObject>(path, (v) => { }, lifecycle);
                        break;
                    case ".material":
                        LoadAsync<Material>(path, (v) => { }, lifecycle);
                        break;
                    default:
                        Debug.LogError("当前尚未设置 " + extension + " 类型的加载方式");
                        break;
                }
            }

            onLoaded.Invoke();

            yield return null;
        }

        static void LoadAllDependenciesAssetBundles(string assetBundleName)
        {
            string manifestAssetBundlePath = GetAssetBundleManifestPath();
            AssetBundle manifestAssetBundle = ObjectPool.Instance.Peek<AssetBundle>(manifestAssetBundlePath, Lifecycle.Current_Scene);

            if (!manifestAssetBundle)
            {
                manifestAssetBundle = AssetBundle.LoadFromFile(manifestAssetBundlePath);
                ObjectPool.Instance.Push(manifestAssetBundlePath, manifestAssetBundle, Lifecycle.Current_Scene, false);
            }

            AssetBundleManifest manifest = (AssetBundleManifest)manifestAssetBundle.LoadAsset("AssetBundleManifest");
            string[] dependencies = manifest.GetAllDependencies(assetBundleName);

            foreach (var depAssetBundleName in dependencies)
            {
                string assetBundlePath = Path.Combine(System.Environment.CurrentDirectory, "AssetBundles/" + depAssetBundleName);
                AssetBundle assetBundle = ObjectPool.Instance.Peek<AssetBundle>(assetBundlePath, Lifecycle.Current_Scene);

                if (!assetBundle)
                {
                    assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                    ObjectPool.Instance.Push(assetBundlePath, assetBundle, Lifecycle.Current_Scene, false);
                }
            }
        }

        static IEnumerator LoadAllDependenciesAssetBundlesAsync(string assetBundleName)
        {
            string manifestAssetBundlePath = GetAssetBundleManifestPath();
            AssetBundle manifestAssetBundle = ObjectPool.Instance.Peek<AssetBundle>(manifestAssetBundlePath, Lifecycle.Current_Scene);

            if (!manifestAssetBundle)
            {
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(manifestAssetBundlePath);
                yield return createRequest;

                manifestAssetBundle = createRequest.assetBundle;
                ObjectPool.Instance.Push(manifestAssetBundlePath, manifestAssetBundle, Lifecycle.Current_Scene, false);
            }

            AssetBundleRequest assetBundleRequest = manifestAssetBundle.LoadAssetAsync("AssetBundleManifest");
            yield return assetBundleRequest;

            AssetBundleManifest manifest = (AssetBundleManifest)assetBundleRequest.asset;
            string[] dependencies = manifest.GetAllDependencies(assetBundleName);

            foreach (var depAssetBundleName in dependencies)
            {
                string assetBundlePath = Path.Combine(System.Environment.CurrentDirectory, "AssetBundles/" + depAssetBundleName);
                AssetBundle assetBundle = ObjectPool.Instance.Peek<AssetBundle>(assetBundlePath, Lifecycle.Current_Scene);

                if (!assetBundle)
                {
                    AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
                    yield return createRequest;

                    ObjectPool.Instance.Push(assetBundlePath, createRequest.assetBundle, Lifecycle.Current_Scene, false);
                }
            }
        }

        static string GetAssetBundleManifestPath()
        {
            return Path.Combine(System.Environment.CurrentDirectory, "AssetBundles/AssetBundles");
        }
    }

    static class RM_Object
    {
        // Instantiate的GameObject与资产ID字典
        public static Dictionary<Object, string> objAssetIDDic = new Dictionary<Object, string>();

        public static T Instantiate<T>(T original, bool isDontDestroyOnLoad) where T : Object
        {
            string id = original.GetHashCode().ToString();

            if (!isDontDestroyOnLoad)
            {
                T obj = ObjectPool.Instance.Pop<T>(id);

                if (!obj)
                {
                    obj = Object.Instantiate<T>(original);
                }

                objAssetIDDic.Add(obj, id);

                return obj;
            }
            // 全局存在的GameObject，设置为DontDestroyOnLoad，不进行回收处理
            else
            {
                T obj = Object.Instantiate<T>(original);
                Object.DontDestroyOnLoad(obj);

                return obj;
            }
        }

        public static void Destroy<T>(T obj) where T : Object
        {
            ObjectPool.Instance.Push<T>(objAssetIDDic[obj], obj, Lifecycle.Zero, true);

            objAssetIDDic.Remove(obj);
        }

        // 把实例化的GameObject全部回收
        public static void Recycle()
        {
            foreach (var obj in objAssetIDDic.Keys)
            {
                ObjectPool.Instance.Push(objAssetIDDic[obj], obj, Lifecycle.Zero, true);
            }

            objAssetIDDic.Clear();
        }
    }
}
