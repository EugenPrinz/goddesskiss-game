using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class NPCMercenaryDataRow : DataRow
	{
		public string id { get; private set; }

		public int wave { get; private set; }

		public string unitId { get; private set; }

		public int unitLevel { get; private set; }

		public int unitGrade { get; private set; }

		public int unitClass { get; private set; }

		public List<int> skillLevel { get; private set; }

		public int unitScale { get; private set; }

		public string explanation { get; private set; }

		private NPCMercenaryDataRow()
		{
		}

		public string GetKey()
		{
			return id.ToString();
		}
	}
}
