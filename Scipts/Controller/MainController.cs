using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cn.sharesdk.unity3d;

public class MainController : MonoBehaviour
{
    public static MainController Instance
    {
        get
        {
            if (GameObject.Find("MainController"))
            {
                _instance = GameObject.Find("MainController").GetComponent<MainController>();
                //GameObject.Find("MainController")..AddComponent<ShowStatusWhenConnecting>();
                //_instance = v.AddComponent<MainController>();
            }
            else
            {
                GameObject go = new GameObject("MainController");
                _instance = go.AddComponent<MainController>();
            }
            return _instance;
        }
    }

    public static MainController _instance;

    public GameObject LoadingUI;
    //服务器版本
    public string Version = "1.0";
    private bool ConnectInUpdate = true;

    public static GameObject lobbyView;
    public static SoundManager _soundManager;
    public static int GameNum;

    public AssetBundle CommanAsset, GameAsset;
    //先留一个，后面留着赋值的
    public static string PhoneID;
    public static string OpenID;

    public static GameObject VoicePrefab;
    public static string sex;
    public static string NickName;
    public static string HeadIMG;
    public static int Coins;
    public static Texture2D HeadTex;
    //微信
    public ShareSDK ssdk;

    public static ViewState viewState;
    public enum ViewState
    {
        Loading = 0,
        GamesLobby = 1,
        RoomsLobby = 2,
        Game = 3,
    }
    private void Start()
    {
        ssdk = gameObject.AddComponent<ShareSDK> ();
        ssdk.authHandler = OnAuthResultHandler;
        ssdk.showUserHandler = OnGetUserInfoResultHandler;
        gameObject.AddComponent<HotFixManager>();
        LoadAssetBudle.Instance.DebugMode = false;
        LoadAssetBudle.Instance.CheckExtractResource();

    }
    public IEnumerator Init()
    {
        Debug.Log("hhhhhhh");
        // 开启热更新
        yield return StartCoroutine(HotFixManager.Instance.LoadHotFixAssembly());
        SetMainController();
    }
    public void SetMainController()
    {
        //热更新自动加载完UI后，这里开始对接功能
        Debug.Log("GameNum:" + GameNum);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        DateTime time = DateTime.Now;

        //加载声音预设
        VoicePrefab = Resources.Load("VoicePrefab") as GameObject;
        print(time.ToString());

        HotFixManager.Instance.appdomain.Invoke("HotFix_Project.MainController_HotFix", "Init", null, null);
    }
    //public void Login(GameObject go)
    //{
    //    switch (go.name)
    //    {
    //        case "GuestLoginBtn":
    //            ConnectPhoton();
    //            break;
    //    }
    //}
    private void Update()
    {
        CheckStatus();
    }

    public void WeChatInit(string appkey,string appcode)
    {
        ssdk = gameObject.AddComponent<ShareSDK>();
        ssdk.appKey = appkey;
        ssdk.appSecret = appcode;
        ssdk.authHandler = OnAuthResultHandler;
        ssdk.showUserHandler = OnGetUserInfoResultHandler;
    }

    public void WechatLogin()
    {
        ssdk.Authorize(PlatformType.WeChat);
        
    }
    //显示系统消息
    public static void ShowMessage(string message)
    {

    }

    public static void JoinRoomResponse(OperationResponse response)
    {
        HotFixManager.Instance.appdomain.Invoke("HotFix_Project.MainController_HotFix", "JoinRoomResponse", null, response);
    }

    public static void CreateRoomResponse(OperationResponse response)
    {
        HotFixManager.Instance.appdomain.Invoke("HotFix_Project.MainController_HotFix", "CreateRoomResponse", null, response);
    }

    public static void JoinRoomEvent(EventData photonEvent)
    {
        HotFixManager.Instance.appdomain.Invoke("HotFix_Project.MainController_HotFix", "JoinRoomEvent", null, photonEvent);
    }

    public static void LeaveRoomEvent(int ActorNr)
    {
        HotFixManager.Instance.appdomain.Invoke("HotFix_Project.MainController_HotFix", "LeaveRoomEvent", null, ActorNr);
    }

    public static void SetEventData(EventData photonEvent)
    {
        HotFixManager.Instance.appdomain.Invoke("HotFix_Project.MainController_HotFix", "SetEventData", null, photonEvent);
    }

