using Shared.Regulation;
using UnityEngine;

public class CutInSkill : MonoBehaviour
{
	public UILabel skillNameLabel;

	public UISpineAnimation uiSpine;

	public void Init(CutInDataSet cutInData)
	{
		CommanderDataRow commanderDataRow = RemoteObjectManager.instance.regulation.commanderDtbl[cutInData.commanderDri];
		uiSpine.spinePrefabName = commanderDataRow.resourceId;
		UIInteraction component = uiSpine.skeletonAnimation.GetComponent<UIInteraction>();
		if (component != null)
		{
			component.EnableInteration = false;
			component.enabled = false;
		}
		uiSpine.skeletonAnimation.state.SetAnimation(0, "a_04_skill", loop: false);
		skillNameLabel.text = string.Empty;
		if (!string.IsNullOrEmpty(cutInData.skill._skillDataRow.skillName) && cutInData.skill._skillDataRow.skillName != "-")
		{
			skillNameLabel.text = Localization.Get(cutInData.skill._skillDataRow.skillName);
		}
		if (cutInData.side == E_TARGET_SIDE.RIGHT)
		{
			base.transform.localRotation = new Quaternion(0f, -180f, 0f, 0f);
			skillNameLabel.transform.localScale = new Vector3(-1f, skillNameLabel.transform.localScale.y, skillNameLabel.transform.localScale.z);
			uiSpine.transform.localScale = new Vector3(-1f, uiSpine.transform.localScale.y, uiSpine.transform.localScale.z);
		}
		else
		{
			base.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			skillNameLabel.transform.localScale = new Vector3(1f, 1f, 1f);
			uiSpine.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		base.gameObject.SetActive(value: true);
	}
}
