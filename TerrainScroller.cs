using Cache;
using UnityEngine;

public class TerrainScroller : MonoBehaviour
{
	public const string prefabRoot = "Assets/Prefabs/Terrains";

	public Transform cache;

	public string theme;

	[Tooltip("이동은 스크롤러 기준 +Z방향")]
	[Range(-10f, 10f)]
	public float speed;

	private string _theme;

	private TimedGameObject _tgo;

	private void Start()
	{
		_tgo = GetComponent<TimedGameObject>();
	}

	private static Vector3 _GetTerrainTailPosition(Transform terrain)
	{
		Transform transform = terrain.Find("_TAIL");
		if (transform == null)
		{
			transform = terrain;
		}
		return transform.position;
	}

	private static float _GetTerrainLength(Transform terrain)
	{
		Vector3 vector = _GetTerrainTailPosition(terrain);
		return (vector - terrain.position).magnitude;
	}

	private void _MoveTerrain(Transform terrain, float d)
	{
		Vector3 fromDirection = terrain.position - _GetTerrainTailPosition(terrain);
		Vector3 forward = base.transform.forward;
		terrain.rotation = Quaternion.FromToRotation(fromDirection, forward);
		terrain.position += d * base.transform.forward;
	}

	private void _ArrangeTerrains()
	{
		for (int i = 1; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i - 1);
			Transform child2 = base.transform.GetChild(i);
			child2.transform.position = _GetTerrainTailPosition(child);
			_MoveTerrain(child2, 0f);
		}
	}

	private bool _LoadTheme(string theme)
	{
		if (SplitScreenManager.instance.left == null || SplitScreenManager.instance.right == null)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		if (SplitScreenManager.instance.right.terrainScroller == this)
		{
			flag2 = true;
		}
		char c = theme[theme.Length - 1];
		if (c == '0')
		{
			flag = true;
		}
		int num = 3;
		_theme = null;
		_RemoveAllChildren();
		while (base.transform.childCount < num)
		{
			GameObject gameObject = CacheManager.instance.TerrainCache.Create(theme);
			if (flag2)
			{
				Transform transform = gameObject.transform.Find("_MAP");
				if (transform != null)
				{
					transform.transform.localScale = new Vector3(transform.transform.localScale.x, transform.transform.localScale.y, 0f - transform.transform.localScale.z);
				}
			}
			Vector3 localScale = gameObject.transform.localScale;
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = localScale;
		}
		Transform child = base.transform.GetChild(0);
		child.transform.localPosition = Vector3.zero;
		float magnitude = _GetTerrainTailPosition(child).magnitude;
		_MoveTerrain(child, magnitude);
		_ArrangeTerrains();
		_theme = theme;
		return true;
	}

	private void _RemoveAllChildren()
	{
		while (base.transform.childCount > 0)
		{
			Transform child = base.transform.GetChild(base.transform.childCount - 1);
			child.parent = null;
			Object.DestroyImmediate(child.gameObject);
		}
	}

	private void Update()
	{
		if (!string.IsNullOrEmpty(_theme) || (!string.IsNullOrEmpty(theme) && _LoadTheme(theme)))
		{
			Transform child = base.transform.GetChild(0);
			float num = _GetTerrainLength(child);
			float d = _tgo.TimedDeltaTime() * speed;
			_MoveTerrain(child, d);
			_ArrangeTerrains();
			float magnitude = child.transform.localPosition.magnitude;
			if (magnitude > 1.5f * num)
			{
				child.SetAsLastSibling();
			}
			else if (magnitude < 0.5f * num)
			{
				Transform child2 = base.transform.GetChild(base.transform.childCount - 1);
				child2.SetAsFirstSibling();
				child2.position = child.position;
				_MoveTerrain(child2, _GetTerrainLength(child2));
			}
		}
	}
}
