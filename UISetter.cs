using UnityEngine;

public static class UISetter
{
	public static void SetGameObjectName(GameObject go, string name)
	{
		if (go != null)
		{
			go.name = name;
		}
	}

	public static void SetActive(MonoBehaviour mb, bool active)
	{
		if (mb != null)
		{
			mb.gameObject.SetActive(active);
		}
	}

	public static void SetActive(UIPanelBase pb, bool active)
	{
		if (pb != null)
		{
			if (pb.root != null)
			{
				pb.root.SetActive(active);
			}
			else
			{
				pb.gameObject.SetActive(active);
			}
		}
	}

	public static void SetActive(UIInnerPartBase ipb, bool active)
	{
		if (ipb != null && ipb.root != null)
		{
			ipb.root.SetActive(active);
		}
	}

	public static void SetActive(GameObject go, bool active)
	{
		if (go != null)
		{
			go.SetActive(active);
		}
	}

	public static void SetVoice(UISpineAnimation spine, bool active)
	{
		if (spine != null && spine.target != null)
		{
			SetVoice(spine.target.GetComponent<UIInteraction>(), active);
		}
	}

	public static void SetVoice(UIInteraction interaction, bool active)
	{
		if (interaction != null)
		{
			interaction.enableVoice = active;
		}
	}

	public static void SetTexture(UITexture ui, Texture texture)
	{
		if (ui != null)
		{
			ui.mainTexture = texture;
		}
	}

	public static void SetColor(UILabel label, Color color)
	{
		if (label != null)
		{
			label.color = color;
		}
	}

	public static void SetColor(UISprite sprite, Color color)
	{
		if (sprite != null)
		{
			sprite.color = color;
		}
	}

	public static void SetButtonEnable(GameObject go, bool enable)
	{
		if (go == null)
		{
			return;
		}
		UIButton component = go.GetComponent<UIButton>();
		if (component == null)
		{
			if (go.GetComponent<Collider>() != null)
			{
				go.GetComponent<Collider>().enabled = enable;
			}
		}
		else
		{
			component.isEnabled = enable;
		}
	}

	public static void SetButtonGray(GameObject go, bool enable)
	{
		if (go == null)
		{
			return;
		}
		UIButton component = go.GetComponent<UIButton>();
		if (!(component == null))
		{
			if (!enable)
			{
				component.hover = Color.gray;
				component.pressed = Color.gray;
				component.defaultColor = Color.gray;
				component.disabledColor = Color.gray;
			}
			else
			{
				component.hover = Color.white;
				component.pressed = Color.white;
				component.defaultColor = Color.white;
				component.disabledColor = Color.white;
			}
			component.isGray = !enable;
		}
	}

	public static void SetLabel(UILabel label, object text)
	{
		if (label != null)
		{
			label.text = text?.ToString();
		}
	}

	public static void SetLabelWithLocalization(UILabel label, string key)
	{
		if (label != null)
		{
			string text = null;
			if (key != null)
			{
				text = ((!Localization.Exists(key)) ? key : Localization.Get(key));
			}
			label.text = text;
		}
	}

	public static void SetSprite(UISprite sprite, string spriteName)
	{
		if (!(sprite != null))
		{
			return;
		}
		if (sprite.atlas != null && !string.IsNullOrEmpty(spriteName))
		{
			if (sprite.atlas.name == "Unit")
			{
				if (spriteName.StartsWith("2_"))
				{
					sprite.atlas = Resources.Load<UIAtlas>("Atlas/Unit_2");
				}
			}
			else if (sprite.atlas.name == "Unit_2")
			{
				if (!spriteName.StartsWith("2_"))
				{
					sprite.atlas = Resources.Load<UIAtlas>("Atlas/Unit");
				}
			}
			else if (sprite.atlas.name == "CommanderAtlas" || sprite.atlas.name == "CommanderAtlas_2")
			{
				string[] array = spriteName.Split('_');
				if (array.Length > 2)
				{
					if (array[2] == "1")
					{
						if (sprite.atlas.name != "CommanderAtlas")
						{
							sprite.atlas = Resources.Load<UIAtlas>("Atlas/CommanderAtlas");
						}
					}
					else if (sprite.atlas.name != "CommanderAtlas_2")
					{
						sprite.atlas = Resources.Load<UIAtlas>("Atlas/CommanderAtlas_2");
					}
				}
			}
		}
		sprite.spriteName = spriteName;
	}

