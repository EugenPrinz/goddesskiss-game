using System;
using UnityEngine;

namespace Unitycoding
{
	[Serializable]
	public class ObjectProperty : INameable
	{
		[SerializeField]
		private string name;

		[SerializeField]
		private int typeIndex;

		public string stringValue;

		public int intValue;

		public float floatValue;

		public Color colorValue;

		public bool boolValue;

		public UnityEngine.Object objectReferenceValue;

		public Vector2 vector2Value;

		public Vector3 vector3Value;

		public bool show;

		public Color color = Color.white;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public Type SerializedType => SupportedTypes[typeIndex];

		public static Type[] SupportedTypes => new Type[8]
		{
			typeof(string),
			typeof(bool),
			typeof(Color),
			typeof(float),
			typeof(UnityEngine.Object),
			typeof(int),
			typeof(Vector2),
			typeof(Vector3)
		};

		public static string[] DisplayNames => new string[8] { "String", "Bool", "Color", "Float", "Object", "Int", "Vector2", "Vector3" };

		public object GetValue()
		{
			Type serializedType = SerializedType;
			if (serializedType == null)
			{
				return null;
			}
			if (serializedType == typeof(string))
			{
				return stringValue;
			}
			if (serializedType == typeof(bool))
			{
				return boolValue;
			}
			if (serializedType == typeof(Color))
			{
				return colorValue;
			}
			if (serializedType == typeof(float))
			{
				return floatValue;
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(serializedType))
			{
				return objectReferenceValue;
			}
			if (serializedType == typeof(int))
			{
				return intValue;
			}
			if (serializedType == typeof(Vector2))
			{
				return vector2Value;
			}
			if (serializedType == typeof(Vector3))
			{
				return vector3Value;
			}
			return null;
		}

		public void SetValue(object value)
		{
			if (value is string)
			{
				stringValue = (string)value;
			}
			else if (value is bool)
			{
				boolValue = (bool)value;
			}
			else if (value is Color)
			{
				colorValue = (Color)value;
			}
			else if (value is float || value is double)
			{
				floatValue = Convert.ToSingle(value);
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(value.GetType()))
			{
				objectReferenceValue = (UnityEngine.Object)value;
			}
			else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
			{
				intValue = Convert.ToInt32(value);
			}
			else if (value is Vector2)
			{
				vector2Value = (Vector2)value;
			}
			else if (value is Vector3)
			{
				vector3Value = (Vector3)value;
			}
		}

		public static string GetPropertyName(Type mType)
		{
			if (mType == typeof(string))
			{
				return "stringValue";
			}
			if (mType == typeof(bool))
			{
				return "boolValue";
			}
			if (mType == typeof(Color))
			{
				return "colorValue";
			}
			if (mType == typeof(float))
			{
				return "floatValue";
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(mType))
			{
				return "objectReferenceValue";
			}
			if (mType == typeof(int))
			{
				return "intValue";
			}
			if (mType == typeof(Vector2))
			{
				return "vector2Value";
			}
			if (mType == typeof(Vector3))
			{
				return "vector3Value";
			}
			return string.Empty;
		}
	}
}
