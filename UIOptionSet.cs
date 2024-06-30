using UnityEngine;

public class UIOptionSet : MonoBehaviour
{
	public delegate void OnClickDelegate(GameObject sender);

	public GameObject optionEnd;

	public GameObject optionPlay;

	public UIOptionField optionSound;

	public UIOptionField optionBgm;

	public UIOptionField optionSkill;

	public OnClickDelegate onClick;

	public virtual void OnClick(GameObject sender)
	{
		if (onClick != null)
		{
			onClick(sender);
		}
	}
}
