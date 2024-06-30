using System;
using UnityEngine;

[Serializable]
public class UISkillBubbleItem
{
	protected static readonly string ResourcePrefix = "ig-skill-bg";

	protected static readonly Color gdnTop1 = new Color32(76, byte.MaxValue, 215, byte.MaxValue);

	protected static readonly Color gdnBottom1 = new Color32(106, 210, 254, byte.MaxValue);

	protected static readonly Color outline1 = new Color32(10, 23, 39, byte.MaxValue);

	protected static readonly Color tint1 = new Color32(64, 211, 234, byte.MaxValue);

	protected static readonly Color gdnTop2 = new Color32(byte.MaxValue, 245, 135, byte.MaxValue);

	protected static readonly Color gdnBottom2 = new Color32(byte.MaxValue, 177, 0, byte.MaxValue);

	protected static readonly Color outline2 = new Color32(96, 49, 39, byte.MaxValue);

	protected static readonly Color tint2 = new Color32(248, 185, 29, byte.MaxValue);

	public UISprite bg1;

	public UISprite bg2;

	public UISprite commander;

	public UILabel skillName;

	public void Set(int mode, string commander, string skillName)
	{
		switch (mode)
		{
		case 0:
			bg1.spriteName = ResourcePrefix + "1";
			bg2.spriteName = ResourcePrefix + "2";
			this.skillName.gradientTop = gdnTop1;
			this.skillName.gradientBottom = gdnBottom1;
			this.skillName.effectColor = outline1;
			this.skillName.color = tint1;
			break;
		case 1:
			bg1.spriteName = ResourcePrefix + "3";
			bg2.spriteName = ResourcePrefix + "4";
			this.skillName.gradientTop = gdnTop2;
			this.skillName.gradientBottom = gdnBottom2;
			this.skillName.effectColor = outline2;
			this.skillName.color = tint2;
			break;
		}
		UISetter.SetSprite(this.commander, commander);
		this.skillName.text = skillName;
	}
}
