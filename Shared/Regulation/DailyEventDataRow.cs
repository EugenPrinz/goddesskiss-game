using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class DailyEventDataRow : DataRow
	{
		public string id { get; private set; }

		public EWeekType week { get; private set; }

		public string start { get; private set; }

		public int time { get; private set; }

		public int type { get; private set; }

		public string name { get; private set; }

		public string GetKey()
		{
			return ((int)week).ToString();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
