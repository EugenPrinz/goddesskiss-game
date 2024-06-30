using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class ScenarioQuarterDataRow : DataRow
	{
		public string csid;

		public string quarter;

		public string GetKey()
		{
			return quarter.ToString();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