    public void CheckStatus()
    {
        switch (PhotonNetwork.connectionStateDetailed)
        {
            case ClientState.Uninitialized:
                break;
            case ClientState.PeerCreated:
                viewState = ViewState.Loading;
                break;
            case ClientState.Queued:
                break;
            case ClientState.Authenticated:
                break;
            case ClientState.JoinedLobby:
                viewState = ViewState.GamesLobby;
                EnterLobby();
                break;
            case ClientState.DisconnectingFromMasterserver:
                break;
            case ClientState.ConnectingToGameserver:
                break;
            case ClientState.ConnectedToGameserver:
                break;
            case ClientState.Joining:
                break;
            case ClientState.Joined:
                viewState = ViewState.Game;
                break;
            case ClientState.Leaving:
                break;
            case ClientState.DisconnectingFromGameserver:
                break;
            case ClientState.ConnectingToMasterserver:
                break;
            case ClientState.QueuedComingFromGameserver:
                break;
            case ClientState.Disconnecting:
                break;
            case ClientState.Disconnected:
                break;
        }
    }
    public void ConnectPhoton()
    {

        //PhotonNetwork.ConnectToMaster
        if (!PhotonNetwork.connected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

            //ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version);
        }
    }

    public void EnterLobby()
    {
        if (!lobbyView)
        {
            _soundManager = SoundManager.Instance;
            lobbyView = Instantiate(MainController.Instance.CommanAsset.LoadAsset("GamesLobby") as GameObject) as GameObject;
            print(PhotonNetwork.player.Coins);
            HotFixManager.Instance.appdomain.Invoke("HotFix_Project.V.GamesLobbyView_HotFix", "Init", null, null);
        }
    }
    //可要可不要
    public static void GetUserInfo()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(PhoneID))
            PhoneID = UnityEngine.Random.RandomRange(0, 1000).ToString();

        print(PhoneID);
#elif UNITY_STANDALONE_WIN
        if (string.IsNullOrEmpty(PhoneID))
            PhoneID = UnityEngine.Random.RandomRange(0, 1000).ToString();

        print(PhoneID);
#elif UNITY_ANDROID
        if (string.IsNullOrEmpty(PhoneID))
		PhoneID = SystemInfo.deviceUniqueIdentifier;

#elif UNITY_IPHONE
		if (string.IsNullOrEmpty(PhoneID))
		PhoneID = SystemInfo.deviceUniqueIdentifier;
		print(PhoneID);
#elif UNITY_STANDALONE_OSX
		if (string.IsNullOrEmpty(PhoneID))
		PhoneID = UnityEngine.Random.RandomRange(0, 1000).ToString();
		print(PhoneID);
#endif
    }
    
    public static void SetUserInfo(Dictionary<byte, object> dic)
    {
        //1姓名
        //2性别
        //3头像
        //4金币
        PhotonNetwork.networkingPeer.LocalPlayer = new PhotonPlayer(true, 0, dic[1].ToString());

        PhotonNetwork.networkingPeer.LocalPlayer.NickName = dic[1].ToString();
        if (dic.ContainsKey(2) && dic[2] != null)
            PhotonNetwork.networkingPeer.LocalPlayer.Sex = Convert.ToByte(dic[2].ToString());
        if (dic.ContainsKey(3) && dic[3] != null)
            PhotonNetwork.networkingPeer.LocalPlayer.HeadIMG = dic[3].ToString();
        if (dic.ContainsKey(4))
            PhotonNetwork.networkingPeer.LocalPlayer.Coins = dic[4].ToString();
		if (dic.ContainsKey(5))
        PhotonNetwork.networkingPeer.LocalPlayer.UID = dic[5].ToString();

    }

    void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, System.Collections.Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            if (result != null && result.Count > 0)
            {
                print("authorize success !" + "Platform :" + type + "result:" + MiniJSON.jsonEncode(result));
            }
            else
            {
                print("authorize success !" + "Platform :" + type);
            }
            ssdk.GetUserInfo(PlatformType.WeChat);
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
		print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }


    void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, System.Collections.Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            Debug.Log("get user info result :");
            Debug.Log(MiniJSON.jsonEncode(result));
            Debug.Log("AuthInfo:" + MiniJSON.jsonEncode(ssdk.GetAuthInfo(PlatformType.WeChat)));
            Debug.Log("Get userInfo success !Platform :" + type);
            OpenID = result["openid"].ToString();
            NickName = result["nickname"].ToString();
            HeadIMG = result["headimgurl"].ToString();

            Debug.Log("nimamamade bi" + HeadIMG);
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
		print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }
}

