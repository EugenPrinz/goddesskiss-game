using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class RaidDataRow : DataRow
	{
		public int index { get; private set; }

		public int key { get; private set; }

		public int phase { get; private set; }

		public int attackIncrease { get; private set; }

		public int defenseIncrease { get; private set; }

		public int accuracyIncrease { get; private set; }

		public int luckIncrease { get; private set; }

		public int criticalIncrease { get; private set; }

		public int criticalDmgIncrease { get; private set; }

		public int speedIncrease { get; private set; }

		public string effectName { get; private set; }

		public int pos { get; private set; }

		public int health { get; private set; }

		private RaidDataRow()
		{
		}

		public string GetKey()
		{
			return $"{key}_{phase}_{pos}";
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
