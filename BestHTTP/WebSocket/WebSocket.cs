using System;
using System.IO;
using System.Text;
using BestHTTP.Decompression.Zlib;
using BestHTTP.WebSocket.Extensions;
using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket
{
	public sealed class WebSocket
	{
		public OnWebSocketOpenDelegate OnOpen;

		public OnWebSocketMessageDelegate OnMessage;

		public OnWebSocketBinaryDelegate OnBinary;

		public OnWebSocketClosedDelegate OnClosed;

		public OnWebSocketErrorDelegate OnError;

		public OnWebSocketErrorDescriptionDelegate OnErrorDesc;

		public OnWebSocketIncompleteFrameDelegate OnIncompleteFrame;

		private bool requestSent;

		private WebSocketResponse webSocket;

		public bool IsOpen => webSocket != null && !webSocket.IsClosed;

		public bool StartPingThread { get; set; }

		public int PingFrequency { get; set; }

		public HTTPRequest InternalRequest { get; private set; }

		public IExtension[] Extensions { get; private set; }

		public WebSocket(Uri uri)
			: this(uri, string.Empty, string.Empty)
		{
			Extensions = new IExtension[1]
			{
				new PerMessageCompression(CompressionLevel.Default, clientNoContextTakeover: false, serverNoContextTakeover: false, 15, 15, 5)
			};
		}

		public WebSocket(Uri uri, string origin, string protocol, params IExtension[] extensions)
		{
			PingFrequency = 1000;
			if (uri.Port == -1)
			{
				uri = new Uri(uri.Scheme + "://" + uri.Host + ":" + ((!uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase)) ? "80" : "443") + uri.PathAndQuery);
			}
			InternalRequest = new HTTPRequest(uri, OnInternalRequestCallback);
			InternalRequest.OnUpgraded = OnInternalRequestUpgraded;
			InternalRequest.SetHeader("Host", uri.Host + ":" + uri.Port);
			InternalRequest.SetHeader("Upgrade", "websocket");
			InternalRequest.SetHeader("Connection", "keep-alive, Upgrade");
			InternalRequest.SetHeader("Sec-WebSocket-Key", GetSecKey(new object[4]
			{
				this,
				InternalRequest,
				uri,
				new object()
			}));
			if (!string.IsNullOrEmpty(origin))
			{
				InternalRequest.SetHeader("Origin", origin);
			}
			InternalRequest.SetHeader("Sec-WebSocket-Version", "13");
			if (!string.IsNullOrEmpty(protocol))
			{
				InternalRequest.SetHeader("Sec-WebSocket-Protocol", protocol);
			}
			InternalRequest.SetHeader("Cache-Control", "no-cache");
			InternalRequest.SetHeader("Pragma", "no-cache");
			Extensions = extensions;
			if (HTTPManager.Proxy != null)
			{
				InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, isTransparent: false, sendWholeUri: false, HTTPManager.Proxy.NonTransparentForHTTPS);
			}
		}

		private void OnInternalRequestCallback(HTTPRequest req, HTTPResponse resp)
		{
			string empty = string.Empty;
			switch (req.State)
			{
			default:
				return;
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess || resp.StatusCode == 101)
				{
					HTTPManager.Logger.Information("WebSocket", $"Request finished. Status Code: {resp.StatusCode.ToString()} Message: {resp.Message}");
					return;
				}
				empty = $"Request Finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}";
				break;
			case HTTPRequestStates.Error:
				empty = "Request Finished with Error! " + ((req.Exception == null) ? string.Empty : ("Exception: " + req.Exception.Message + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				empty = "Request Aborted!";
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				empty = "Connection Timed Out!";
				break;
			case HTTPRequestStates.TimedOut:
				empty = "Processing the request Timed Out!";
				break;
			}
			if (OnError != null)
			{
				OnError(this, req.Exception);
			}
			if (OnErrorDesc != null)
			{
				OnErrorDesc(this, empty);
			}
			if (OnError == null && OnErrorDesc == null)
			{
				HTTPManager.Logger.Error("WebSocket", empty);
			}
		}

		private void OnInternalRequestUpgraded(HTTPRequest req, HTTPResponse resp)
		{
			webSocket = resp as WebSocketResponse;
			if (webSocket == null)
			{
				if (OnError != null)
				{
					OnError(this, req.Exception);
				}
				if (OnErrorDesc != null)
				{
					string reason = string.Empty;
					if (req.Exception != null)
					{
						reason = req.Exception.Message + " " + req.Exception.StackTrace;
					}
					OnErrorDesc(this, reason);
				}
				return;
			}
			webSocket.WebSocket = this;
			if (Extensions != null)
			{
				for (int i = 0; i < Extensions.Length; i++)
				{
					IExtension extension = Extensions[i];
					try
					{
						if (extension != null && !extension.ParseNegotiation(webSocket))
						{
							Extensions[i] = null;
						}
					}
					catch (Exception ex)
					{
						HTTPManager.Logger.Exception("WebSocket", "ParseNegotiation", ex);
						Extensions[i] = null;
					}
				}
			}
			if (OnOpen != null)
			{
				try
				{
					OnOpen(this);
				}
				catch (Exception ex2)
				{
					HTTPManager.Logger.Exception("WebSocket", "OnOpen", ex2);
				}
			}
			webSocket.OnText = delegate(WebSocketResponse ws, string msg)
			{
				if (OnMessage != null)
				{
					OnMessage(this, msg);
				}
			};
			webSocket.OnBinary = delegate(WebSocketResponse ws, byte[] bin)
			{
				if (OnBinary != null)
				{
					OnBinary(this, bin);
				}
			};
			webSocket.OnClosed = delegate(WebSocketResponse ws, ushort code, string msg)
			{
				if (OnClosed != null)
				{
					OnClosed(this, code, msg);
				}
			};
			if (OnIncompleteFrame != null)
			{
				webSocket.OnIncompleteFrame = delegate(WebSocketResponse ws, WebSocketFrameReader frame)
				{
					if (OnIncompleteFrame != null)
					{
						OnIncompleteFrame(this, frame);
					}
				};
			}
			if (StartPingThread)
			{
				webSocket.StartPinging(Math.Max(PingFrequency, 100));
			}
			webSocket.StartReceive();
		}

		public void Open()
		{
			if (requestSent || InternalRequest == null)
			{
				return;
			}
			if (Extensions != null)
			{
				try
				{
					for (int i = 0; i < Extensions.Length; i++)
					{
						Extensions[i]?.AddNegotiation(InternalRequest);
					}
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("WebSocket", "Open", ex);
				}
			}
			InternalRequest.Send();
			requestSent = true;
		}

		public void Send(string message)
		{
			if (IsOpen)
			{
				webSocket.Send(message);
			}
		}

		public void Send(byte[] buffer)
		{
			if (IsOpen)
			{
				webSocket.Send(buffer);
			}
		}

		public void Send(byte[] buffer, ulong offset, ulong count)
		{
			if (IsOpen)
			{
				webSocket.Send(buffer, offset, count);
			}
		}

		public void Send(WebSocketFrame frame)
		{
			if (IsOpen)
			{
				webSocket.Send(frame);
			}
		}

		public void Close()
		{
			if (IsOpen)
			{
				webSocket.Close();
			}
		}

		public void Close(ushort code, string message)
		{
			if (IsOpen)
			{
				webSocket.Close(code, message);
			}
		}

		public static byte[] EncodeCloseData(ushort code, string message)
		{
			int byteCount = Encoding.UTF8.GetByteCount(message);
			using MemoryStream memoryStream = new MemoryStream(2 + byteCount);
			byte[] bytes = BitConverter.GetBytes(code);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes, 0, bytes.Length);
			}
			memoryStream.Write(bytes, 0, bytes.Length);
			bytes = Encoding.UTF8.GetBytes(message);
			memoryStream.Write(bytes, 0, bytes.Length);
			return memoryStream.ToArray();
		}

		private string GetSecKey(object[] from)
		{
			byte[] array = new byte[16];
			int num = 0;
			for (int i = 0; i < from.Length; i++)
			{
				byte[] bytes = BitConverter.GetBytes(from[i].GetHashCode());
				for (int j = 0; j < bytes.Length; j++)
				{
					if (num >= array.Length)
					{
						break;
					}
					array[num++] = bytes[j];
				}
			}
			return Convert.ToBase64String(array);
		}
	}
}
