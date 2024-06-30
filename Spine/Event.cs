namespace Spine
{
	public class Event
	{
		public EventData Data { get; private set; }

		public int Int { get; set; }

		public float Float { get; set; }

		public string String { get; set; }

		public float Time { get; private set; }

		public Event(float time, EventData data)
		{
			Time = time;
			Data = data;
		}

		public override string ToString()
		{
			return Data.Name;
		}
	}
}
