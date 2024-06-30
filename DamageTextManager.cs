using System.Collections;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
	private static DamageTextManager _singleton;

	public GameObject panel;

	public GameObject prefab;

	private void Awake()
	{
		_singleton = this;
	}

	private void OnDestroy()
	{
		if (_singleton == this)
		{
			_singleton = null;
		}
	}

	public static void Add(Vector3 uiPos, int damage, float delay)
	{
		_singleton.StartCoroutine(_singleton.WaitAndPop(uiPos, damage, delay));
	}

	private IEnumerator WaitAndPop(Vector3 uiPos, int damage, float delay)
	{
		yield return new WaitForSeconds(delay);
		Add(uiPos, damage);
	}

	public static void Add(Vector3 uiPos, int damage)
	{
		GameObject gameObject = NGUITools.AddChild(_singleton.panel, _singleton.prefab);
		gameObject.transform.position = uiPos;
		gameObject.SetActive(value: true);
		UIDamage component = gameObject.GetComponent<UIDamage>();
		component.label.text = damage.ToString();
	}
}
