using UnityEngine;

public class GLinkAndroid : IGLink
{
	private class OnClickAppSchemeBannerListener : AndroidJavaProxy
	{
		public OnClickAppSchemeBannerListener()
			: base("com.naver.glink.android.sdk.Glink$OnClickAppSchemeBannerListener")
		{
		}

		private void onClickAppSchemeBanner(string appScheme)
		{
			showToast("tapped:" + appScheme);
		}
	}

	private class OnSdkStartedListener : AndroidJavaProxy
	{
		public OnSdkStartedListener()
			: base("com.naver.glink.android.sdk.Glink$OnSdkStartedListener")
		{
		}

		private void onSdkStarted()
		{
			showToast("sdk start.");
			UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		}
	}

	private class OnSdkStoppedListener : AndroidJavaProxy
	{
		public OnSdkStoppedListener()
			: base("com.naver.glink.android.sdk.Glink$OnSdkStoppedListener")
		{
		}

		private void onSdkStopped()
		{
			showToast("sdk stop.");
			RemoteObjectManager.instance.localUser.isShowGLink = false;
			UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		}
	}

	private class OnLoggedInListener : AndroidJavaProxy
	{
		public OnLoggedInListener()
			: base("com.naver.glink.android.sdk.Glink$OnLoggedInListener")
		{
		}

		private void onLoggedIn(bool success)
		{
		}
	}

	private class OnJoinedListener : AndroidJavaProxy
	{
		public OnJoinedListener()
			: base("com.naver.glink.android.sdk.Glink$OnJoinedListener")
		{
		}

		private void onJoined()
		{
			showToast("카페에 가입하였습니다. (from listener)");
		}
	}

	private class OnPostedArticleListener : AndroidJavaProxy
	{
		public OnPostedArticleListener()
			: base("com.naver.glink.android.sdk.Glink$OnPostedArticleListener")
		{
		}

		private void onPostedArticle(int menuId, int imageCount, int videoCount)
		{
			string message = $"게시글이 작성되었습니다. (from listener, 메뉴: {menuId})";
			showToast(message);
			if (RemoteObjectManager.instance.localUser.articleEvent != null && RemoteObjectManager.instance.localUser.articleEvent.Contains(menuId))
			{
				RemoteObjectManager.instance.RequestGetPostEventReward(menuId);
				RemoteObjectManager.instance.localUser.articleEvent.Remove(menuId);
			}
		}
	}

	private class OnPostedCommentListener : AndroidJavaProxy
	{
		public OnPostedCommentListener()
			: base("com.naver.glink.android.sdk.Glink$OnPostedCommentListener")
		{
		}

		private void onPostedComment(int articleId)
		{
			string message = $"댓글이 작성되었습니다. (from listener, 게시글: {articleId})";
			showToast(message);
			if (RemoteObjectManager.instance.localUser.commentEvent != null && RemoteObjectManager.instance.localUser.commentEvent.Contains(articleId))
			{
				RemoteObjectManager.instance.RequestGetCommentEventReward(articleId);
				RemoteObjectManager.instance.localUser.commentEvent.Remove(articleId);
			}
		}
	}

	private class OnWidgetScreenshotClickListener : AndroidJavaProxy
	{
		public OnWidgetScreenshotClickListener()
			: base("com.naver.glink.android.sdk.Glink$OnWidgetScreenshotClickListener")
		{
		}

