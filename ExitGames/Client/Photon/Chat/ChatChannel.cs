using System;
using System.Collections.Generic;
using System.Text;

namespace ExitGames.Client.Photon.Chat
{
	public class ChatChannel
	{
		public readonly string Name;

		public readonly List<string> Senders = new List<string>();

		public readonly List<object> Messages = new List<object>();

		public readonly List<long> MessageTimes = new List<long>();

		public int MessageLimit;

		public bool IsPrivate { get; protected internal set; }

		public int MessageCount => Messages.Count;

		public ChatChannel(string name)
		{
			Name = name;
		}

		public void Add(string sender, object message)
		{
			Senders.Add(sender);
			Messages.Add(message);
			MessageTimes.Add(DateTime.Now.Ticks);
			TruncateMessages();
		}

		public void Add(string[] senders, object[] messages)
		{
			Senders.AddRange(senders);
			Messages.AddRange(messages);
			for (int i = 0; i < messages.Length; i++)
			{
				MessageTimes.Add(DateTime.Now.Ticks);
			}
			TruncateMessages();
		}

		public void TruncateMessages()
		{
			if (MessageLimit > 0 && Messages.Count > MessageLimit)
			{
				int count = Messages.Count - MessageLimit;
				Senders.RemoveRange(0, count);
				Messages.RemoveRange(0, count);
				MessageTimes.RemoveRange(0, count);
			}
		}

		public void ClearMessages()
		{
			Senders.Clear();
			Messages.Clear();
			MessageTimes.Clear();
		}

		public string ToStringMessages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Messages.Count; i++)
			{
				stringBuilder.AppendLine($"{Senders[i]}: {Messages[i]}");
			}
			return stringBuilder.ToString();
		}

		public void RemoveMessages(int count)
		{
			if (MessageCount > 0 && count > 0)
			{
				if (count > MessageCount)
				{
					count = MessageCount;
				}
				Senders.RemoveRange(0, count);
				Messages.RemoveRange(0, count);
				MessageTimes.RemoveRange(0, count);
			}
		}

		public void LeaveMessage(int count)
		{
			if (MessageCount > count)
			{
				int count2 = Messages.Count - count;
				RemoveMessages(count2);
			}
		}
	}
}
