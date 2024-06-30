using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared
{
	[JsonObject(MemberSerialization.OptIn)]
	public class RaidData
	{
		public static int delayActiveTime = 4000;

		[JsonProperty("rid")]
		internal int _raidId;

		[JsonProperty("stm")]
		internal double _raidStartTime;

		[JsonProperty("etm")]
		internal double _raidEndTime;

		[JsonProperty("ctm")]
		internal double _raidCurTime;

		public int raidId => _raidId;

		public float Amount => (float)((_raidEndTime - _raidStartTime) / (_raidEndTime - _raidCurTime));

		public static RaidData Create(int raidId, double startTime, double endTime, double curTime)
		{
			RaidData raidData = new RaidData();
			raidData._raidId = raidId;
			raidData._raidStartTime = startTime;
			raidData._raidEndTime = endTime;
			raidData._raidCurTime = curTime;
			return raidData;
		}

		public static RaidData Copy(RaidData src)
		{
			if (src == null)
			{
				return null;
			}
			RaidData raidData = new RaidData();
			raidData._raidId = src._raidId;
			raidData._raidStartTime = src._raidStartTime;
			raidData._raidEndTime = src._raidEndTime;
			raidData._raidCurTime = src._raidCurTime;
			return raidData;
		}

		public static explicit operator JToken(RaidData value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			JArray jArray = new JArray();
			jArray.Add(value._raidId);
			jArray.Add(value._raidStartTime);
			jArray.Add(value._raidEndTime);
			jArray.Add(value._raidCurTime);
			return jArray;
		}

		public static explicit operator RaidData(JToken value)
		{
			if (value.Type != JTokenType.Array)
			{
				return null;
			}
			RaidData raidData = new RaidData();
			raidData._raidId = (int)value[0];
			raidData._raidStartTime = (double)value[1];
			raidData._raidEndTime = (double)value[2];
			raidData._raidCurTime = (double)value[3];
			return raidData;
		}
	}
}
