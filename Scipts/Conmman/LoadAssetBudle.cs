using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadAssetBudle : MonoBehaviour
{
    public static LoadAssetBudle Instance
    {
        get
        {
            if (_instance == null)
            {
                if (GameObject.Find("LoadAssetBudle"))
                {
                    _instance = GameObject.Find("LoadAssetBudle").GetComponent<LoadAssetBudle>();
                    _instance.ProcessBar = GameObject.Find("Canvas/bar").GetComponent<Image>();
                    _instance.Loading = GameObject.Find("Canvas/loading").GetComponent<Image>();
                }
                else
                {
                    GameObject go = new GameObject("LoadAssetBudle");
                    _instance = go.AddComponent<LoadAssetBudle>();
                    _instance.ProcessBar = GameObject.Find("Canvas/bar").GetComponent<Image>();
                    _instance.Loading = GameObject.Find("Canvas/loading").GetComponent<Image>();
                }
            }
            return _instance;
        }
    }

    public string Url
    {
        get
        {
            return url;
        }

        set
        {
            url = value;
        }
    }

    public static LoadAssetBudle _instance;
    public WWW Download;

    public Image ProcessBar;

    public Image Loading;

    public MainController m;

    public bool DebugMode;

    public string url = "http://39.107.93.188/";
    //只能分开写
    //    public IEnumerator LoadCommanAsset()
    //    {
    //        string streamingPath = "";
    //        WWW www;
    //#if UNITY_ANDROID
    //       streamingPath = Application.streamingAssetsPath;
    //#else
    //        streamingPath = "file:///" + Application.streamingAssetsPath;
    //#endif
    //        string main = streamingPath + "/AssetBundle/Comman.unity3d";
    //        //print(main);
    //        www = new WWW(main);

    //        yield return www;
    //        if (www.error == null)
    //        {
    //            //print("xiazai cheng gong");
    //            MainController.Instance.CommanAsset = www.assetBundle;
    //        }
    //    }


    public IEnumerator LoadGameAsset(string path)
    {
        string main = "";

        //加载的资源类型;
        int type = 0;
        WWW www;

        switch (path)
        {
            case "Comman.unity3d":
                type = 1;
                if (DebugMode)
                    main = "file:///" + Application.streamingAssetsPath + "/AssetBundle/" + path;
                else
                    main = "file:///" + LoadAssetBudle.DataPath + "AssetBundle/" + path;
                Debug.Log(main);
                www = new WWW(main);
                yield return www;
                if (www.error == null)
                {
                    Debug.Log("加载 cheng gong");
                    MainController.Instance.CommanAsset = www.assetBundle;
                }
                break;
            default:
                type = 2;
                if (DebugMode)
                    main = "file:///" + Application.streamingAssetsPath + "/AssetBundle/" + path;
                else
                    main = "file:///" + LoadAssetBudle.DataPath + "AssetBundle/" + path;
                print(main);
                www = new WWW(main);
                yield return www;
                if (www.error == null)
                {
                    print("加载 cheng gong");
                    MainController.Instance.GameAsset = www.assetBundle;
                }
                break;
        }
    }
    public void CheckExtractResource()
    {
        if (DebugMode)
        {
            MainController m = GameObject.FindObjectOfType<MainController>();
            StartCoroutine(m.Init());
        }
        else
        {
            bool isExists = Directory.Exists(LoadAssetBudle.DataPath) && Directory.Exists(LoadAssetBudle.DataPath + "AssetBundle/") && File.Exists(LoadAssetBudle.DataPath + "files.txt");
            if (isExists)
            {
                StartCoroutine(OnUpdateResource());
                return;   //文件已经解压过了，自己可添加检查文件列表逻辑
            }
            StartCoroutine(OnExtractResource());    //启动释放协成 
        }
    }

    //先把files拷贝过去
    //再解压资源
    IEnumerator OnExtractResource()
    {
        string dataPath = LoadAssetBudle.DataPath;  //数据目录
        string resPath = LoadAssetBudle.AppContentPath(); //游戏包资源目录

        Debug.Log(resPath);
        //if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
        //Directory.CreateDirectory(dataPath);

        string infile = resPath + "files.txt";//persi
        string outfile = dataPath + "files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);

        string message = "正在解包文件:>files.txt";
        Debug.Log(message);

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            infile = resPath + fs[0];  //
            outfile = dataPath + fs[0];

            message = "正在解包文件:>" + fs[0];
            Debug.Log("正在解包文件:>" + infile);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                Download = new WWW(infile);
                yield return Download;

                if (Download.isDone)
                {
                    File.WriteAllBytes(outfile, Download.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.1f);
        //启动更新
        StartCoroutine(OnUpdateResource());
    }

    //如果跟本地文件不相同
    //则跟服务器更新文件
    IEnumerator OnUpdateResource()
    {
        //判断什么平台的路径
        string target = "";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        target = "Windows/";
#elif UNITY_ANDROID
                 target = "Android/";

#elif UNITY_IOS || UNITY_STANDALONE_OSX
        		 target="IOS/";
#endif
        //服务器路径
        string servicePath = url + target;

        //本地文件路径
        string localPath = "";

        localPath = "file:///" + LoadAssetBudle.DataPath + "files.txt";
        WWW www = new WWW(servicePath + "files.txt");
        Debug.Log(localPath);
        Debug.Log(servicePath);
        yield return www;
        if (www.error == null)
        {
            print(www.text);
        }

        WWW local = new WWW(localPath);

        yield return local;
        if (local.error == null)
        {
            print(local.text);
        }

        if (local.text.Equals(www.text))
        {
            print("yi yang");
        }
        else
        {
            print("bu yiyang");

            //不一样的时候才是从服务器下载新的文件
            string filesText = www.text;
            string[] files = filesText.Split('\n');
            for (int i = 0; i < files.Length; i++)
            {
                if (string.IsNullOrEmpty(files[i])) continue;
                string[] keyValue = files[i].Split('|');
                string f = keyValue[0];
                string localfile = (LoadAssetBudle.DataPath + f).Trim();
                string path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //如果有这个文件就先删除
                if (File.Exists(localfile)) File.Delete(localfile);

                //开始下载
                Download = new WWW(servicePath + f);
                Debug.Log(servicePath + f);
                yield return Download;

                File.WriteAllBytes(localfile, Download.bytes);
            }
        }
        File.Delete(LoadAssetBudle.DataPath + "files.txt");
        File.WriteAllBytes(LoadAssetBudle.DataPath + "files.txt", www.bytes);
        //释放
        www.Dispose();
        local.Dispose();

        yield return new WaitForEndOfFrame();

        Debug.Log("更新完成");

        MainController m = GameObject.FindObjectOfType<MainController>();
        StartCoroutine(m.Init());
        //message = "更新完成!!";
    }
    //加载游戏资源
    public IEnumerator DownLoadGameAsset(string fileName)
    {
        string LocalPath = Application.persistentDataPath + "/AssetBundle/" + fileName;
        //如果文件存在退出
        if (File.Exists(LocalPath)) yield return null;

        //判断什么平台的路径
        string target = "";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        target = "Windows/AssetBundle/";
#elif UNITY_ANDROID
                 target = "Android/AssetBundle/";

#elif UNITY_IOS || UNITY_STANDALONE_OSX
        		 target="IOS/AssetBundle/";
#endif

        string servicePath = url + target + fileName;
        print(servicePath);

        Download = new WWW(servicePath);

        yield return Download;
        if (Download.error == null)
        {
            byte[] data = Download.bytes;
            string dir = LocalPath.Remove(LocalPath.LastIndexOf('/'));
            if (File.Exists(LocalPath)) File.Delete(LocalPath);
            File.WriteAllBytes(LocalPath, data);
            MainController.Instance.GameAsset = Download.assetBundle;
        }
    }
    public void Update()
    {
        if (Download != null)
        {
            ProcessBar.fillAmount = Download.progress;
            Loading.transform.Rotate(Vector3.back * Time.deltaTime * 120, Space.World);

        }
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/";
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return Application.persistentDataPath + "/";
            }
            if (Application.isEditor)
            {
                return Application.persistentDataPath + "/";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + "/";
            }
            return "c:/" + "/";
        }
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.dataPath + "/StreamingAssets/";
                break;
        }
        return path;
    }

    public enum AssetType
    {
        Comman,
        Yaosezi,
    }
}
