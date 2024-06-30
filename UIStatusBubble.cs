using System.Collections;
using UnityEngine;

public class UIStatusBubble : MonoBehaviour
{
	public GameObjectPool pool;

	public GameObject panel;

	private void OnEnable()
	{
		while (base.transform.childCount > 1)
		{
			Transform child = base.transform.GetChild(1);
			pool.Release(child.gameObject);
		}
	}

	public void Add(Vector3 uiPos, long damage, float delay)
	{
		StartCoroutine(WaitAndPop(uiPos, damage, delay));
	}

	private IEnumerator WaitAndPop(Vector3 uiPos, long damage, float delay)
	{
		yield return new WaitForSeconds(delay);
		Add(uiPos, damage);
	}

	public void Add(Vector3 uiPos, long damage)
	{
		GameObject gameObject = _Create(uiPos);
		UIStatusBubbleItem component = gameObject.GetComponent<UIStatusBubbleItem>();
		component.Set(damage.ToString());
	}

	public void Add(Vector3 uiPos, long damage, Color color)
	{
		GameObject gameObject = _Create(uiPos);
		UIStatusBubbleItem component = gameObject.GetComponent<UIStatusBubbleItem>();
		component.Set(damage.ToString());
		TweenColor component2 = component.damage.gameObject.GetComponent<TweenColor>();
		component2.to = color;
	}

	public void Add(Vector3 uiPos)
	{
		_Create(uiPos);
	}

	protected GameObject _Create(Vector3 pos)
	{
		GameObject gameObject = pool.Create(panel.transform);
		gameObject.transform.position = pos;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	protected GameObject _Create()
	{
		GameObject gameObject = pool.Create(panel.transform);
		gameObject.SetActive(value: true);
		return gameObject;
	}
}
