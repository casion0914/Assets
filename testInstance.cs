using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInstance : MonoBehaviour {

    public static testInstance Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new testInstance();
            }
            return _instance;
        }
    }

    public static testInstance _instance;

    public int xx = 5;
}
