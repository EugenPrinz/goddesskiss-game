using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class CommanderRankDataRow : DataRow
	{
		public int rank { get; private set; }

		public EGoods medalGoodsType { get; private set; }

		public int medal { get; private set; }

		public int gold { get; private set; }

		public int completeCash { get; private set; }

		public int time { get; private set; }

		public string GetKey()
		{
			return rank.ToString();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
