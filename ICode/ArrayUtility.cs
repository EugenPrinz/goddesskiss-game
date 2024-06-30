using System.Collections.Generic;

namespace ICode
{
	public static class ArrayUtility
	{
		public static T[] Add<T>(T[] array, T item)
		{
			List<T> list = new List<T>(array);
			list.Add(item);
			return list.ToArray();
		}

		public static T[] AddRange<T>(T[] array, T[] items)
		{
			List<T> list = new List<T>(array);
			list.AddRange(items);
			return list.ToArray();
		}

		public static T[] Copy<T>(T[] array)
		{
			return new List<T>(array).ToArray();
		}

		public static T[] MoveItem<T>(T[] array, int oldIndex, int newIndex)
		{
			List<T> list = new List<T>(array);
			T item = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, item);
			return list.ToArray();
		}

		public static T[] Remove<T>(T[] array, T item)
		{
			List<T> list = new List<T>(array);
			list.Remove(item);
			return list.ToArray();
		}

		public static T[] RemoveAt<T>(T[] array, int index)
		{
			List<T> list = new List<T>(array);
			list.RemoveAt(index);
			return list.ToArray();
		}

		public static T[] Insert<T>(T[] array, T item, int index)
		{
			List<T> list = new List<T>(array);
			list.Insert(index, item);
			return list.ToArray();
		}

		public static T[] Sort<T>(T[] array)
		{
			List<T> list = new List<T>(array);
			list.Sort();
			return list.ToArray();
		}

		public static T[] Sort<T>(T[] array, IComparer<T> comparer)
		{
			List<T> list = new List<T>(array);
			list.Sort(comparer);
			return list.ToArray();
		}

		public static T[] Reverse<T>(T[] array)
		{
			List<T> list = new List<T>(array);
			list.Reverse();
			return list.ToArray();
		}
	}
}
