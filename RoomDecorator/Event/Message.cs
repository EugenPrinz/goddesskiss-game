using System;
using System.Collections.Generic;

namespace RoomDecorator.Event
{
	public class Message
	{
		public delegate void OnMessageHandleDelegate(Type callerType, Type handlerType, Type messageType, string messageName, string handlerMethodName);

		public static OnMessageHandleDelegate OnMessageHandle;

		private static Dictionary<string, List<Delegate>> handlers = new Dictionary<string, List<Delegate>>();

		private const string TypelessMessagePrefix = "typeless ";

		protected Message()
		{
		}

		public static void AddListener(string messageName, Action callback)
		{
			RegisterListener("typeless " + messageName, callback);
		}

		public static void AddListener<T>(Action<T> callback)
		{
			RegisterListener(typeof(T).ToString(), callback);
		}

		public static void AddListener<T>(string messageName, Action<T> callback)
		{
			RegisterListener(typeof(T).ToString() + messageName, callback);
		}

		public static void RemoveListener(string messageName, Action callback)
		{
			UnregisterListener("typeless " + messageName, callback);
		}

		public static void RemoveListener<T>(Action<T> callback)
		{
			UnregisterListener(typeof(T).ToString(), callback);
		}

		public static void RemoveListener<T>(string messageName, Action<T> callback)
		{
			UnregisterListener(typeof(T).ToString() + messageName, callback);
		}

		public static void Send(string messageName)
		{
			SendMessage<Message>("typeless " + messageName, null);
		}

		public static void Send<T>(T message)
		{
			SendMessage(typeof(T).ToString(), message);
		}

		public static void Send<T>(string messageName, T message)
		{
			SendMessage(typeof(T).ToString() + messageName, message);
		}

		private static void RegisterListener(string messageName, Delegate callback)
		{
			if ((object)callback != null)
			{
				if (!handlers.ContainsKey(messageName))
				{
					handlers.Add(messageName, new List<Delegate>());
				}
				List<Delegate> list = handlers[messageName];
				list.Add(callback);
			}
		}

		private static void UnregisterListener(string messageName, Delegate callback)
		{
			if (handlers.ContainsKey(messageName))
			{
				List<Delegate> list = handlers[messageName];
				Delegate @delegate = list.Find((Delegate x) => x.Method == callback.Method && x.Target == callback.Target);
				if ((object)@delegate != null)
				{
					list.Remove(@delegate);
				}
			}
		}

		private static void SendMessage<T>(string messageName, T e)
		{
			if (!handlers.ContainsKey(messageName))
			{
				return;
			}
			List<Delegate> list = handlers[messageName];
			foreach (Delegate item in list)
			{
				if (item.GetType() == typeof(Action<T>) || item.GetType() == typeof(Action))
				{
					if (typeof(T) == typeof(Message))
					{
						Action action = (Action)item;
						action();
					}
					else
					{
						Action<T> action2 = (Action<T>)item;
						action2(e);
					}
				}
			}
		}
	}
}
