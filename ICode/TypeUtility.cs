using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ICode
{
	public static class TypeUtility
	{
		private static Assembly[] assembliesLookup;

		private static Dictionary<string, Type> typeLookup;

		static TypeUtility()
		{
			assembliesLookup = AppDomain.CurrentDomain.GetAssemblies();
			List<Assembly> list = new List<Assembly>();
			Assembly[] array = assembliesLookup;
			foreach (Assembly assembly in array)
			{
				if (!assembly.GetName().Name.Contains("Editor"))
				{
					list.Add(assembly);
				}
			}
			assembliesLookup = list.ToArray();
			typeLookup = new Dictionary<string, Type>();
		}

		public static string[] GetSubTypeNames(Type baseType)
		{
			return (from x in GetSubTypes(baseType)
				select x.Name).ToArray();
		}

		public static Type[] GetSubTypes(Type baseType)
		{
			IEnumerable<Type> source = from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetTypes())
				where type.IsSubclassOf(baseType)
				select type;
			return source.ToArray();
		}

		public static Type GetMemberType(Type type, string name)
		{
			FieldInfo field = type.GetField(name);
			if (field != null)
			{
				return field.FieldType;
			}
			return type.GetProperty(name)?.PropertyType;
		}

		public static Type GetType(string name)
		{
			Type value = null;
			if (typeLookup.TryGetValue(name, out value))
			{
				return value;
			}
			Assembly[] array = assembliesLookup;
			foreach (Assembly assembly in array)
			{
				Type[] types = assembly.GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					if (types[j].Name == name)
					{
						typeLookup.Add(name, types[j]);
						return types[j];
					}
				}
			}
			return null;
		}

		public static Type[] GetTypeByName(string className)
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					if (types[j].Name == className && !types[j].ToString().Contains("PlayMaker"))
					{
						list.Add(types[j]);
					}
				}
			}
			return list.ToArray();
		}
	}
}
