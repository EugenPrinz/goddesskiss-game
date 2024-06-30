using UnityEngine;

public class UIStatusBubbleItem : MonoBehaviour
{
	[HideInInspector]
	public int idx;

	public UILabel damage;

	public UISkillBubbleItem skill;

	protected Color _orgColor;

	private void Awake()
	{
		if (damage != null)
		{
			_orgColor = damage.color;
		}
	}

	public void Set(string damage)
	{
		this.damage.color = _orgColor;
		this.damage.text = damage;
	}

	public void Set(int mode, string commander, string skillName)
	{
		skill.Set(mode, commander, skillName);
	}
}
