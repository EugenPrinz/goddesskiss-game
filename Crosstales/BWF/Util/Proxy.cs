using UnityEngine;

namespace Crosstales.BWF.Util
{
	[DisallowMultipleComponent]
	[HelpURL("http://www.crosstales.com/en/assets/badwordfilter/api/class_crosstales_1_1_bad_word_1_1_proxy.html")]
	public class Proxy : MonoBehaviour
	{
		[Header("HTTP Proxy Settings")]
		[Tooltip("URL (without protocol) or IP of the proxy server.")]
		public string HTTPProxyURL;

		[Tooltip("Port of the proxy server.")]
		public int HTTPProxyPort;

		[Tooltip("Username for the proxy server (optional).")]
		public string HTTPProxyUsername = string.Empty;

		[Tooltip("Password for the proxy server (optional).")]
		public string HTTPProxyPassword = string.Empty;

		[Tooltip("Protocol (e.g. 'http://') for the proxy server (optional).")]
		public string HTTPProxyURLProtocol = string.Empty;

		[Header("HTTPS Proxy Settings")]
		[Tooltip("URL (without protocol) or IP of the proxy server.")]
		public string HTTPSProxyURL;

		[Tooltip("Port of the proxy server.")]
		public int HTTPSProxyPort;

		[Tooltip("Username for the proxy server (optional).")]
		public string HTTPSProxyUsername = string.Empty;

		[Tooltip("Password for the proxy server (optional).")]
		public string HTTPSProxyPassword = string.Empty;

		[Tooltip("Protocol (e.g. 'http://') for the proxy server (optional).")]
		public string HTTPSProxyURLProtocol = string.Empty;

		[Header("Startup behaviour")]
		[Tooltip("Enable the proxy on awake (default: off).")]
		public bool EnableOnAwake;

		private const string HTTPProxyEnvVar = "HTTP_PROXY";

		private const string HTTPSProxyEnvVar = "HTTPS_PROXY";

		public void Awake()
		{
			if (EnableOnAwake)
			{
				EnableHTTPProxy();
				EnableHTTPSProxy();
			}
		}

		public void EnableHTTPProxy(bool enabled = true)
		{
			if (enabled)
			{
				EnableHTTPProxy(HTTPProxyURL, HTTPProxyPort, HTTPProxyUsername, HTTPProxyPassword, HTTPProxyURLProtocol);
			}
			else
			{
				DisableHTTPProxy();
			}
		}

		public void EnableHTTPSProxy(bool enabled = true)
		{
			if (enabled)
			{
				EnableHTTPSProxy(HTTPSProxyURL, HTTPSProxyPort, HTTPSProxyUsername, HTTPSProxyPassword, HTTPSProxyURLProtocol);
			}
			else
			{
				DisableHTTPSProxy();
			}
		}

		public void EnableHTTPProxy(string url, int port, string username = "", string password = "", string urlProtocol = "")
		{
		}

		public void EnableHTTPSProxy(string url, int port, string username = "", string password = "", string urlProtocol = "")
		{
		}

		public void DisableHTTPProxy()
		{
		}

		public void DisableHTTPSProxy()
		{
		}

		private static bool validPort(int port)
		{
			return port >= 0 && port < 65536;
		}
	}
}
