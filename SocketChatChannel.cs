using System;
using System.Collections.Generic;

public class SocketChatChannel
{
	public readonly string Name;

	public readonly List<string> Messages = new List<string>();

	public readonly List<long> MessageTimes = new List<long>();

	public int MessageLimit;

	public int MessageCount => Messages.Count;

	public SocketChatChannel(string name)
	{
		Name = name;
	}

	public void Add(string message)
	{
		Messages.Add(message);
		MessageTimes.Add(DateTime.Now.Ticks);
		TruncateMessages();
	}

	public void Add(string[] messages)
	{
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
			Messages.RemoveRange(0, count);
			MessageTimes.RemoveRange(0, count);
		}
	}

	public void ClearMessages()
	{
		Messages.Clear();
		MessageTimes.Clear();
	}

	public void RemoveMessages(int count)
	{
		if (MessageCount > 0 && count > 0)
		{
			if (count > MessageCount)
			{
				count = MessageCount;
			}
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
