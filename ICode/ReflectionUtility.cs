using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ICode
{
	public static class ReflectionUtility
	{
		private static readonly Dictionary<Type, FieldInfo[]> fieldInfoLookup;

		private static readonly Dictionary<Type, PropertyInfo[]> propertyInfoLookup;

		static ReflectionUtility()
		{
			fieldInfoLookup = new Dictionary<Type, FieldInfo[]>();
			propertyInfoLookup = new Dictionary<Type, PropertyInfo[]>();
		}

		public static string[] GetAllComponentNames()
		{
			IEnumerable<Type> source = from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetTypes())
				where type.IsSubclassOf(typeof(Component))
				select type;
			return source.Select((Type x) => x.FullName).ToArray();
		}

		public static string[] GetFieldNames(this Type type)
		{
			FieldInfo[] source = type.GetAllFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			source = source.Where((FieldInfo x) => FsmUtility.GetVariableType(x.FieldType) != null).ToArray();
			return source.Select((FieldInfo x) => x.Name).ToArray();
		}

		public static string[] GetPropertyNames(this Type type, bool requiresWrite)
		{
			PropertyInfo[] source = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToArray();
			if (requiresWrite)
			{
				source = source.Where((PropertyInfo x) => x.CanWrite && FsmUtility.GetVariableType(x.PropertyType) != null).ToArray();
			}
			return source.Select((PropertyInfo x) => x.Name).ToArray();
		}

		public static string[] GetPropertyAndFieldNames(this Type type, bool requiresWrite)
		{
			List<string> list = new List<string>(type.GetPropertyNames(requiresWrite));
			list.AddRange(type.GetFieldNames());
			return list.ToArray();
		}

		public static string[] GetMethodNames(this Type type)
		{
			MethodInfo[] source = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToArray();
			return (from y in source
				where y.GetParameters().Length == 0 && y.ReturnType == typeof(void)
				select y into x
				select x.Name).ToArray();
		}

		public static FieldInfo[] GetAllFields(this Type type, BindingFlags flags)
		{
			if (type == null)
			{
				return new FieldInfo[0];
			}
			return type.GetFields(flags).Concat(type.BaseType.GetAllFields(flags)).ToArray();
		}

		public static FieldInfo[] GetPublicFields(this object obj)
		{
			return obj.GetType().GetPublicFields();
		}

		public static FieldInfo[] GetPublicFields(this Type type)
		{
			if (!fieldInfoLookup.ContainsKey(type))
			{
				fieldInfoLookup[type] = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			}
			return fieldInfoLookup[type];
		}

		public static PropertyInfo[] GetPublicProperties(this object obj)
		{
			return obj.GetType().GetPublicProperties();
		}

		public static PropertyInfo[] GetPublicProperties(this Type type)
		{
			if (!propertyInfoLookup.ContainsKey(type))
			{
				propertyInfoLookup[type] = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			}
			return propertyInfoLookup[type];
		}

		public static FieldInfo[] GetFields(this Type type)
		{
			return type.GetFields();
		}

		public static FieldInfo GetField(this Type type, string name)
		{
			return type.GetField(name);
		}

		public static PropertyInfo GetProperty(this Type type, string name)
		{
			return type.GetProperty(name);
		}

		public static bool IsSubclassOf(this Type type, Type c)
		{
			return type.IsSubclassOf(c);
		}
	}
}
