using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class FavorStepDataRow : DataRow
	{
		public int step { get; private set; }

		public int favor { get; private set; }

		public string GetKey()
		{
			return step.ToString();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
