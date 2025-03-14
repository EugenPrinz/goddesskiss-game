using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class WorldMapStageTypeDataRow : DataRow
	{
		public string id { get; private set; }

		public string resourceId { get; private set; }

		public int battleCount { get; private set; }

		public string bgm { get; private set; }

		public string GetKey()
		{
			return id;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
