using UnityEngine;

public class UIBattleUnitPanel : MonoBehaviour
{
	public GameObjectPool pool;

	public Transform targetView;

	public UIBattleUnit CreateUnitUI()
	{
		return pool.Create<UIBattleUnit>(targetView);
	}

	public void Release(UIBattleUnit obj)
	{
		obj.CleanUp();
		pool.Release(obj.gameObject);
	}

	public void Clean()
	{
		while (targetView.childCount > 1)
		{
			Transform child = base.transform.GetChild(1);
			UIBattleUnit component = child.GetComponent<UIBattleUnit>();
			if (component != null)
			{
				component.CleanUp();
				pool.Release(component.gameObject);
			}
			else
			{
				Object.DestroyImmediate(component.gameObject);
			}
		}
	}
}
