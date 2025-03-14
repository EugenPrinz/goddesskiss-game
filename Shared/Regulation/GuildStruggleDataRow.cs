using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class GuildStruggleDataRow : DataRow
	{
		public string idx { get; private set; }

		public int positionx { get; private set; }

		public int positiony { get; private set; }

		public int guildpoint { get; private set; }

		public string battlemap { get; private set; }

		public string enemymap { get; private set; }

		public Vector3 position => new Vector3(positionx, positiony, 0f);

		public string GetKey()
		{
			return idx;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
	}
}