	public static void SetSprite(UISprite sprite, string spriteName, bool snap)
	{
		if (!(sprite != null))
		{
			return;
		}
		if (sprite.atlas != null && !string.IsNullOrEmpty(spriteName))
		{
			if (sprite.atlas.name == "Unit")
			{
				if (spriteName.StartsWith("2_"))
				{
					sprite.atlas = Resources.Load<UIAtlas>("Atlas/Unit_2");
				}
			}
			else if (sprite.atlas.name == "Unit_2")
			{
				if (!spriteName.StartsWith("2_"))
				{
					sprite.atlas = Resources.Load<UIAtlas>("Atlas/Unit");
				}
			}
			else if (sprite.atlas.name == "CommanderAtlas" || sprite.atlas.name == "CommanderAtlas_2")
			{
				string[] array = spriteName.Split('_');
				if (array.Length > 2)
				{
					if (array[2] == "1")
					{
						if (sprite.atlas.name != "CommanderAtlas")
						{
							sprite.atlas = Resources.Load<UIAtlas>("Atlas/CommanderAtlas");
						}
					}
					else if (sprite.atlas.name != "CommanderAtlas_2")
					{
						sprite.atlas = Resources.Load<UIAtlas>("Atlas/CommanderAtlas_2");
					}
				}
			}
		}
		sprite.spriteName = spriteName;
		if (snap)
		{
			sprite.MakePixelPerfect();
		}
	}

	public static void SetSpriteWithSnap(UISprite sprite, string spriteName, bool pixelPerfect = true)
	{
		if (!(sprite != null))
		{
			return;
		}
		if (sprite.atlas != null && !string.IsNullOrEmpty(spriteName))
		{
			if (sprite.atlas.name == "Unit")
			{
				if (spriteName.StartsWith("2_"))
				{
					sprite.atlas = Resources.Load<UIAtlas>("Atlas/Unit_2");
				}
			}
			else if (sprite.atlas.name == "Unit_2")
			{
				if (!spriteName.StartsWith("2_"))
				{
					sprite.atlas = Resources.Load<UIAtlas>("Atlas/Unit");
				}
			}
			else if (sprite.atlas.name == "CommanderAtlas" || sprite.atlas.name == "CommanderAtlas_2")
			{
				string[] array = spriteName.Split('_');
				if (array.Length > 2)
				{
					if (array[2] == "1")
					{
						if (sprite.atlas.name != "CommanderAtlas")
						{
							sprite.atlas = Resources.Load<UIAtlas>("Atlas/CommanderAtlas");
						}
					}
					else if (sprite.atlas.name != "CommanderAtlas_2")
					{
						sprite.atlas = Resources.Load<UIAtlas>("Atlas/CommanderAtlas_2");
					}
				}
			}
		}
		sprite.spriteName = spriteName;
		if (pixelPerfect)
		{
			sprite.MakePixelPerfect();
		}
	}

	public static void SetSprite(UISprite sprite, string atlasName, string spriteName)
	{
		if (!(sprite == null))
		{
			sprite.SetAtlasImage(atlasName, spriteName);
		}
	}

	public static void SetProgress(UIProgressBar progressBar, float val)
	{
		if (progressBar != null)
		{
			progressBar.value = Mathf.Clamp01(val);
		}
	}

	public static void SetProgress(UISprite sprite, float val)
	{
		if (sprite != null)
		{
			sprite.fillAmount = Mathf.Clamp01(val);
		}
	}

	public static void SetAlpha(UIRect rt, float alpha)
	{
		if (rt != null)
		{
			rt.alpha = alpha;
		}
	}

	public static void SetTimer(UITimer ui, TimeData time)
	{
		if (ui != null)
		{
			ui.Set(time);
		}
	}

