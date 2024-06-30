using UnityEngine;

namespace Step
{
	public class GetTutorialViewObject : AbstractStepAction
	{
		public enum E_VIEW_TARGET
		{
			UNIT_PANEL,
			LEFT_BOTTOM
		}

		public E_VIEW_TARGET viewType;

		public GameObjectData ret;

		public override bool Enter()
		{
			if (ret == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			M04_Tutorial m04_Tutorial = (M04_Tutorial)UIManager.instance.battle.Main;
			GameObject value = GameObject.Find("SceneRoot/UI Root/Battle/Main/UIView/M_Center");
			GameObject value2 = GameObject.Find("SceneRoot/UI Root/Battle/UnitPanel");
			switch (viewType)
			{
			case E_VIEW_TARGET.LEFT_BOTTOM:
				ret.value = value;
				break;
			case E_VIEW_TARGET.UNIT_PANEL:
				ret.value = value2;
				break;
			}
		}
	}
}
