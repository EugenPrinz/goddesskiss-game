using UnityEngine;

namespace RoomDecorator
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = (T)Object.FindObjectOfType(typeof(T));
					if (_instance == null)
					{
						_instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
					}
				}
				return _instance;
			}
		}

		public static bool exist => _instance != null;

		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = (T)this;
			}
			else if (_instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = (T)null;
			}
		}
	}
}
