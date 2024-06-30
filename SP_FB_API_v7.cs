using System;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class SP_FB_API_v7 : SP_FB_API
{
	private string _UserId = string.Empty;

	private string _AccessToken = string.Empty;

	public bool IsLoggedIn => FB.IsLoggedIn;

	public string UserId => _UserId;

	public string AccessToken => _AccessToken;

	public string AppId => FB.AppId;

	public static bool IsAPIEnabled => true;

	public void Init()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}

	public void Login(params string[] scopes)
	{
		List<string> list = new List<string>(scopes);
		if (list.Contains("publish_actions"))
		{
			FB.LogInWithPublishPermissions(list, LoginCallback);
		}
		else
		{
			FB.LogInWithReadPermissions(list, LoginCallback);
		}
	}

	public void Logout()
	{
		_UserId = string.Empty;
		_AccessToken = string.Empty;
		FB.LogOut();
	}

	public void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback)
	{
		new FB_GrapRequest_V7(query, ConvertHttpMethod(method), callback);
	}

	public void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback, WWWForm form)
	{
		new FB_GrapRequest_V7(query, ConvertHttpMethod(method), callback, form);
	}

	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, string[] to, string data = "", string title = "")
	{
		FB.AppRequest(message, ConvertActionType(actionType), objectId, to, data, title, AppRequestCallback);
	}

	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "")
	{
		FB.AppRequest(message, ConvertActionType(actionType), objectId, filters, excludeIds, maxRecipients, data, title, AppRequestCallback);
	}

	public void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "")
	{
		FB.AppRequest(message, to, filters, excludeIds, maxRecipients, data, title, AppRequestCallback);
	}

	public void FeedShare(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string actionName = "", string actionLink = "", string reference = "")
	{
		Uri link2 = new Uri(link);
		Uri picture2 = new Uri(picture);
		FB.FeedShare(toId, link2, linkName, linkCaption, linkDescription, picture2, reference, PostCallback);
	}

	private void AppRequestCallback(IAppRequestResult result)
	{
		FB_AppRequestResult result2 = new FB_AppRequestResult(result.RawResult, result.Error);
		SA_Singleton<SPFacebook>.Instance.AppRequestCallback(result2);
	}

	private void LoginCallback(ILoginResult result)
	{
		FB_LoginResult fB_LoginResult = ((result != null) ? new FB_LoginResult(result.RawResult, result.Error, result.Cancelled) : new FB_LoginResult(string.Empty, "Null Response", isCanceled: false));
		if (fB_LoginResult.IsSucceeded && !result.Cancelled && result.AccessToken != null)
		{
			_UserId = result.AccessToken.UserId;
			_AccessToken = result.AccessToken.TokenString;
			fB_LoginResult.SetCredential(_UserId, _AccessToken);
		}
		SA_Singleton<SPFacebook>.Instance.LoginCallback(fB_LoginResult);
	}

	private void OnInitComplete()
	{
		if (Facebook.Unity.AccessToken.CurrentAccessToken != null)
		{
			_AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
			_UserId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
		}
		SA_Singleton<SPFacebook>.Instance.OnInitComplete();
	}

	private void OnHideUnity(bool isGameShown)
	{
		SA_Singleton<SPFacebook>.Instance.OnHideUnity(isGameShown);
	}

	private void PostCallback(IShareResult result)
	{
		FB_PostResult result2 = new FB_PostResult(result.RawResult, result.Error);
		SA_Singleton<SPFacebook>.Instance.PostCallback(result2);
	}

	private HttpMethod ConvertHttpMethod(FB_HttpMethod method)
	{
		return method switch
		{
			FB_HttpMethod.GET => HttpMethod.GET, 
			FB_HttpMethod.POST => HttpMethod.POST, 
			FB_HttpMethod.DELETE => HttpMethod.POST, 
			_ => HttpMethod.GET, 
		};
	}

	private OGActionType ConvertActionType(FB_RequestActionType actionType)
	{
		return actionType switch
		{
			FB_RequestActionType.AskFor => OGActionType.ASKFOR, 
			FB_RequestActionType.Send => OGActionType.SEND, 
			FB_RequestActionType.Turn => OGActionType.TURN, 
			_ => OGActionType.ASKFOR, 
		};
	}
}
