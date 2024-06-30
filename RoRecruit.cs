using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class RoRecruit
{
	[JsonObject]
	public class Entry
	{
		public RoCommander commander { get; set; }

		public bool recruited { get; set; }

		public bool exist { get; set; }

		public TimeData delayTime { get; set; }

		public int delayCount { get; set; }

		public int gold { get; set; }

		public int honor { get; set; }

		public int medal { get; set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}

		public static Entry Create(string commanderId, int level, int grade, int cls, int costume, int favorRewardStep, int marry, List<int> transcendence)
		{
			Entry entry = new Entry();
			entry.commander = RoCommander.Create(commanderId, level, grade, cls, costume, favorRewardStep, marry, transcendence);
			entry.recruited = false;
			entry.delayTime = TimeData.Create();
			entry.delayCount = 0;
			return entry;
		}
	}

	public static double RefreshInterval = Utility.ToSeconds(0.0, 0.0, 1.0, 0.0);

	public static double DelayTimePerWait = Utility.ToSeconds(7.0, 0.0, 0.0, 0.0);

	[JsonProperty]
	public TimeData refreshTime { get; set; }

	[JsonProperty]
	public List<Entry> entryList { get; set; }

	public int entryCount { get; set; }

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		if (entryList == null)
		{
			entryList = new List<Entry>();
		}
	}

	public static RoRecruit Create()
	{
		RoRecruit roRecruit = new RoRecruit();
		roRecruit.entryList = new List<Entry>();
		roRecruit.refreshTime = TimeData.Create();
		roRecruit.entryCount = 8;
		return roRecruit;
	}

	public void Clear()
	{
		entryList.Clear();
		refreshTime.SetInvalidValue();
	}

	public List<RoCommander> GetCommanderList()
	{
		List<RoCommander> list = new List<RoCommander>();
		entryList.ForEach(delegate(Entry entry)
		{
			list.Add(entry.commander);
		});
		return list;
	}

	public Entry Find(string commanderId)
	{
		return entryList.Find((Entry entry) => entry.commander.id == commanderId);
	}

	public int FindIndex(string commanderId)
	{
		return entryList.FindIndex((Entry entry) => entry.commander.id == commanderId);
	}

	public void RemoveNoDealyCommander()
	{
		entryList.RemoveAll((Entry entry) => entry.recruited || !entry.delayTime.IsProgress());
	}
}
