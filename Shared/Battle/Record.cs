using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Battle
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Record
	{
		[JsonProperty("smvr")]
		internal int _simulatorVersion;

		[JsonProperty("rgvr")]
		internal double _regulationVersion;

		[JsonProperty("optn")]
		internal Option _option;

		[JsonProperty("init")]
		internal InitState _initState;

		[JsonProperty("flen")]
		internal int _length;

		[JsonProperty("lipt")]
		internal Dictionary<int, Input> _lhsInputMap;

		[JsonProperty("ript")]
		internal Dictionary<int, Input> _rhsInputMap;

		[JsonIgnore]
		internal List<Frame> _frames;

		[JsonIgnore]
		internal Result _result;

		public int simulatorVersion => _simulatorVersion;

		public double regulationVersion => _regulationVersion;

		public Option option => _option;

		public InitState initState => _initState;

		public int length => _length;

		public IList<Frame> frames => _frames.AsReadOnly();

		public Result result => _result;

		internal Record()
		{
		}

		public bool HasLhsInput(int frameNum)
		{
			return _lhsInputMap.ContainsKey(frameNum);
		}

		public Input GetLhsInput(int frameNum)
		{
			return _lhsInputMap[frameNum];
		}

		public bool HasRhsInput(int frameNum)
		{
			return _rhsInputMap.ContainsKey(frameNum);
		}

		public Input GetRhsInput(int frameNum)
		{
			return _rhsInputMap[frameNum];
		}

		public static Record Copy(Record src)
		{
			Record record = new Record();
			record._simulatorVersion = src._simulatorVersion;
			record._regulationVersion = src._regulationVersion;
			record._option = src._option;
			record._initState = InitState.Copy(src._initState);
			record._lhsInputMap = new Dictionary<int, Input>();
			foreach (KeyValuePair<int, Input> item in src._lhsInputMap)
			{
				record._lhsInputMap.Add(item.Key, Input.Copy(item.Value));
			}
			record._rhsInputMap = new Dictionary<int, Input>();
			foreach (KeyValuePair<int, Input> item2 in src._rhsInputMap)
			{
				record._rhsInputMap.Add(item2.Key, Input.Copy(item2.Value));
			}
			record._frames = new List<Frame>();
			record._length = src._length;
			record._result = null;
			return record;
		}

		public static explicit operator JToken(Record value)
		{
			JArray jArray = new JArray();
			jArray.Add(value._simulatorVersion);
			jArray.Add(value._regulationVersion);
			jArray.Add((JToken)value._option);
			jArray.Add((JToken)value._initState);
			jArray.Add(value._length);
			if (value._lhsInputMap.Count > 0)
			{
				JArray jArray2 = new JArray();
				Dictionary<int, Input>.Enumerator enumerator = value._lhsInputMap.GetEnumerator();
				while (enumerator.MoveNext())
				{
					JArray jArray3 = new JArray();
					jArray3.Add(enumerator.Current.Key);
					jArray3.Add(enumerator.Current.Value._unitIndex);
					jArray3.Add(enumerator.Current.Value._skillIndex);
					jArray3.Add(enumerator.Current.Value._targetIndex);
					jArray2.Add(jArray3);
				}
				jArray.Add(jArray2);
			}
			else
			{
				jArray.Add(string.Empty);
			}
			if (value._rhsInputMap.Count > 0)
			{
				JArray jArray4 = new JArray();
				Dictionary<int, Input>.Enumerator enumerator2 = value._rhsInputMap.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					JArray jArray5 = new JArray();
					jArray5.Add(enumerator2.Current.Key);
					jArray5.Add(enumerator2.Current.Value._unitIndex);
					jArray5.Add(enumerator2.Current.Value._skillIndex);
					jArray5.Add(enumerator2.Current.Value._targetIndex);
					jArray4.Add(jArray5);
				}
				jArray.Add(jArray4);
			}
			else
			{
				jArray.Add(string.Empty);
			}
			return jArray;
		}

		public static explicit operator Record(JToken value)
		{
			Record record = new Record();
			record._simulatorVersion = (int)value[0];
			record._regulationVersion = (double)value[1];
			record._option = (Option)value[2];
			record._initState = (InitState)value[3];
			record._length = (int)value[4];
			record._lhsInputMap = new Dictionary<int, Input>();
			if (value[5].Type == JTokenType.Array)
			{
				JArray jArray = (JArray)value[5];
				for (int i = 0; i < jArray.Count; i++)
				{
					JArray jArray2 = (JArray)jArray[i];
					record._lhsInputMap.Add((int)jArray2[0], new Input((int)jArray2[1], (int)jArray2[2], (int)jArray2[3]));
				}
			}
			record._rhsInputMap = new Dictionary<int, Input>();
			if (value[6].Type == JTokenType.Array)
			{
				JArray jArray3 = (JArray)value[6];
				for (int j = 0; j < jArray3.Count; j++)
				{
					JArray jArray4 = (JArray)jArray3[j];
					record._rhsInputMap.Add((int)jArray4[0], new Input((int)jArray4[1], (int)jArray4[2], (int)jArray4[3]));
				}
			}
			return record;
		}
	}
}
