using System.Collections;
using UnityEngine;

public class SortingOrderSetter : MonoBehaviour
{
	public bool children;

	public bool maintain;

	public int sortingOrder;

	private void Start()
	{
		if (children)
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].sortingOrder = sortingOrder;
			}
		}
		else
		{
			GetComponent<Renderer>().sortingOrder = sortingOrder;
		}
		if (maintain)
		{
			StartCoroutine(_Update());
		}
		else
		{
			base.enabled = false;
		}
	}

	private IEnumerator _Update()
	{
		while (true)
		{
			if (children)
			{
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].sortingOrder = sortingOrder;
				}
			}
			else
			{
				GetComponent<Renderer>().sortingOrder = sortingOrder;
			}
			yield return new WaitForSeconds(0.2f);
		}
	}
}
