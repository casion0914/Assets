using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{

    public Image Loading;
    public void Start()
    {
        print(Application.persistentDataPath);
    }
    public void Update()
    {
        transform.Rotate(Vector3.back * Time.deltaTime*60, Space.World);
    }

    public void oncomplete()
    {
        print("caon");
    }
}
