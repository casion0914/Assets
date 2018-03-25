using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PackageAsset : MonoBehaviour
{
    public static List<string> files = new List<string>();

    [MenuItem("ZIJINGE/Build Windows Resource", false, 100)]
    public static void BuildWindowsResource()
    {
        BuildTarget target = BuildTarget.StandaloneWindows64;

        files.Clear();
        //热更新文件
        string Path = Application.dataPath + "/StreamingAssets/HotFix_Project.dll";
        files.Add(Path);
        BuildCommanResource(target);
        BuildFileIndex();
        BuildYaosezi(target);
    }

    [MenuItem("ZIJINGE/Build IOS Resource", false, 100)]
    public static void BuildIOSResource()
    {
        BuildTarget target = BuildTarget.iOS;

        files.Clear();
        //热更新文件
        string Path = Application.dataPath + "/StreamingAssets/HotFix_Project.dll";
        files.Add(Path);
        BuildCommanResource(target);
        BuildFileIndex();
        BuildYaosezi(target);
    }


    [MenuItem("ZIJINGE/Build Android Resource", false, 100)]
    public static void BuildAndoridResource()
    {
        BuildTarget target = BuildTarget.Android;

        files.Clear();
        //热更新文件
        string Path = Application.dataPath + "/StreamingAssets/HotFix_Project.dll";
        files.Add(Path);
        BuildCommanResource(target);
        BuildFileIndex();
        BuildYaosezi(target);
    }
    static void BuildCommanResource(BuildTarget target)
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/UI/Comman", "*.*", SearchOption.TopDirectoryOnly);

        if (!Directory.Exists(Application.streamingAssetsPath + "/AssetBundle")) Directory.CreateDirectory(Application.streamingAssetsPath + "/AssetBundle");

        string assetPath = Application.dataPath + "/StreamingAssets/AssetBundle/Comman.unity3d";
        BuildPipeline.PushAssetDependencies();

        List<Object> list = new List<Object>();
        for (int i = 0; i < files.Length; i++)
        {
            //files[i]= files[i].Replace('\\', '/');
            files[i] = files[i].TrimStart("D:/ Documents / GameCitys /".ToCharArray());
            Object o = AssetDatabase.LoadMainAssetAtPath(files[i]);
            list.Add(o);

        }

        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;


        if (files.Length > 0)
        {
            BuildPipeline.BuildAssetBundle(null, list.ToArray(), assetPath, options, target);
            PackageAsset.files.Add(assetPath);
        }
    }


    static void BuildYaosezi(BuildTarget target)
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/UI/yaosezi", "*.*", SearchOption.TopDirectoryOnly);
        string assetPath = Application.dataPath + "/StreamingAssets/AssetBundle/Yaoyiyao.unity3d";
        BuildPipeline.PushAssetDependencies();

        List<Object> list = new List<Object>();
        for (int i = 0; i < files.Length; i++)
        {
            //files[i]= files[i].Replace('\\', '/');
            files[i] = files[i].TrimStart("D:/ Documents / GameCitys /".ToCharArray());
            Object o = AssetDatabase.LoadMainAssetAtPath(files[i]);
            list.Add(o);
        }

        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;


        if (files.Length > 0)
        {
            BuildPipeline.BuildAssetBundle(null, list.ToArray(), assetPath, options, target);
            PackageAsset.files.Add(assetPath);
        }
    }

    static void BuildFileIndex()
    {
        string resPath = Application.dataPath + "/StreamingAssets/";
        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        //paths.Clear();
        //files.Clear();
        //Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

            string md5 = md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close(); fs.Close();
    }
    public static string md5file(string file)
    {
        FileStream fs = new FileStream(file, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(fs);
        fs.Close();

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            sb.Append(retVal[i].ToString("x2"));
        }
        return sb.ToString();
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = "zijinge";
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return Application.streamingAssetsPath + "/";
            }
            if (Application.isEditor)
            {
                return Application.streamingAssetsPath + "/";
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + game + "/";
            }
            return "c:/" + game + "/";
        }
    }
}
