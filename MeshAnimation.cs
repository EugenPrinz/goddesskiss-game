using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnimation : MonoBehaviour
{
	public class Key : IEquatable<Key>
	{
		public readonly Mesh mesh;

		public readonly string name;

		public readonly int sampleRate;

		public Key(Mesh mesh, string name, int sampleRate)
		{
			this.mesh = mesh;
			this.name = name;
			this.sampleRate = sampleRate;
		}

		public override string ToString()
		{
			return $"0x{mesh.GetInstanceID():X8}-{name}-{sampleRate}";
		}

		public override int GetHashCode()
		{
			int hashCode = mesh.GetHashCode();
			int hashCode2 = name.GetHashCode();
			int hashCode3 = sampleRate.GetHashCode();
			return hashCode ^ hashCode2 ^ hashCode3;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Key);
		}

		public bool Equals(Key key)
		{
			if (key == null || !object.ReferenceEquals(mesh, key.mesh))
			{
				return false;
			}
			if (name != key.name || sampleRate != key.sampleRate)
			{
				return false;
			}
			return true;
		}
	}

	public class Baker
	{
		public readonly Animation animation;

		public readonly SkinnedMeshRenderer renderer;

		public Baker(Animation anim, SkinnedMeshRenderer smr)
		{
			animation = anim;
			renderer = smr;
		}
	}

	public const int MinSampleRate = 15;

	public const int MaxSampleRate = 60;

	private static Dictionary<Mesh, Baker> _bakerCache = new Dictionary<Mesh, Baker>();

	private static Dictionary<Key, Mesh[]> _frameCache = new Dictionary<Key, Mesh[]>();

	private static float _cacheClearedTime = 0f;

	private float _initializedTime = -1f;

	private Animation _animation;

	private SkinnedMeshRenderer _skinnedMeshRenderer;

	private int _sampleRate;

	private Dictionary<string, Key> _keyDict;

	private MeshFilter _meshFilter;

	private MeshRenderer _meshRenderer;

	public Animation animation => _animation;

	public int sampleRate => _sampleRate;

	public MeshFilter meshFilter => _meshFilter;

	public MeshRenderer meshRenderer => _meshRenderer;

	public bool isInitialized
	{
		get
		{
			if (_initializedTime < _cacheClearedTime)
			{
				return false;
			}
			return _animation != null;
		}
	}

	public static void ClearCache()
	{
		_cacheClearedTime = Time.realtimeSinceStartup;
		foreach (Baker value in _bakerCache.Values)
		{
			GameObject obj = value.animation.gameObject;
			UnityEngine.Object.Destroy(obj);
		}
		foreach (Mesh[] value2 in _frameCache.Values)
		{
			Mesh[] array = value2;
			foreach (Mesh obj2 in array)
			{
				UnityEngine.Object.Destroy(obj2);
			}
		}
		_bakerCache = new Dictionary<Mesh, Baker>();
		_frameCache = new Dictionary<Key, Mesh[]>();
	}

	public static MeshAnimation[] AddToAnimation(Animation anim, int sampleRate, int depth = -1, params string[] prebakingClipNames)
	{
		if (anim == null)
		{
			throw new ArgumentNullException("anim");
		}
		if (prebakingClipNames == null)
		{
			prebakingClipNames = new string[0];
		}
		List<MeshAnimation> list = new List<MeshAnimation>();
		_AddToAnimation(anim, sampleRate, depth, prebakingClipNames, list);
		foreach (MeshAnimation item in list)
		{
			item._Initialize(prebakingClipNames);
		}
		return list.ToArray();
	}

	private static void _AddToAnimation(Animation anim, int sampleRate, int depth, string[] prebakingClipNames, List<MeshAnimation> list)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = _FindComponentInChildren<SkinnedMeshRenderer>(anim);
		if (skinnedMeshRenderer == null)
		{
			return;
		}
		MeshAnimation component = anim.GetComponent<MeshAnimation>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		component = anim.gameObject.AddComponent<MeshAnimation>();
		component._sampleRate = sampleRate;
		list.Add(component);
		if (depth != 0)
		{
			Animation[] array = _FindComponentsInChildren<Animation>(anim);
			Animation[] array2 = array;
			foreach (Animation anim2 in array2)
			{
				_AddToAnimation(anim2, sampleRate, depth - 1, prebakingClipNames, list);
			}
		}
	}

	private void Start()
	{
		if (!isInitialized)
		{
			_Initialize(new string[0]);
		}
	}

	private void _Initialize(string[] prebakingClipNames)
	{
		Animation component = GetComponent<Animation>();
		if (component == null)
		{
			throw new MissingComponentException("Animation");
		}
		SkinnedMeshRenderer skinnedMeshRenderer = _FindComponentInChildren<SkinnedMeshRenderer>(component);
		if (skinnedMeshRenderer == null)
		{
			throw new MissingComponentException("SkinnedMeshRenderer");
		}
		if (skinnedMeshRenderer.sharedMesh == null)
		{
			string message = "SkinnedMeshRenderer.sharedMesh";
			throw new MissingReferenceException(message);
		}
		if (skinnedMeshRenderer.rootBone == null)
		{
			string message2 = "SkinnedMeshRenderer.rootBone";
			throw new MissingReferenceException(message2);
		}
		_sampleRate = Mathf.Clamp(_sampleRate, 15, 60);
		if (!_bakerCache.TryGetValue(skinnedMeshRenderer.sharedMesh, out var value))
		{
			value = _CreateBaker(component);
			_bakerCache.Add(skinnedMeshRenderer.sharedMesh, value);
		}
		Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
		Dictionary<string, Key> dictionary = _CreateKeyDict(component, skinnedMeshRenderer.sharedMesh, _sampleRate);
		foreach (Key value4 in dictionary.Values)
		{
			if (!_frameCache.TryGetValue(value4, out var value2))
			{
				int num = Mathf.CeilToInt(component[value4.name].length * (float)_sampleRate);
				value2 = new Mesh[num];
				value2[0] = _CreateFrame(value4, 0);
				_frameCache.Add(value4, value2);
			}
			if (value4.name == component.clip.name)
			{
				sharedMesh = value2[0];
			}
		}
		foreach (string key in prebakingClipNames)
		{
			if (!dictionary.TryGetValue(key, out var value3))
			{
				continue;
			}
			Mesh[] array = _frameCache[value3];
			for (int j = 1; j < array.Length; j++)
			{
				if (array[j] == null)
				{
					array[j] = _CreateFrame(value3, j);
				}
			}
		}
		_animation = component;
		_skinnedMeshRenderer = skinnedMeshRenderer;
		_keyDict = dictionary;
		_meshFilter = skinnedMeshRenderer.gameObject.GetComponent<MeshFilter>();
		if (_meshFilter == null)
		{
			_meshFilter = skinnedMeshRenderer.gameObject.AddComponent<MeshFilter>();
		}
		_meshRenderer = skinnedMeshRenderer.gameObject.GetComponent<MeshRenderer>();
		if (_meshRenderer == null)
		{
			_meshRenderer = skinnedMeshRenderer.gameObject.AddComponent<MeshRenderer>();
		}
		_skinnedMeshRenderer.enabled = false;
		_skinnedMeshRenderer.rootBone.gameObject.SetActive(value: false);
		_meshFilter.sharedMesh = sharedMesh;
		_RefreshMeshRenderer();
		_initializedTime = Time.realtimeSinceStartup;
	}

	private void LateUpdate()
	{
		if (!_meshRenderer.isVisible)
		{
			return;
		}
		if (!isInitialized)
		{
			_Initialize(new string[0]);
		}
		AnimationState animationState = _GetPlayingMaxWeightAnimationState();
		if (_animation.enabled && !(animationState == null))
		{
			float num = animationState.normalizedTime % 1f;
			int num2 = (int)(num * animationState.length / (1f / (float)_sampleRate));
			Key key = _keyDict[animationState.name];
			Mesh[] array = _frameCache[key];
			if (array[num2] == null)
			{
				array[num2] = _CreateFrame(key, num2);
			}
			if (_meshFilter.sharedMesh != array[num2])
			{
				_meshFilter.sharedMesh = array[num2];
				_RefreshMeshRenderer();
			}
		}
	}

	private void _RefreshMeshRenderer()
	{
		MeshRenderer meshRenderer = _meshRenderer;
		SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderer;
		meshRenderer.useLightProbes = skinnedMeshRenderer.useLightProbes;
		meshRenderer.lightmapIndex = skinnedMeshRenderer.lightmapIndex;
		meshRenderer.lightmapScaleOffset = skinnedMeshRenderer.lightmapScaleOffset;
		meshRenderer.realtimeLightmapScaleOffset = skinnedMeshRenderer.realtimeLightmapScaleOffset;
		meshRenderer.reflectionProbeUsage = skinnedMeshRenderer.reflectionProbeUsage;
		meshRenderer.receiveShadows = skinnedMeshRenderer.receiveShadows;
		meshRenderer.shadowCastingMode = skinnedMeshRenderer.shadowCastingMode;
		meshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
	}

	private static T _FindComponentInChildren<T>(Component c) where T : Component
	{
		Transform transform = c.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			T val = child.GetComponent(typeof(T)) as T;
			if (val != null)
			{
				return val;
			}
		}
		return (T)null;
	}

	private static T[] _FindComponentsInChildren<T>(Component c) where T : Component
	{
		Transform transform = c.transform;
		List<T> list = new List<T>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			T val = child.GetComponent(typeof(T)) as T;
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}

	private AnimationState _GetPlayingMaxWeightAnimationState()
	{
		AnimationState animationState = null;
		foreach (AnimationState item in _animation)
		{
			if (_animation.IsPlaying(item.name) && (animationState == null || animationState.weight < item.weight))
			{
				animationState = item;
			}
		}
		return animationState;
	}

	private static Dictionary<string, Key> _CreateKeyDict(Animation anim, Mesh mesh, int fps)
	{
		Dictionary<string, Key> dictionary = new Dictionary<string, Key>();
		foreach (AnimationState item in anim)
		{
			Key key = new Key(mesh, item.name, fps);
			dictionary.Add(key.name, key);
		}
		return dictionary;
	}

	private static Baker _CreateBaker(Animation src)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(src.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.one;
		Animation component = gameObject.GetComponent<Animation>();
		SkinnedMeshRenderer skinnedMeshRenderer = _FindComponentInChildren<SkinnedMeshRenderer>(component);
		if (skinnedMeshRenderer == null)
		{
			throw new MissingReferenceException(component.name);
		}
		return new Baker(component, skinnedMeshRenderer);
	}

	private static Mesh _CreateFrame(Key key, int num)
	{
		Baker baker = _bakerCache[key.mesh];
		baker.animation.Play(key.name, PlayMode.StopAll);
		AnimationState animationState = baker.animation[key.name];
		animationState.normalizedTime = (float)num / (animationState.length / (1f / (float)key.sampleRate));
		baker.animation.Sample();
		Mesh mesh = new Mesh();
		UnityEngine.Object.DontDestroyOnLoad(mesh);
		mesh.hideFlags = HideFlags.HideInHierarchy;
		mesh.name = $"F{num:D2}-{key.ToString()}";
		baker.renderer.BakeMesh(mesh);
		return mesh;
	}
}
