using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class FavorDataRow : DataRow
	{
		public int cid { get; private set; }

		public int step { get; private set; }

		public string profile { get; private set; }

		public StatType statType { get; private set; }

		public int stat { get; private set; }

		public string GetKey()
		{
			return cid.ToString();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
