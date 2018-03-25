﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ReflectionDemo : MonoBehaviour
{
    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    AppDomain appdomain;

    void Start()
    {
        StartCoroutine(LoadHotFixAssembly());
    }

    IEnumerator LoadHotFixAssembly()
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
#else
        www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        using (System.IO.MemoryStream fs = new MemoryStream(dll))
        {
            using (System.IO.MemoryStream p = new MemoryStream(pdb))
            {
                appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }

        InitializeILRuntime();
        OnHotFixLoaded();
    }

    void InitializeILRuntime()
    {
        //这里做一些ILRuntime的注册，比如委托的适配器，但是为了演示不些适配器的报错，注册写在了OnHotFixLoaded里

    }

    void OnHotFixLoaded()
    {
        Debug.Log("C#工程中反射是一个非常经常用到功能，ILRuntime也对反射进行了支持，在热更DLL中使用反射跟原生C#没有任何区别，故不做介绍");
        Debug.Log("这个Demo主要是介绍如何在主工程中反射热更DLL中的类型");
        Debug.Log("假设我们要通过反射创建HotFix_Project.InstanceClass的实例");
        Debug.Log("显然我们通过Activator或者Type.GetType(\"HotFix_Project.InstanceClass\")是无法取到类型信息的");
        Debug.Log("热更DLL中的类型我们均需要通过AppDomain取得");
        var it = appdomain.LoadedTypes["HotFix_Project.InstanceClass"];
        Debug.Log("LoadedTypes返回的是IType类型，但是我们需要获得对应的System.Type才能继续使用反射接口");
        var type = it.ReflectionType;
        Debug.Log("取得Type之后就可以按照我们熟悉的方式来反射调用了");
        var ctor = type.GetConstructor(new System.Type[0]);
        var obj = ctor.Invoke(null);
        Debug.Log("打印一下结果");
        Debug.Log(obj);
        Debug.Log("我们试一下用反射给字段赋值");
        var fi = type.GetField("id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fi.SetValue(obj, 111111);
        Debug.Log("我们用反射调用属性检查刚刚的赋值");
        var pi = type.GetProperty("ID");
        Debug.Log("ID = " + pi.GetValue(obj, null));
    }
}
