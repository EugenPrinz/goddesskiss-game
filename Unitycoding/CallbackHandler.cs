using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unitycoding
{
	public abstract class CallbackHandler : MonoBehaviour
	{
		[Serializable]
		public class Entry
		{
			public string eventID;

			public CallbackEvent callback;
		}

		[Serializable]
		public class CallbackEvent : UnityEvent<CallbackEventData>
		{
		}

		[HideInInspector]
		public List<Entry> delegates;

		public abstract string[] Callbacks { get; }

		protected void Execute(string eventID, CallbackEventData eventData)
		{
			if (delegates == null)
			{
				return;
			}
			int i = 0;
			for (int count = delegates.Count; i < count; i++)
			{
				Entry entry = delegates[i];
				if (entry.eventID == eventID && entry.callback != null)
				{
					entry.callback.Invoke(eventData);
				}
			}
		}

		public void RegisterListener(string eventID, UnityAction<CallbackEventData> call)
		{
			if (delegates == null)
			{
				delegates = new List<Entry>();
			}
			Entry entry = null;
			for (int i = 0; i < delegates.Count; i++)
			{
				Entry entry2 = delegates[i];
				if (entry2.eventID == eventID)
				{
					entry = entry2;
					break;
				}
			}
			if (entry == null)
			{
				entry = new Entry();
				entry.eventID = eventID;
				entry.callback = new CallbackEvent();
				delegates.Add(entry);
			}
			entry.callback.AddListener(call);
		}

		public void RemoveListener(string eventID, UnityAction<CallbackEventData> call)
		{
			if (delegates == null)
			{
				return;
			}
			for (int i = 0; i < delegates.Count; i++)
			{
				Entry entry = delegates[i];
				if (entry.eventID == eventID)
				{
					entry.callback.RemoveListener(call);
				}
			}
		}
	}
}
