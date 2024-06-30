using UnityEngine;

public class CreateUnitCacheItem : CreateCacheItem
{
	public bool useUnitRenderer;

	protected override void Create()
	{
		base.Create();
		if (!(targetItem != null))
		{
			return;
		}
		targetItem.transform.localScale = Vector3.one;
		if (!useUnitRenderer)
		{
			UnitRenderer component = targetItem.GetComponent<UnitRenderer>();
			if (component != null)
			{
				component.enabled = false;
			}
			Collider component2 = targetItem.GetComponent<Collider>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}
	}
}
