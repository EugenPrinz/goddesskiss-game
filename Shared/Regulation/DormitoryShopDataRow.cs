using System;
using Newtonsoft.Json;
using RoomDecorator;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class DormitoryShopDataRow : DataRow
	{
		public EDormitoryItemType type { get; private set; }

		[JsonProperty("idx")]
		public string id { get; private set; }

		public int priceType { get; private set; }

		public EDormitoryShopSellType sellType { get; private set; }

		public string GetKey()
		{
			return $"{type}_{id}";
		}
	}
}
