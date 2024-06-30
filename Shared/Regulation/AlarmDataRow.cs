using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class AlarmDataRow : DataRow
	{
		public string idx { get; private set; }

		public string key { get; private set; }

		public string title { get; private set; }

		public string description { get; private set; }

		private AlarmDataRow()
		{
		}

		public string GetKey()
		{
			return key;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
