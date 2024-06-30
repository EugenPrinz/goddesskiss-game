using UnityEngine;

public class UniWeb : MonoBehaviour
{
	public bool bLoadComplete;

	public int mTop = 100;

	public int mLeft = 120;

	public int mRight = 120;

	public int mBottom = 100;

	private UniWebView _webView;

	private string _errorMessage;

	public UniWebView GetWebView()
	{
		return _webView;
	}

	public void CloseWebView()
	{
		_webView.Hide();
		Object.Destroy(_webView);
		_webView.OnReceivedMessage -= OnReceivedMessage;
		_webView.OnLoadComplete -= OnLoadComplete;
		_webView.OnWebViewShouldClose -= OnWebViewShouldClose;
		_webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
		_webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
		_webView = null;
	}

	public void OpenWebView()
	{
		_webView = GetComponent<UniWebView>();
		if (_webView == null)
		{
			_webView = base.gameObject.AddComponent<UniWebView>();
			_webView.OnReceivedMessage += OnReceivedMessage;
			_webView.OnLoadComplete += OnLoadComplete;
			_webView.OnWebViewShouldClose += OnWebViewShouldClose;
			_webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
			_webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
		}
		_webView.url = "http://google.com";
		_webView.Load();
		_errorMessage = null;
	}

	private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		bLoadComplete = success;
		if (success)
		{
			webView.Show();
		}
		else
		{
			_errorMessage = errorMessage;
		}
	}

	private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		if (string.Equals(message.path, "close"))
		{
			webView.Hide();
			Object.Destroy(webView);
			webView.OnReceivedMessage -= OnReceivedMessage;
			webView.OnLoadComplete -= OnLoadComplete;
			webView.OnWebViewShouldClose -= OnWebViewShouldClose;
			webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
			webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
			_webView = null;
		}
	}

	public void ShowAlertInWebview(float time, bool first)
	{
		if (first)
		{
			_webView.EvaluatingJavaScript("sample(" + time + ")");
		}
	}

	private void OnEvalJavaScriptFinished(UniWebView webView, string result)
	{
	}

	private bool OnWebViewShouldClose(UniWebView webView)
	{
		if (webView == _webView)
		{
			_webView = null;
			return true;
		}
		return false;
	}

	private UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
	{
		int aBottom = (int)((float)UniWebViewHelper.screenHeight * 0.5f);
		if (orientation == UniWebViewOrientation.Portrait)
		{
			return new UniWebViewEdgeInsets(5, 5, aBottom, 5);
		}
		return new UniWebViewEdgeInsets(mTop, mLeft, mBottom, mRight);
	}
}
