using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class SerializedVariable
	{
		public string name;

		[SerializeField]
		private string type;

		public int intValue;

		public float floatValue;

		public UnityEngine.Object objectReferenceValue;

		public string stringValue;

		public Color colorValue;

		public Vector2 vector2Value;

		public Vector3 vector3Value;

		public bool boolValue;

		public GameObject gameObjectValue;

		public Type SerializedType
		{
			get
			{
				return TypeUtility.GetType(type);
			}
			set
			{
				type = value.ToString();
			}
		}

		public static Type[] SupportedTypes => new Type[9]
		{
			typeof(string),
			typeof(bool),
			typeof(Color),
			typeof(float),
			typeof(GameObject),
			typeof(UnityEngine.Object),
			typeof(int),
			typeof(Vector2),
			typeof(Vector3)
		};

		public static string[] DisplayNames => new string[10] { "None", "FsmString", "FsmBool", "FsmColor", "FsmFloat", "FsmGameObject", "FsmObject", "FsmInt", "FsmVector2", "FsmVector3" };

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
			if (serializedType == typeof(GameObject))
			{
				return gameObjectValue;
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
			Type type = value.GetType();
			if (type == typeof(string))
			{
				stringValue = (string)value;
			}
			else if (type == typeof(bool))
			{
				boolValue = (bool)value;
			}
			else if (type == typeof(Color))
			{
				colorValue = (Color)value;
			}
			else if (type == typeof(float))
			{
				floatValue = (float)value;
			}
			else if (type == typeof(GameObject))
			{
				gameObjectValue = (GameObject)value;
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				objectReferenceValue = (UnityEngine.Object)value;
			}
			else if (type == typeof(int))
			{
				intValue = (int)value;
			}
			else if (type == typeof(Vector2))
			{
				vector2Value = (Vector2)value;
			}
			else if (type == typeof(Vector3))
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
			if (mType == typeof(GameObject))
			{
				return "gameObjectValue";
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
