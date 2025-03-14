using System;
using Newtonsoft.Json;

namespace Shared.Regulation
{
	[Serializable]
	[JsonObject]
	public class AttackSwitchEvent
	{
		public enum E_SWITCH_TYPE
		{
			LOCAL,
			TARGET
		}

		public E_SWITCH_TYPE eType;

		public int time { get; private set; }

		public AttackSwitchEvent(int time, E_SWITCH_TYPE etype)
		{
			this.time = time;
			eType = etype;
		}
	}
}
