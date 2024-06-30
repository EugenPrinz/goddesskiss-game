using System.Collections.Generic;

namespace Step
{
	public class SetWidgetDepthSorting : AbstractStepAction
	{
		public GameObjectData target;

		public IntData depth;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			if (depth == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			UIWidget[] componentsInChildren = target.value.GetComponentsInChildren<UIWidget>();
			if (componentsInChildren.Length > 0)
			{
				List<UIWidget> list = new List<UIWidget>(componentsInChildren);
				list.Sort((UIWidget a, UIWidget b) => a.depth - b.depth);
				for (int i = 0; i < list.Count; i++)
				{
					list[i].depth = depth.value + i;
				}
			}
		}
	}
}