		private void onScreenshotClick()
		{
			string name = "CafeSdkController";
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				gameObject = new GameObject("CafeSdkController");
				gameObject.AddComponent<SampleBehaviour>();
			}
			SampleBehaviour component = gameObject.GetComponent<SampleBehaviour>();
			component.OnClickScreenShotButton();
		}
	}

	private class OnRecordFinishListener : AndroidJavaProxy
	{
		public OnRecordFinishListener()
			: base("com.naver.glink.android.sdk.Glink$OnRecordFinishListener")
		{
		}

		private void onRecordFinished(string uri)
		{
			GLinkAndroid gLinkAndroid = (GLinkAndroid)GLink.sharedInstance();
			gLinkAndroid.executeArticlePostWithVideo(uri);
		}
	}

	private class OnVotedListener : AndroidJavaProxy
	{
		public OnVotedListener()
			: base("com.naver.glink.android.sdk.Glink$OnVotedListener")
		{
		}

		private void onVoted(int articleId)
		{
			string message = $"on voted. (from listener, 게시글: {articleId})";
			showToast(message);
		}
	}

	private class OnEndStreamingLiveListener : AndroidJavaProxy
	{
		public OnEndStreamingLiveListener()
			: base("com.naver.glink.android.sdk.Glink$OnEndStreamingLiveListener")
		{
		}

		private void onEndStreamingLive(int viewCount, int likeCount)
		{
			string message = $"viewCount: {viewCount}, likeCount: {likeCount}";
			showToast(message);
		}
	}

	private class OnEndWatchingLiveListener : AndroidJavaProxy
	{
		public OnEndWatchingLiveListener()
			: base("com.naver.glink.android.sdk.Glink$OnEndWatchingLiveListener")
		{
		}

		private void onEndWatchingLive(int seconds)
		{
			string message = $"seconds: {seconds}";
			showToast(message);
		}
	}

	private AndroidJavaClass glinkClass;

	private AndroidJavaObject currentActivity;

	public GLinkAndroid()
	{
		currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		glinkClass = new AndroidJavaClass("com.naver.glink.android.sdk.Glink");
		glinkClass.CallStatic("init", currentActivity, "h0cPh5hzfLrfkZ26j5E1", "LsO4iz8waE", 28693157);
		glinkClass.CallStatic("initGlobal", currentActivity, "ZlLtnYSLhNNjvegYkxPo", 1013345);
		glinkClass.CallStatic("setOnSdkStartedListener", new OnSdkStartedListener());
		glinkClass.CallStatic("setOnSdkStoppedListener", new OnSdkStoppedListener());
		glinkClass.CallStatic("setOnPostedArticleListener", new OnPostedArticleListener());
		glinkClass.CallStatic("setOnPostedCommentListener", new OnPostedCommentListener());
		glinkClass.CallStatic("setOnWidgetScreenshotClickListener", new OnWidgetScreenshotClickListener());
		setUseWidgetVideoRecord(useVideoRecord: true);
		glinkClass.CallStatic("setOnRecordFinishListener", new OnRecordFinishListener());
	}

	private static void showToast(string message)
	{
	}

	public void executeHome()
	{
		glinkClass.CallStatic("startHome", currentActivity);
	}

	public void executeArticle(int articleId)
	{
		glinkClass.CallStatic("startArticle", currentActivity, articleId);
	}

	public void executeArticlePost()
	{
		glinkClass.CallStatic("startWrite", currentActivity);
	}

	public void executeArticlePostWithImage(string filePath)
	{
		glinkClass.CallStatic("startImageWrite", currentActivity, "file://" + filePath);
	}

	public void executeArticlePostWithVideo(string filePath)
	{
		glinkClass.CallStatic("startVideoWrite", currentActivity, "file://" + filePath);
	}

	public void executeMore()
	{
		glinkClass.CallStatic("startMore", currentActivity);
	}

	public void syncGameUserId(string gameUserId)
	{
		glinkClass.CallStatic("syncGameUserId", currentActivity, gameUserId);
	}

	public void startWidget()
	{
		glinkClass.CallStatic("startWidget", currentActivity);
	}

	public void stopWidget()
	{
		glinkClass.CallStatic("stopWidget", currentActivity);
	}

	public void setUseWidgetVideoRecord(bool useVideoRecord)
	{
		glinkClass.CallStatic("setUseVideoRecord", currentActivity, useVideoRecord);
	}

	public void setUseWidgetScreenShot(bool useScreenShot)
	{
		glinkClass.CallStatic("setUseScreenshot", currentActivity, useScreenShot);
	}

	public void setShowWidgetWhenUnloadSDK(bool useWidget)
	{
		glinkClass.CallStatic("showWidgetWhenUnloadSdk", currentActivity, useWidget);
	}

	public string getCurrentChannelCode()
	{
		string text = null;
		return glinkClass.CallStatic<string>("getChannelCode", new object[0]);
	}

	public void setChannelCode(string channelCode)
	{
		glinkClass.CallStatic("setChannelCode", channelCode);
	}

	public void setThemeColor(string themeColorCSSString)
	{
		glinkClass.CallStatic("setThemeColor", themeColorCSSString);
	}

	public void setThemeColor(string themeColorCSSString, string backgroundCSSString)
	{
		glinkClass.CallStatic("setThemeColor", themeColorCSSString, backgroundCSSString);
	}

	public void setWidgetStartPosition(bool isLeft, int heightPercentage)
	{
		glinkClass.CallStatic("setWidgetStartPosition", currentActivity, isLeft, heightPercentage);
	}
}
