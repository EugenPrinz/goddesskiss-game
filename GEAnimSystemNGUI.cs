using UnityEngine;

public class GEAnimSystemNGUI : GUIAnimSystemNGUI
{
	public override void GEAnimSystemNGUIStart()
	{
		m_AutoAnimation = false;
		m_GUISpeed = 1f;
		Object.DontDestroyOnLoad(this);
	}

	public override void GEAnimSystemNGUIUpdate()
	{
	}

	public override void Anim_EnableButton(Transform trans, bool Enable)
	{
		UIButton component = trans.gameObject.GetComponent<UIButton>();
		if (component != null)
		{
			component.enabled = Enable;
		}
	}

	public override void Anim_EnableAllButtons(bool Enable)
	{
		UIButton[] array = Object.FindObjectsOfType<UIButton>();
		UIButton[] array2 = array;
		foreach (UIButton uIButton in array2)
		{
			uIButton.enabled = Enable;
		}
	}
}
