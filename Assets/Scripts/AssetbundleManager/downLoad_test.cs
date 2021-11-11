using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class downLoad_test : MonoBehaviour
{
    public Image img;
    // Start is called before the first frame update

    Queue<Action> qq;
    IEnumerator Start()
    {
        qq = new Queue<Action>();
                yield return null;
        //test_ftp();
        //DownloadProgressTracking();
        //StartCoroutine(down_abMain());
        AssetBundleManager.Instance.Load("newbundle", "Image", delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        {
            GameObject obj = rOriginalRes as GameObject;
            GameObject.Instantiate(obj);
            Debug.Log(222);
        });

        //AssetBundleManager.Instance.Load("image", "Image", delegate (UnityEngine.Object rOriginalRes, string rABName, string rResName)
        //{
        //    GameObject obj = rOriginalRes as GameObject;
        //    GameObject.Instantiate(obj, GameObject.Find("Canvas").transform);
        //    Debug.Log(222);
        //});

        //yield return new WaitForSeconds(5);
        //foreach (var db in AssetBundleManager.Instance.get_mAssetBundleInfoDic())
        //{
        //    Debug.Log(db.Value.mABName + "引用" + db.Value.mRefCount);
        //    if (db.Value.mABName == "border")
        //    {
        //        if (db.Value.mAssetBundle != null)
        //        {
        //            Debug.Log("加载了");
        //        }
        //    }
        //}
    }
    void tttt()
    {

        Debug.LogError(1);
    }


    // Update is called once per frame
    void Update()
    {
        lock (qq)
        {
            if (qq.Count > 0)
            {
                qq.Dequeue()();
            }
        }
    }

    void DownloadProgressTracking()
    {
        //HTTPRequest request = new HTTPRequest(new Uri("http://122.51.212.65/download/%E6%97%8B%E8%BD%AC%E7%9F%A9%E9%98%B5.png"), (req, resp) =>
        //{
             
        //    Texture2D t = resp.DataAsTexture2D;
        //    //if (t.Resize(960, 540))
        //    //{
        //    //    t.Apply();
        //    //}
        //    DateTime beforDT = System.DateTime.Now;
        //    Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        //    DateTime afterDT = System.DateTime.Now;
        //    TimeSpan ts = afterDT.Subtract(beforDT);
        //    Debug.Log("DateTime总共花费" + ts.TotalMilliseconds);
        //    RectTransform r = img.GetComponent<RectTransform>();
        //    r.sizeDelta = new Vector2(t.width, t.height);
        //    img.sprite = sprite;


        //});
        //request.OnProgress = OnDownloadProgress;
        //request.Send();
    }

    //void OnDownloadProgress(HTTPRequest request, long downloaded, long length)
    //{
    //    float progressPercent = (downloaded / (float)length) * 100.0f;
    //    Debug.Log("Downloaded: " + progressPercent.ToString("F2") + "%");
    //}

    IEnumerator sfidfd(AssetBundleManifest rManifest)
    {
        string hashcode = rManifest.GetAssetBundleHash("image").ToString();
        int offset = Mathf.Abs(hashcode.GetHashCode() % 10);
        if (offset == 0)
            offset++;
        Debug.Log(offset);
        using (var fileStream = new MyStream(Application.streamingAssetsPath + "/bundles/image", FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false,offset))
        {
            fileStream.Seek(16, SeekOrigin.Begin);
            //yield return null;
            AssetBundleCreateRequest myLoadedAssetBundle = AssetBundle.LoadFromStreamAsync(fileStream,0,(uint)offset);
            yield return myLoadedAssetBundle;
            AssetBundleRequest abgo = myLoadedAssetBundle.assetBundle.LoadAssetAsync<GameObject>("Cube");
            yield return abgo;
            GameObject gog = abgo.asset as GameObject;
            GameObject.Instantiate(gog);




        }
    }

    IEnumerator load_offset(AssetBundleManifest rManifest)
    {
        string hashcode = rManifest.GetAssetBundleHash("image").ToString();
        int offset = Mathf.Abs(hashcode.GetHashCode() % 10);
        if (offset == 0)
            offset++;
        AssetBundleCreateRequest rABCreateRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/bundles/" + "image", 0, (ulong)offset);
        yield return rABCreateRequest;
        AssetBundle ab = rABCreateRequest.assetBundle;
        AssetBundleRequest rABResReq = ab.LoadAssetAsync("Cube");
        yield return rABResReq;

        GameObject go = rABResReq.asset as GameObject;
        GameObject.Instantiate(go);
    }


    IEnumerator down_abMain()
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle("http://122.51.212.65:8998/download/bundles/AssetBundles");

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
                //AssetReqBaseInfo rBaseInfo = new AssetReqBaseInfo(rAllAssetNames[asset_index], rDependencsName, rVersion);
                //mAssetBundleInfoDic.Add(rAllAssetNames[asset_index], rBaseInfo);
            }
            StartCoroutine(sfidfd(rManifest));

        }
        else
        {
            Debug.LogError("Fail load Mainmanifest's  all assets at ");
            yield break;
        }

    }
    IEnumerator load_fromM(byte[] b)
    {
        AssetBundleCreateRequest abCube = AssetBundle.LoadFromMemoryAsync(b);
        yield return abCube;
        AssetBundle ab = abCube.assetBundle;
        AssetBundleRequest rABReq = ab.LoadAllAssetsAsync();
        yield return rABReq;
        AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;

        string[] rAllAssetNames = rManifest.GetAllAssetBundles();
        for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
        {
            string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
            for (int i = 0; i < rDependencsName.Length; i++)
            {
                Debug.LogError(rDependencsName[i]);
            }
        }

        //StartCoroutine(load_offset(rManifest));
    }
    IEnumerator down_save1()
    {
        List<byte> stream = new List<byte>();
        UnityWebRequest request = UnityWebRequest.Get("http://122.51.212.65:8998/download/bundles/image");
        yield return request.SendWebRequest();
        if (request.isDone)
        {
            Debug.Log("cg");
            //StartCoroutine(load_fromM(request.downloadHandler.data)); 

            using (MemoryStream ms = new MemoryStream(request.downloadHandler.data, 0, request.downloadHandler.data.Length))
            {

                AssetBundleCreateRequest abCube = AssetBundle.LoadFromStreamAsync(ms);

                yield return abCube;
                AssetBundle ab = abCube.assetBundle;
                AssetBundleRequest rABReq = ab.LoadAllAssetsAsync();
                yield return rABReq;
                for (int i = 0; i < rABReq.allAssets.Length; i++)
                {
                    GameObject.Instantiate(rABReq.allAssets[i] as GameObject);
                }

                //AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;

                //string[] rAllAssetNames = rManifest.GetAllAssetBundles();
                //for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
                //{
                //    string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
                //    for (int i = 0; i < rDependencsName.Length; i++)
                //    {
                //        Debug.LogError(rDependencsName[i]);
                //    }
                //}

            }
        }
    }

    IEnumerator down_save2()
    {
        List<byte> stream = new List<byte>();
        UnityWebRequest request = UnityWebRequest.Get("http://122.51.212.65:8998/download/bundles/image");
        yield return request.SendWebRequest();
        if (request.isDone)
        {
            Debug.Log("cg");
            //StartCoroutine(load_fromM(request.downloadHandler.data)); 

            

                AssetBundleCreateRequest abCube = AssetBundle.LoadFromMemoryAsync(request.downloadHandler.data);

                yield return abCube;
                AssetBundle ab = abCube.assetBundle;
                AssetBundleRequest rABReq = ab.LoadAllAssetsAsync();
                yield return rABReq;
            for (int i = 0; i < rABReq.allAssets.Length; i++)
            {
                GameObject.Instantiate(rABReq.allAssets[i] as GameObject);
            }
            //AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;

            //string[] rAllAssetNames = rManifest.GetAllAssetBundles();
            //for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
            //{
            //    string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
            //    for (int i = 0; i < rDependencsName.Length; i++)
            //    {
            //        Debug.LogError(rDependencsName[i]);
            //    }
            //}


        }
    }



    void down_save()
    {
        //List<byte> stream = new List<byte>();
        //HTTPRequest request = new HTTPRequest(new Uri("http://122.51.212.65:8998/download/bundles/AssetBundles"), (req, resp) =>
        //{
        //    List<byte[]> fragments = resp.GetStreamedFragments();
        //    if (fragments != null)
        //    {
        //        for (int i = 0; i < fragments.Count; i++)
        //        {
        //            stream.AddRange(fragments[i]);
        //        }
        //    }
        //    if (resp.IsStreamingFinished)
        //    {

        //        Debug.Log("下载完毕");
        //        StartCoroutine(load_fromM(stream.ToArray()));
        //    }


        //});
        //request.UseStreaming = true;
        //request.StreamFragmentSize = 1 * 1024 * 1024;
        ////request.DisableCache = true;
        //request.OnProgress = OnDownloadProgress;
        //request.Send();
    }

    //void test_request()
    //{
    //    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://122.51.212.65:8998/download/bundles/AssetBundles");
    //    RequestState myRequestState = new RequestState();
    //    myRequestState.m_request = myHttpWebRequest;

    //    Debug.Log("BeginGetResponse Start");
    //    //异步获取;
    //    IAsyncResult result = (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);
    //}

    //IEnumerator load_fromStreaming(Stream responseStream)
    //{
    //    AssetBundleCreateRequest abCube = AssetBundle.LoadFromStreamAsync(responseStream);

    //    yield return abCube;
    //    AssetBundle ab = abCube.assetBundle;
    //    AssetBundleRequest rABReq = ab.LoadAllAssetsAsync();
    //    yield return rABReq;
    //    AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;

    //    string[] rAllAssetNames = rManifest.GetAllAssetBundles();
    //    for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
    //    {
    //        string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
    //        for (int i = 0; i < rDependencsName.Length; i++)
    //        {
    //            Debug.LogError(rDependencsName[i]);
    //        }
    //    }
    //}

    //void RespCallback(IAsyncResult result)
    //{
    //    Debug.Log("RespCallback 0");

    //    try
    //    {
    //        RequestState myRequestState = (RequestState)result.AsyncState;
    //        HttpWebRequest myHttpWebRequest = myRequestState.m_request;

    //        Debug.Log("RespCallback EndGetResponse");
    //        myRequestState.m_response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(result);

    //        Stream responseStream = myRequestState.m_response.GetResponseStream();
    //        //responseStream.CanSeek = true;
    //        myRequestState.m_streamResponse = responseStream;
    //        lock (qq)
    //        {
    //            qq.Enqueue(delegate { StartCoroutine(load_fromStreaming(responseStream)); });
    //        }



    //        //开始读取数据;
    //        // IAsyncResult asyncreadresult = responseStream.BeginRead(myRequestState.m_bufferRead, 0, 1024, new AsyncCallback(ReadCallBack), myRequestState);



    //        return;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError(ex.ToString());
    //    }
    //}




    ////void ReadCallBack(IAsyncResult result)
    ////{
    ////    Debug.Log("ReadCallBack");
    ////    try
    ////    {
    ////        RequestState myRequestState = (RequestState)result.AsyncState;
    ////        Stream responseStream = myRequestState.m_streamResponse;
    ////        int read = responseStream.EndRead(result);

    ////        Debug.Log("read size =" + read);

    ////        if (read > 0)
    ////        {
    ////            //将接收的数据写入;
    ////            fileStream.Write(myRequestState.m_bufferRead, 0, 1024);
    ////            fileStream.Flush();
    ////            //fileStream.Close();

    ////            //继续读取数据;
    ////            myRequestState.m_bufferRead = new byte[1024];
    ////            IAsyncResult asyncreadresult = responseStream.BeginRead(myRequestState.m_bufferRead, 0, 1024, new AsyncCallback(ReadCallBack), myRequestState);
    ////        }
    ////    }
    ////    catch (System.Exception ex)
    ////    {
    ////        Debug.LogError(ex.ToString());
    ////    }
    ////}


    //void TimeoutCallback(object state, bool timeout)
    //{
    //    if (timeout)
    //    {
    //        HttpWebRequest request = state as HttpWebRequest;
    //        if (request != null)
    //        {
    //            request.Abort();
    //        }

    //    }
    //}
    void test_wr()
    {
        byte[] buffur;
        using (FileStream fs = new FileStream(Application.streamingAssetsPath + "/test.txt", FileMode.Open, FileAccess.ReadWrite))
        {
            try
            {

                buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);
                //Debug.Log(buffur.Length);


                for (int i = 0; i < buffur.Length; i++)
                {
                    Debug.Log(buffur[i]);
                }
                //fs.
                //fs.Write(buffur)

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        using (FileStream fs = new FileStream(Application.streamingAssetsPath + "/test.txt", FileMode.Create, FileAccess.Write))
        {
            try
            {
                byte[] bb = new byte[buffur.Length + 10];
                buffur.CopyTo(bb, 10);
                //Array.Copy(buffur,bb,)
                for (int i = 0; i < bb.Length; i++)
                {
                    Debug.Log(bb[i]);
                }
                fs.Write(bb, 0, bb.Length);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    IEnumerator tmy()
    {
        WebRequest myrequest = WebRequest.Create("http://122.51.212.65:8998/download/bundles/AssetBundles");
        WebResponse myresponse = myrequest.GetResponse();
        Stream imgstream = myresponse.GetResponseStream();
        BufferedStream bs = new BufferedStream(imgstream);
        AssetBundleCreateRequest abCube = AssetBundle.LoadFromStreamAsync(bs);

        yield return abCube;
        AssetBundle ab = abCube.assetBundle;
        AssetBundleRequest rABReq = ab.LoadAllAssetsAsync();
        yield return rABReq;
        AssetBundleManifest rManifest = rABReq.asset as AssetBundleManifest;

        string[] rAllAssetNames = rManifest.GetAllAssetBundles();
        for (int asset_index = 0; asset_index < rAllAssetNames.Length; asset_index++)
        {
            string[] rDependencsName = rManifest.GetAllDependencies(rAllAssetNames[asset_index]);
            for (int i = 0; i < rDependencsName.Length; i++)
            {
                Debug.LogError(rDependencsName[i]);
            }
        }
    }

    string FTPAddress = "ftp://122.51.212.65:21"; //ftp服务器地址
    string FTPUsername = "ftpuser";   //用户名
    string FTPPwd = "pq3651538";        //密码

    void test_ftp()
    {
        string FtpFilePath = "/upload/json.txt";   //远程路径
        string LocalPath = Application.streamingAssetsPath+"/json.txt"; //下载到的本地路径
        if (File.Exists(LocalPath))
        {
            File.Delete(LocalPath);
        }
        string FTPPath = FTPAddress + FtpFilePath;
        //建立ftp连接
        FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(FTPPath));
        reqFtp.UseBinary = true;
        reqFtp.Credentials = new NetworkCredential(FTPUsername, FTPPwd);
        FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
        Stream ftpStream = response.GetResponseStream();
        long cl = response.ContentLength;
        int buffersize = 2048;
        int readCount;
        byte[] buffer = new byte[buffersize];
        readCount = ftpStream.Read(buffer, 0, buffersize);
        //创建并写入文件
        FileStream OutputStream = new FileStream(LocalPath, FileMode.Create);
        while (readCount > 0)
        {
            OutputStream.Write(buffer, 0, buffersize);
            readCount = ftpStream.Read(buffer, 0, buffersize);
        }
        ftpStream.Close();
        OutputStream.Close();
        response.Close();
        if (File.Exists(LocalPath))
            Console.Write("下载完毕");

    }

}
public class RequestState
{
    const int m_buffetSize = 1024;
    public StringBuilder m_requestData;
    public byte[] m_bufferRead;
    public HttpWebRequest m_request;
    public HttpWebResponse m_response;
    public Stream m_streamResponse;

    public RequestState()
    {
        m_bufferRead = new byte[m_buffetSize];
        m_requestData = new StringBuilder("");
        m_request = null;
        m_streamResponse = null;
    }
}


    