	public static void SetTimer(UITimer ui, TimeData time, string localkey)
	{
		if (ui != null)
		{
			ui.Set(time, localkey);
		}
	}

	public static void SetRank(UIGrid gradeGrid, int grade)
	{
		if (gradeGrid == null || grade < 0)
		{
			return;
		}
		Transform transform = gradeGrid.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child == null)
			{
				break;
			}
			child.gameObject.SetActive(i < grade);
			if (grade > 5)
			{
				if (child.gameObject.GetComponent<UISprite>() != null)
				{
					if (child.gameObject.GetComponent<UISprite>().spriteName.StartsWith("com_icon_star"))
					{
						SetSprite(child.gameObject.GetComponent<UISprite>(), "com_icon_star2");
					}
					else if (child.gameObject.GetComponent<UISprite>().spriteName.StartsWith("ig-character-star"))
					{
						SetSprite(child.gameObject.GetComponent<UISprite>(), "ig-character-star2");
					}
				}
			}
			else if (child.gameObject.GetComponent<UISprite>() != null)
			{
				if (child.gameObject.GetComponent<UISprite>().spriteName.StartsWith("com_icon_star"))
				{
					SetSprite(child.gameObject.GetComponent<UISprite>(), "com_icon_star");
				}
				else if (child.gameObject.GetComponent<UISprite>().spriteName.StartsWith("ig-character-star"))
				{
					SetSprite(child.gameObject.GetComponent<UISprite>(), "ig-character-star");
				}
			}
		}
		gradeGrid.enabled = true;
		gradeGrid.Reposition();
	}

	public static void SetRank(GameObject gradeGrid, int grade)
	{
		if (gradeGrid == null || grade < 0)
		{
			return;
		}
		Transform transform = gradeGrid.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child == null)
			{
				break;
			}
			child.gameObject.SetActive(i < grade);
		}
	}

	public static void SetStatus(UIStatus ui, RoUnit unit)
	{
		if (ui != null)
		{
			ui.Set(unit);
		}
	}

	public static void SetStatus(UIStatus ui, RoCommander commander)
	{
		if (ui != null)
		{
			ui.Set(commander);
		}
	}

	public static void SetStatus(UIStatus ui, RoTroop troop)
	{
		if (ui != null)
		{
			ui.Set(troop);
		}
	}

	public static void SetToggle(UIToggle toggle, bool state)
	{
		if (toggle != null)
		{
			toggle.Set(state);
		}
	}

	public static void SetFlipSwitch(UIFlipSwitch flip, bool state)
	{
		SwitchStatus state2 = (state ? SwitchStatus.ON : SwitchStatus.OFF);
		if (flip != null)
		{
			flip.Set(state2);
		}
	}

	public static void SetCollider(Collider coll, bool active)
	{
		if (coll != null)
		{
			coll.enabled = active;
		}
	}

	public static void SetSpine(UISpineAnimation spine, string prefabName)
	{
		if (!(spine != null))
		{
			return;
		}
		spine.spinePrefabName = prefabName;
		RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommanderResourceId(prefabName);
		if (roCommander != null)
		{
			if (roCommander.isBasicCostume)
			{
				spine.SetSkin(roCommander.currentViewCostume);
			}
			else
			{
				spine.SetSkin(roCommander.getCurrentCostumeName());
			}
		}
	}

	public static void SetSpine(UISpineAnimation spine, string prefabName, string costume)
	{
		if (spine != null)
		{
			spine.spinePrefabName = prefabName;
			spine.SetSkin(costume);
		}
	}

	public static void SetSpineAnimationName(UISpineAnimation spine, string animationName)
	{
		if (spine != null)
		{
			spine.SetAnimation(animationName);
		}
	}

	public static void SetUser(UIUser ui, RoUser user)
	{
		if (ui != null)
		{
			ui.Set(user);
		}
	}

	public static void SetScale(UISprite sprite, Vector3 vec)
	{
		if (sprite != null)
		{
			sprite.transform.localScale = vec;
		}
	}

	public static void SetScale(GameObject go, Vector3 vec)
	{
		if (go != null)
		{
			go.transform.localScale = vec;
		}
	}
}
