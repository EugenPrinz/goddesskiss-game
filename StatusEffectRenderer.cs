using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectRenderer : MonoBehaviour
{
	public string key;

	private static readonly HashSet<Type> _TimedObjectTypeSet = new HashSet<Type>
	{
		typeof(Animation),
		typeof(Animator),
		typeof(ParticleSystem),
		typeof(Rigidbody),
		typeof(Rigidbody2D),
		typeof(AudioSource)
	};

	public UnityEngine.Object[] FindTimedObjects()
	{
		List<UnityEngine.Object> list = new List<UnityEngine.Object>();
		_FindTimedObjects(base.transform, list);
		return list.ToArray();
	}

	private static void _FindTimedObjects(Transform t, List<UnityEngine.Object> dst)
	{
		Component[] components = t.GetComponents<Component>();
		Component[] array = components;
		foreach (Component component in array)
		{
			if (_TimedObjectTypeSet.Contains(component.GetType()))
			{
				dst.Add(component);
			}
		}
		for (int j = 0; j < t.childCount; j++)
		{
			_FindTimedObjects(t.GetChild(j), dst);
		}
	}
}
