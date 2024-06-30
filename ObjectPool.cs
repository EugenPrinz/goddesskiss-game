using System.Collections.Generic;

public class ObjectPool<KEY, VALUE>
{
	protected Dictionary<KEY, Stack<VALUE>> datas = new Dictionary<KEY, Stack<VALUE>>();

	public Dictionary<KEY, Stack<VALUE>>.Enumerator Itr => datas.GetEnumerator();

	public virtual void Init()
	{
		datas = new Dictionary<KEY, Stack<VALUE>>();
	}

	public virtual int Count(KEY key)
	{
		if (HasValue(key))
		{
			Stack<VALUE> stack = datas[key];
			return stack.Count;
		}
		return 0;
	}

	public virtual bool HasValue(KEY key)
	{
		if (datas.ContainsKey(key))
		{
			return true;
		}
		return false;
	}

	public virtual VALUE Pop(KEY key)
	{
		if (!HasValue(key))
		{
			return default(VALUE);
		}
		Stack<VALUE> stack = datas[key];
		if (stack.Count <= 0)
		{
			return default(VALUE);
		}
		VALUE result = stack.Pop();
		if (stack.Count <= 0)
		{
			datas.Remove(key);
		}
		return result;
	}

	public virtual void Push(KEY key, VALUE data)
	{
		if (data != null)
		{
			if (!datas.ContainsKey(key))
			{
				datas.Add(key, new Stack<VALUE>());
			}
			datas[key].Push(data);
		}
	}
}
