using UnityEngine;

public class UIListItem : ListItemBase
{
	public int Index;

	public GameObject Target;

	public UIListItem()
	{
		Index = -1;
		Target = null;
	}

	public void SetIndex(int index)
	{
		if (Index != index)
		{
			Index = index;
			if (Target != null)
			{
				cUIScrollListBase component = Target.GetComponent<cUIScrollListBase>();
				component.ListItem = this;
			}
		}
	}
}
