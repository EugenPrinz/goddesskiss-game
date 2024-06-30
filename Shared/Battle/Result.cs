using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Battle
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Result
	{
		[JsonProperty("cksm")]
		internal string _checksum;

		[JsonProperty("tmot")]
		internal bool _isTimeOut;

		[JsonProperty("wsid")]
		internal int _winSide;

		[JsonProperty("gld")]
		internal int _gold;

		[JsonProperty("rnk")]
		internal int _clearRank;

		[JsonProperty("amkn")]
		internal int _armyDestoryCnt;

		[JsonProperty("amckn")]
		internal int _armyCmdDestoryCnt;

		[JsonProperty("nvkn")]
		internal int _navyDestoryCnt;

		[JsonProperty("nvckn")]
		internal int _navyCmdDestoryCnt;

		[JsonProperty("tadm")]
		internal long _totalAttackDamage;

		[JsonProperty("ltrp")]
		internal List<Troop> _lhsTroops;

		[JsonProperty("rtrp")]
		internal List<Troop> _rhsTroops;

		public string checksum => _checksum;

		public bool isTimeOut => _isTimeOut;

		public int winSide => _winSide;

		public int clearRank => _clearRank;

		public int gold => _gold;

		public int armyDestoryCnt => _armyDestoryCnt;

		public int armyCmdDestoryCnt => _armyCmdDestoryCnt;

		public int navyDestoryCnt => _navyDestoryCnt;

		public int navyCmdDestoryCnt => _navyCmdDestoryCnt;

		public long totalAttackDamage => _totalAttackDamage;

		public IList<Troop> lhsTroops => _lhsTroops.AsReadOnly();

		public IList<Troop> rhsTroops => _rhsTroops.AsReadOnly();

		public Troop victoryTroop
		{
			get
			{
				if (_winSide < 0)
				{
					foreach (Troop lhsTroop in _lhsTroops)
					{
						if (!lhsTroop.isAnnihilated)
						{
							return lhsTroop;
						}
					}
				}
				if (_winSide > 0)
				{
					foreach (Troop rhsTroop in _rhsTroops)
					{
						if (!rhsTroop.isAnnihilated)
						{
							return rhsTroop;
						}
					}
				}
				return null;
			}
		}

		public bool isLhsAnnihilated
		{
			get
			{
				foreach (Troop lhsTroop in _lhsTroops)
				{
					if (lhsTroop.isAnnihilated)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool isRhsAnnihilated
		{
			get
			{
				foreach (Troop rhsTroop in _rhsTroops)
				{
					if (rhsTroop.isAnnihilated)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsWin => _winSide < 0;

		internal Result()
		{
		}

		public static explicit operator JToken(Result value)
		{
			JArray jArray = new JArray();
			jArray.Add(value._checksum);
			jArray.Add(value._isTimeOut);
			jArray.Add(value._winSide);
			jArray.Add(value._gold);
			jArray.Add(value._clearRank);
			jArray.Add(value._armyDestoryCnt);
			jArray.Add(value._armyCmdDestoryCnt);
			jArray.Add(value._navyDestoryCnt);
			jArray.Add(value._navyCmdDestoryCnt);
			jArray.Add(value._totalAttackDamage);
			JArray jArray2 = new JArray();
			for (int i = 0; i < value._lhsTroops.Count; i++)
			{
				jArray2.Add((JToken)value._lhsTroops[i]);
			}
			jArray.Add(jArray2);
			JArray jArray3 = new JArray();
			for (int j = 0; j < value._rhsTroops.Count; j++)
			{
				jArray3.Add((JToken)value._rhsTroops[j]);
			}
			jArray.Add(jArray3);
			return jArray;
		}

		public static explicit operator Result(JToken value)
		{
			Result result = new Result();
			result._checksum = (string)value[0];
			result._isTimeOut = (bool)value[1];
			result._winSide = (int)value[2];
			result._gold = (int)value[3];
			result._clearRank = (int)value[4];
			result._armyDestoryCnt = (int)value[5];
			result._armyCmdDestoryCnt = (int)value[6];
			result._navyDestoryCnt = (int)value[7];
			result._navyCmdDestoryCnt = (int)value[8];
			result._totalAttackDamage = (long)value[9];
			result._lhsTroops = new List<Troop>();
			JArray jArray = (JArray)value[10];
			for (int i = 0; i < jArray.Count; i++)
			{
				result._lhsTroops.Add((Troop)jArray[i]);
			}
			result._rhsTroops = new List<Troop>();
			JArray jArray2 = (JArray)value[11];
			for (int j = 0; j < jArray2.Count; j++)
			{
				result._rhsTroops.Add((Troop)jArray2[j]);
			}
			return result;
		}
	}
}
