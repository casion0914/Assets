using UnityEngine;
using System.Collections;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public delegate void VoidDelegate(GameObject go);

public class HotFixManager : MonoBehaviour
{
    TweenCallback callback;

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public static HotFixManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = GameObject.Find("MainController");
                _instance = go.GetComponent<HotFixManager>();
            }
            return _instance;
        }
    }

    public static HotFixManager _instance;
    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    public AppDomain appdomain;


    public IEnumerator LoadHotFixAssembly()
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        //appdomain.DebugService.StartDebugService(56000);
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
        WWW www;
        if (LoadAssetBudle.Instance.DebugMode)
            www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
        else
            www = new WWW("file:///" + Application.persistentDataPath + "/HotFix_Project.dll");

        Debug.Log("file://" + Application.persistentDataPath + "/HotFix_Project.dll");

        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        using (System.IO.MemoryStream fs = new MemoryStream(dll))
        {
            appdomain.LoadAssembly(fs, null, new Mono.Cecil.Pdb.PdbReaderProvider());
        }


        InitializeILRuntime();
        OnHotFixLoaded();
    }

    void InitializeILRuntime()
    {
        //迭代器
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

        appdomain.DelegateManager.RegisterMethodDelegate<EventTriggerListener.VoidDelegate>();

        appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();

        appdomain.DelegateManager.RegisterMethodDelegate<DG.Tweening.TweenCallback>();

        //appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        //{
        //    return new DG.Tweening.TweenCallback(() =>
        //    {
        //        ((UnityAction<>)act)();
        //    });
        //});
        //appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback<System.Collections.Generic.Dictionary<byte, object>>>((act) =>
        //{
        //    return new DG.Tweening.TweenCallback<System.Collections.Generic.Dictionary<byte, object>>((a) =>
        //    {
        //        ((System.Action<System.Collections.Generic.Dictionary<byte, object>>)act)(a);
        //    });
        //});


        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        {
            return new DG.Tweening.TweenCallback<float>((a) =>
            {
                ((System.Action<float>)act)(a);
            });
        });

        //appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        //{
        //    return new DG.Tweening.TweenCallback((a) =>
        //    {
        //        ((System.Action<float>)act)(a);
        //    });
        //});


        appdomain.DelegateManager.RegisterDelegateConvertor<EventTriggerListener.VoidDelegate>((act) =>
        {
            return new EventTriggerListener.VoidDelegate((go) =>
            {
                ((System.Action<GameObject>)act)(go);
            });
        });
    }

    void OnHotFixLoaded()
    {
        //appdomain.DelegateManager.RegisterMethodDelegate<EventTriggerListener>();

    }
    public void DoCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public IEnumerator YieldDoCoroutine(IEnumerator coroutine)
    {
        //if (MainController.CommanAsset != null)
        //    MainController.CommanAsset.assetBundle.Unload(true);

        yield return StartCoroutine(coroutine);
    }
}
