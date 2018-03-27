using UnityEngine;
using System.Collections;
using System;

namespace cn.sharesdk.unity3d 
{
	[Serializable]
	public class DevInfoSet
	{
		public WeChat wechat;
		public WeChatMoments wechatMoments; 
		public WeChatFavorites wechatFavorites;
	}

	public class DevInfo 
	{	
		public bool Enable = true;
	}

	
	[Serializable]
	public class WeChat : DevInfo 
	{	
		#if UNITY_ANDROID
		public string SortId = "5";
		public const int type = (int) PlatformType.WeChat;
		public string AppId = "wx4868b35061f87885";
		public string AppSecret = "64020361b8ec4c99936c0e3999a9f249";
		public string UserName = "gh_afb25ac019c9@app";
		public string Path = "/page/API/pages/share/share";
		public bool BypassApproval = true;
		public bool WithShareTicket = true;
		public string MiniprogramType = "0";
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChat;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}

	[Serializable]
	public class WeChatMoments : DevInfo 
	{
		#if UNITY_ANDROID
		public string SortId = "6";
		public const int type = (int) PlatformType.WeChatMoments;
		public string AppId = "wx4868b35061f87885";
		public string AppSecret = "64020361b8ec4c99936c0e3999a9f249";
		public bool BypassApproval = true;
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChatMoments;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}

	[Serializable]
	public class WeChatFavorites : DevInfo 
	{
		#if UNITY_ANDROID
		public string SortId = "7";
		public const int type = (int) PlatformType.WeChatFavorites;
		public string AppId = "wx4868b35061f87885";
		public string AppSecret = "64020361b8ec4c99936c0e3999a9f249";
		#elif UNITY_IPHONE
		public const int type = (int) PlatformType.WeChatFavorites;
		public string app_id = "wx4868b35061f87885";
		public string app_secret = "64020361b8ec4c99936c0e3999a9f249";
		#endif
	}

}
