using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour
{
    public string serverPath
    {
        get {
            return "http://122.51.212.65/download/bundles/";
        }
    }

    public string Path;

     public string mMainManifestPath
    {
        get
        {
            return string.Format("{0}/{1}", Application.streamingAssetsPath, "/bundles/AssetBundles");

        }
    }
    private static AssetBundleManager _instance;
    public static AssetBundleManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private Dictionary<string, AssetReqBaseInfo> mAssetBundleInfoDic = new Dictionary<string, AssetReqBaseInfo>();
    private List<AssetReq> mLoadingAssetReq = new List<AssetReq>();
    private List<string> mDelayReleaseAssets = new List<string>();
    private const int mMaxLoadCount = 5;
    private const int mMaxReleaseCount = 20;
    private const int mReleaseCountPerFrame = 5;
    private WaitForEndOfFrame mWaitFrameEnd = new WaitForEndOfFrame();
    private float mTime = 0;
    private float mTimeInterval = 50;
    bool isready=false;
    private ABGetBase aBGetBase;
    int rVersion=1;
    public delegate void AssetReqCallBack(UnityEngine.Object rOriginalRes, string rABName, string rResName);
    public class AssetReq
    {
        public string mABName;
        public string mResName;
        public AssetReqCallBack mCallBack;
        public bool mDelay;
        public AssetReq(string rABName, string rResName, AssetReqCallBack rCallBack, bool rDelay)
        {
            mABName = rABName;
            mResName = rResName;
            mCallBack = rCallBack;
            mDelay = rDelay;
        }
    }
    public Dictionary<string, AssetReqBaseInfo> get_mAssetBundleInfoDic()
    {
        return mAssetBundleInfoDic;
    }

    public class AssetReqBaseInfo
    {
        public string mABName;
        public AssetBundle mAssetBundle;
        public string[] mDependenceAB;
        public int mRefCount;
        public int mVersion;
        public ABLoadState mABState;
        public string ABHash;
        public Dictionary<string, AssetInfo> mAsset;
        public AssetReqBaseInfo(string rABName, string[] rDepABName, int rVersion,string abhash)
        {
            mABName = rABName;
            mDependenceAB = rDepABName;
            mRefCount = 0;
            ABHash = abhash;
            mAssetBundle = null;
            mVersion = rVersion;
            mAsset = new Dictionary<string, AssetInfo>();
            mABState = ABLoadState.None;
        }
    }
    public class AssetInfo
    {
        public UnityEngine.Object mAsset;
        public AssetLodState mState;
        public AssetInfo(UnityEngine.Object rAsset, AssetLodState rAssetState)
        {
            mAsset = rAsset;
            mState = rAssetState;
        }
    }
    public enum AssetLodState
    {
        LoadFailed,
        Loading,
        Loaded,
    }
    public enum ABLoadState
    {
        None,
        Loading,
        Loaded,
        Release,
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        aBGetBase = new ABGetFromFile(new ABDecodeBase());
        Path = Application.streamingAssetsPath+"/bundles/";
        LoadAssetBaseInfo();
        //StartCoroutine(LoadAssetBaseInfo(1));
        //StartCoroutine(LoadAssetBaseInfo_FromServer(1));
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (!isready)
            return;
        LoopLoadAsset();
        if (mTime > mTimeInterval)
            DelayRelease();
        else
            mTime += Time.deltaTime;
    }
    /// <summary>
    /// release assetbundle which refcount is zero 
    /// </summary>
    /// <param name="rABName"></param>
    /// <param name="rDelay"></param>
    /// <param name="rCallBack"></param>
    public void Release(string rABName, bool rDelay, AssetReqCallBack rCallBack)
    {
        AssetReqBaseInfo rBaseInfo = mAssetBundleInfoDic[rABName];
        rBaseInfo.mRefCount--;
        if (rBaseInfo.mRefCount < 0)
            rBaseInfo.mRefCount = 0;
        Debug.Log(rABName + "引用数为" + rBaseInfo.mRefCount);
        if (rDelay && rBaseInfo.mRefCount == 0 && rBaseInfo.mABState == ABLoadState.Loaded && rBaseInfo.mAssetBundle != null)
        {
            if (!mDelayReleaseAssets.Contains(rABName))
                mDelayReleaseAssets.Add(rABName);
        }
        else if (rBaseInfo.mABState == ABLoadState.Loaded && rBaseInfo.mAssetBundle != null && rBaseInfo.mRefCount == 0)
            UnloadAssetbundle(rABName);

        if (rCallBack != null)
            rCallBack(null, rABName, null);
    }

    public void UnloadAssetbundle_false(string rABName)
    {
        AssetReqBaseInfo rBaseInfo = mAssetBundleInfoDic[rABName];
        rBaseInfo.mAsset.Clear();
        rBaseInfo.mAssetBundle.Unload(false);
        rBaseInfo.mAssetBundle = null;
        rBaseInfo.mABState = ABLoadState.None;
    }

    public void OnSceneChanged()
    {
        foreach (var item in mAssetBundleInfoDic)
        {
            item.Value.mRefCount = 0;
            if (!mDelayReleaseAssets.Contains(item.Value.mABName))
                mDelayReleaseAssets.Add(item.Value.mABName);
            //Release(item.Value.mABName, true, null);
        }
        mTime = 0;
    }

    private void DelayRelease()
    {
        mTime = 0;
        if (mDelayReleaseAssets.Count > 0)
        {
            for (int i = 0; i < mDelayReleaseAssets.Count; i++)
            {
                
                    string rReleaseABName = mDelayReleaseAssets[i];
                    if (mAssetBundleInfoDic[rReleaseABName].mRefCount <= 0)
                    {
                   
                        UnloadAssetbundle(rReleaseABName);
                    }
                
            }
            mDelayReleaseAssets.Clear();
        }
    }
    private void UnloadAssetbundle(string rABName)
    {
        AssetReqBaseInfo rBaseInfo = mAssetBundleInfoDic[rABName];
        
        if (rBaseInfo.mAssetBundle != null)
        {
            Debug.Log("卸载" + rABName);
            rBaseInfo.mAsset.Clear();
            rBaseInfo.mAssetBundle.Unload(true);
            rBaseInfo.mAssetBundle = null;
        }
        rBaseInfo.mABState = ABLoadState.None;
    }

    public void Load(string rABName, string rResName, AssetReqCallBack rCallBack)
    {
        AssetReq rReq = new global::AssetBundleManager.AssetReq(rABName, rResName, rCallBack, false);
        mLoadingAssetReq.Add(rReq);
    }



    private void LoopLoadAsset()
    {
        if (mLoadingAssetReq.Count == 0)
            return;
        for (int req_index = 0; req_index < mLoadingAssetReq.Count; req_index++)
        {
            //Debug.Log("fff");
            StartCoroutine(LoadAssetBundle(mLoadingAssetReq[req_index], null, req_index));
            //StartCoroutine(LoadAssetBundleFromFile_Server(mLoadingAssetReq[req_index], null, req_index));
        }
    }
    private bool CheckABName(string rABName)
    {
        if (mAssetBundleInfoDic.ContainsKey(rABName))
            return true;
        return false;
    }

    UnityWebRequest get_request(string path)
    {
        Debug.LogError("设置头");
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(serverPath + path);
        //request.SetRequestHeader("Access-Control-Allow-Origin", "*");
        //request.SetRequestHeader("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE, HEAD, OPTIONS");
        //request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
        //request.SetRequestHeader("Access-Control-Allow-Headers", "X-Requested-With, origin, content-type, accept");
        return request;
    }
    private IEnumerator LoadAssetBundleFromFile_Server(AssetReq rAssetReq, string rDependABName, int rCurIndex)
    {
        mLoadingAssetReq.Remove(rAssetReq);
        if (rAssetReq == null)
        {
            if (!CheckABName(rDependABName))
            {
                Debug.LogError("not find assetbundle of " + rDependABName);
                yield break;
            }
            AssetReqBaseInfo rAssetReqInfo = mAssetBundleInfoDic[rDependABName];
            if (rAssetReqInfo.mABState == ABLoadState.Loaded && rAssetReqInfo.mAsset != null)
            {
                yield break;
            }
            while (rAssetReqInfo.mABState == ABLoadState.Loading)
            {
                //wait dependency assetbundle load finish;
                yield return null;
            }
            rAssetReqInfo.mABState = ABLoadState.Loading;
            //AssetBundleCreateRequest rABCreateRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/bundles/" + rAssetReqInfo.mABName);
            //yield return rABCreateRequest;
            UnityWebRequest request = get_request(rDependABName);
            yield return request.SendWebRequest();

            AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            //if (!rABCreateRequest.isDone)
            //{
            //    Debug.LogError(rAssetReqInfo.mABName + " assetbundle load faid ");
            //    yield break;
            //}
            rAssetReqInfo.mAssetBundle = ab;
            rAssetReqInfo.mABState = ABLoadState.Loaded;
            // rAssetReqInfo.mRefCount++;
        }
        else
        {
            //load assetbundle and asset
            if (!CheckABName(rAssetReq.mABName))
            {
                Debug.LogError("not find assetbundle of " + rAssetReq.mABName);
                yield break;
            }
            // mLoadingAssetFlag[rCurIndex] = true;
            AssetReqBaseInfo rAssetReqInfo = mAssetBundleInfoDic[rAssetReq.mABName];
            while (rAssetReqInfo.mABState == ABLoadState.Loading)
            {
                //wait dependency assetbundle load finish;
                yield return null;
            }
            if (rAssetReqInfo.mABState == ABLoadState.None)
            {
                //load dependency assetbundle
                rAssetReqInfo.mABState = ABLoadState.Loading;
                mDelayReleaseAssets.Remove(rAssetReq.mABName);
                for (int dep_index = 0; dep_index < rAssetReqInfo.mDependenceAB.Length; dep_index++)
                {
                    yield return StartCoroutine(LoadAssetBundleFromFile_Server(null, rAssetReqInfo.mDependenceAB[dep_index], -1));
                }

                UnityWebRequest request = get_request(rAssetReq.mABName);

                yield return request.SendWebRequest();
                if (!request.isDone)
                {
                    rAssetReqInfo.mABState = ABLoadState.None;
                    Debug.LogError(rAssetReqInfo.mABName + " assetbundle load faid ");
                    yield break;
                }
                else
                {
                    AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                    rAssetReqInfo.mABState = ABLoadState.Loaded;
                    rAssetReqInfo.mAssetBundle = ab;
                }
               
                //AssetBundleCreateRequest rABCreateRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/bundles/" + rAssetReqInfo.mABName);
                //yield return rABCreateRequest;
                //if (!rABCreateRequest.isDone)
                //{
                //    rAssetReqInfo.mABState = ABLoadState.None;
                //    Debug.LogError(rAssetReqInfo.mABName + " assetbundle load faid ");
                //    yield break;
                //}
                //else
                //{
                //    rAssetReqInfo.mABState = ABLoadState.Loaded;
                //    rAssetReqInfo.mAssetBundle = rABCreateRequest.assetBundle;
                //}
            }
            if (rAssetReqInfo.mABState == ABLoadState.Loaded)
            {
                if (!rAssetReqInfo.mAsset.ContainsKey(rAssetReq.mResName))
                {
                    AssetInfo rAssetInfo = new AssetInfo(null, AssetLodState.Loading);
                    rAssetReqInfo.mAsset.Add(rAssetReq.mResName, rAssetInfo);
                    AssetBundleRequest rABResReq = rAssetReqInfo.mAssetBundle.LoadAssetAsync(rAssetReq.mResName);
                    yield return rABResReq;
                    if (rABResReq.isDone)
                    {
                        rAssetReqInfo.mAsset[rAssetReq.mResName].mState = AssetLodState.Loaded;
                        rAssetInfo.mAsset = rABResReq.asset;
                    }
                    else
                    {
                        Debug.LogError("fail load " + rAssetReq.mResName + " from " + rAssetReq.mABName);
                        rAssetInfo.mState = AssetLodState.LoadFailed;
                        yield break;
                    }
                }
                else
                {
                    while (rAssetReqInfo.mAsset[rAssetReq.mResName].mState == AssetLodState.Loading)
                    {
                        yield return null;
                    }
                    if (rAssetReqInfo.mAsset[rAssetReq.mResName].mState == AssetLodState.LoadFailed)
                        yield break;
                }

                if (rAssetReqInfo.mRefCount == 0)
                {
                    for (int dep_index = 0; dep_index < rAssetReqInfo.mDependenceAB.Length; dep_index++)
                    {
                        mAssetBundleInfoDic[rAssetReqInfo.mDependenceAB[dep_index]].mRefCount++;
                    }
                }
                rAssetReq.mCallBack(rAssetReqInfo.mAsset[rAssetReq.mResName].mAsset, rAssetReqInfo.mABName, rAssetReq.mResName);
            }

            rAssetReqInfo.mRefCount++;
        }
    }

    private IEnumerator LoadAssetBundle(AssetReq rAssetReq, string rDependABName, int rCurIndex)
    {
        mLoadingAssetReq.Remove(rAssetReq);
        if (rAssetReq == null)
        {
            if (!CheckABName(rDependABName))
            {
                Debug.LogError("not find assetbundle of " + rDependABName);
                yield break;
            }
            AssetReqBaseInfo rAssetReqInfo = mAssetBundleInfoDic[rDependABName];
            if (rAssetReqInfo.mABState == ABLoadState.Loaded && rAssetReqInfo.mAsset != null)
            {
                yield break;
            }
            while (rAssetReqInfo.mABState == ABLoadState.Loading)
            {
                //wait dependency assetbundle load finish;
                yield return null;
            }
            rAssetReqInfo.mABState = ABLoadState.Loading;
            StartCoroutine(aBGetBase.Get_AssetBundel(rAssetReqInfo.mABName,Path, delegate (AssetBundle a)
             {
                 rAssetReqInfo.mAssetBundle = a;
                 rAssetReqInfo.mABState = ABLoadState.Loaded;
             }
             )
        );
        }
        else
        {
            //load assetbundle and asset
            if (!CheckABName(rAssetReq.mABName))
            {
                Debug.LogError("not find assetbundle of " + rAssetReq.mABName);
                yield break;
            }
           // mLoadingAssetFlag[rCurIndex] = true;
            AssetReqBaseInfo rAssetReqInfo = mAssetBundleInfoDic[rAssetReq.mABName];
            while (rAssetReqInfo.mABState == ABLoadState.Loading)
            {
                //wait dependency assetbundle load finish;
                yield return null;
            }
            if (rAssetReqInfo.mABState == ABLoadState.None)
            {
                //load dependency assetbundle
                rAssetReqInfo.mABState = ABLoadState.Loading;
                mDelayReleaseAssets.Remove(rAssetReq.mABName);
                for (int dep_index = 0; dep_index < rAssetReqInfo.mDependenceAB.Length; dep_index++)
                {
                    yield return StartCoroutine(LoadAssetBundle(null, rAssetReqInfo.mDependenceAB[dep_index], -1));
                }
              yield return  StartCoroutine(aBGetBase.Get_AssetBundel(rAssetReqInfo.mABName,Path, delegate (AssetBundle a)
                {
                    rAssetReqInfo.mABState = ABLoadState.Loaded;
                    rAssetReqInfo.mAssetBundle = a;
                }
          )
     );
            }
            if (rAssetReqInfo.mABState == ABLoadState.Loaded)
            {
                if (!rAssetReqInfo.mAsset.ContainsKey(rAssetReq.mResName))
                {
                    AssetInfo rAssetInfo = new AssetInfo(null, AssetLodState.Loading);
                    rAssetReqInfo.mAsset.Add(rAssetReq.mResName, rAssetInfo);
                    AssetBundleRequest rABResReq = rAssetReqInfo.mAssetBundle.LoadAssetAsync(rAssetReq.mResName);
                    yield return rABResReq;
                    if (rABResReq.isDone)
                    {
                        rAssetReqInfo.mAsset[rAssetReq.mResName].mState = AssetLodState.Loaded;
                        rAssetInfo.mAsset = rABResReq.asset;
                    }
                    else
                    {
                        Debug.LogError("fail load " + rAssetReq.mResName + " from " + rAssetReq.mABName);
                        rAssetInfo.mState = AssetLodState.LoadFailed;
                        yield break;
                    }
                }
                else
                {
                    while (rAssetReqInfo.mAsset[rAssetReq.mResName].mState == AssetLodState.Loading)
                    {
                        yield return null;
                    }
                    if (rAssetReqInfo.mAsset[rAssetReq.mResName].mState == AssetLodState.LoadFailed)
                        yield break;
                }

                if (rAssetReqInfo.mRefCount == 0)
                {
                    for (int dep_index = 0; dep_index < rAssetReqInfo.mDependenceAB.Length; dep_index++)
                    {
                        mAssetBundleInfoDic[rAssetReqInfo.mDependenceAB[dep_index]].mRefCount++;
                    }
                }
                rAssetReq.mCallBack(rAssetReqInfo.mAsset[rAssetReq.mResName].mAsset, rAssetReqInfo.mABName, rAssetReq.mResName);
            }

            rAssetReqInfo.mRefCount++;
        }
    }

    IEnumerator load_MainManifest(AssetBundle a)
    {
        AssetBundleRequest rABReq = a.LoadAllAssetsAsync();
        yield return rABReq;
        if (rABReq.isDone)
        {
            AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;
            string[] rAllAssetNames = rManifest.GetAllAssetBundles();
            for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
            {
                string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
                for (int i = 0; i < rDependencsName.Length; i++)
                {
                    Debug.LogError(rDependencsName[i]);
                }
                AssetReqBaseInfo rBaseInfo = new AssetReqBaseInfo(rAllAssetNames[asset_index], rDependencsName, rVersion, rManifest.GetAssetBundleHash(rAllAssetNames[asset_index]).ToString());
                mAssetBundleInfoDic.Add(rAllAssetNames[asset_index], rBaseInfo);
            }
            isready = true;
        }
    }

    /// <summary>
    /// Load MainManifest File
    /// </summary>
    /// <param name="rVersion"></param>
    /// <returns></returns>
    void LoadAssetBaseInfo()
    {

        StartCoroutine(aBGetBase.Get_AssetBundel("/bundles/bundles", Application.streamingAssetsPath, load_MainManifest));
        //yield return null;
    }

    IEnumerator LoadAssetBaseInfo_FromServer(int rVersion)
    {


        UnityWebRequest request = get_request("AssetBundles");

        yield return request.SendWebRequest();

        AssetBundle abCube = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        AssetBundleRequest rABReq = abCube.LoadAllAssetsAsync();
        yield return rABReq;
        if (rABReq.isDone)
        {
            AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;
            string[] rAllAssetNames = rManifest.GetAllAssetBundles();
            for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
            {
                string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
                for (int i = 0; i < rDependencsName.Length; i++)
                {
                    Debug.LogError(rDependencsName[i]);
                }
                AssetReqBaseInfo rBaseInfo = new AssetReqBaseInfo(rAllAssetNames[asset_index], rDependencsName, rVersion,rManifest.GetAssetBundleHash(rAllAssetNames[asset_index]).ToString());
                mAssetBundleInfoDic.Add(rAllAssetNames[asset_index], rBaseInfo);
            }
            isready = true;
        }
        else
        {
            Debug.LogError("Fail load Mainmanifest's  all assets at ");
            yield break;
        }

    }
}