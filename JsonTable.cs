using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonTable<T> : IEnumerable<T>, IEnumerable
{
	public class Converter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JsonTable<T> jsonTable = (JsonTable<T>)value;
			serializer.Serialize(writer, jsonTable._list);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return new JsonTable<T>(reader);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(JsonTable<T>);
		}
	}

	private List<T> _list;

	private Dictionary<string, int> _dict;

	public T this[int index] => _list[index];

	public T this[string key] => Find(key);

	public int length => _list.Count;

	private JsonTable(JsonReader reader)
	{
		JArray jArray = JArray.Load(reader);
		_list = new List<T>();
		_dict = new Dictionary<string, int>();
		foreach (JToken item in (IEnumerable<JToken>)jArray)
		{
			_list.Add(item.ToObject<T>());
			string key = item.First.First.ToString();
			if (_dict.ContainsKey(key))
			{
				_list.Clear();
				_dict.Clear();
				throw new ArgumentException("row key must be unique.");
			}
			_dict.Add(key, _list.Count - 1);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool ContainsKey(string key)
	{
		return _dict.ContainsKey(key);
	}

	public int FindIndex(string key)
	{
		return _dict[key];
	}

	public T Find(string key)
	{
		return _list[FindIndex(key)];
	}

	public JArray ToJsonArray()
	{
		return JArray.FromObject(_list);
	}

	public void Add(T row)
	{
		throw new NotImplementedException();
	}

	public void Insert(T row, int index)
	{
		throw new NotImplementedException();
	}

	public void Remove(T row)
	{
		throw new NotImplementedException();
	}

	public void Remove(int index)
	{
		throw new NotImplementedException();
	}

	public void ForEach(Action<T> action)
	{
		_list.ForEach(action);
	}
}
