using UnityEngine;

public class GEAnimNGUI : GUIAnimNGUI
{
	private UIBasicSprite m_UIBasicSprite;

	private UIButton m_UIButton;

	private UILabel m_UILabel;

	private UIButtonScale m_UIButtonScale;

	public override void Anim_Awake()
	{
	}

	public override void Anim_Start()
	{
	}

	public override void Anim_In_Move()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", m_MoveIn.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_MoveIn.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_MoveIn.EaseType), "onupdate", "AnimIn_MoveUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimIn_MoveComplete"));
		m_MoveIn.Began = true;
	}

	public override void Anim_In_MoveComplete()
	{
	}

	public override void Anim_In_Rotate()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1f, "time", m_RotationIn.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_RotationIn.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_RotationIn.EaseType), "onupdate", "AnimIn_RotationUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimIn_RotationComplete"));
		m_RotationIn.Began = true;
	}

	public override void Anim_In_RotateComplete()
	{
	}

	public override void Anim_In_Scale()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1f, "time", m_ScaleIn.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_ScaleIn.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_ScaleIn.EaseType), "onupdate", "AnimIn_ScaleUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimIn_ScaleComplete"));
		m_ScaleIn.Began = true;
	}

	public override void Anim_In_ScaleComplete()
	{
	}

	public override void Anim_In_Fade()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1f, "time", m_FadeIn.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_FadeIn.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_FadeIn.EaseType), "onupdate", "AnimIn_FadeUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimIn_FadeComplete"));
	}

	public override void Anim_In_FadeComplete()
	{
	}

	public override void Anim_In_AllComplete()
	{
	}

	public override void Anim_Out_Move()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", m_MoveOut.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_MoveOut.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_MoveOut.EaseType), "onupdate", "AnimOut_MoveUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimOut_MoveComplete"));
		m_MoveOut.Began = true;
	}

	public override void Anim_Out_MoveComplete()
	{
	}

	public override void Anim_Out_Rotate()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", m_RotationOut.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_RotationOut.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_RotationOut.EaseType), "onupdate", "AnimOut_RotationUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimOut_RotationComplete"));
		m_RotationOut.Began = true;
	}

	public override void Anim_Out_RotateComplete()
	{
	}

	public override void Anim_Out_Scale()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", m_ScaleOut.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_ScaleOut.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_ScaleOut.EaseType), "onupdate", "AnimOut_ScaleUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimOut_ScaleComplete"));
		m_ScaleOut.Began = true;
	}

	public override void Anim_Out_ScaleComplete()
	{
	}

	public override void Anim_Out_Fade()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1f, "time", m_FadeOut.Time / GUIAnimSystemNGUI.Instance.m_GUISpeed, "ignoretimescale", false, "delay", m_FadeOut.Delay / GUIAnimSystemNGUI.Instance.m_GUISpeed, "easeType", iTweenEaseType(m_FadeOut.EaseType), "onupdate", "AnimOut_FadeUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "AnimOut_FadeComplete"));
	}

	public override void Anim_Out_FadeComplete()
	{
	}

	public override void Anim_Out_AllComplete()
	{
	}

	public override void Anim_Idle_ScaleLoopStart(float delay)
	{
		float num = m_ScaleLoop.Time / 2f / GUIAnimSystemNGUI.Instance.m_GUISpeed;
		float num2 = delay / GUIAnimSystemNGUI.Instance.m_GUISpeed;
		float num3 = num2 + num;
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", num, "delay", num2, "easeType", iTweenEaseType(m_ScaleLoop.EaseType), "onupdate", "BreathLoopUpdateByValue", "onupdatetarget", base.gameObject));
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", num, "delay", num3, "easeType", iTweenEaseType(m_ScaleLoop.EaseType), "onupdate", "BreathLoopUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "BreathLoopComplete"));
	}

	public override void Anim_Idle_StopScaleLoop()
	{
	}

	public override Vector2 Anim_Idle_CheckScaleLoopComplete(float x, float y)
	{
		if (m_UIButtonScale != null)
		{
			return new Vector2(Mathf.Abs(m_UIButtonScale.pressed.x - x), Mathf.Abs(m_UIButtonScale.pressed.y - y));
		}
		return new Vector2(0.05f, 0.05f);
	}

	public override void Anim_Idle_FadeLoopStart(float delay)
	{
		float num = m_FadeLoop.Time / 2f / GUIAnimSystemNGUI.Instance.m_GUISpeed;
		float num2 = delay / GUIAnimSystemNGUI.Instance.m_GUISpeed;
		float num3 = num2 + num;
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", num, "delay", num2, "easeType", iTweenEaseType(m_FadeLoop.EaseType), "onupdate", "FadeLoopUpdateByValue", "onupdatetarget", base.gameObject));
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", num, "delay", num3, "easeType", iTweenEaseType(m_FadeLoop.EaseType), "onupdate", "FadeLoopUpdateByValue", "onupdatetarget", base.gameObject, "oncomplete", "FadeLoopComplete"));
	}

	public override void Anim_Idle_StopFadeLoop()
	{
	}

	public string iTweenEaseType(eEaseType easeType)
	{
		string text = "linear";
		return easeType switch
		{
			eEaseType.InQuad => "easeInQuad", 
			eEaseType.OutQuad => "easeOutQuad", 
			eEaseType.InOutQuad => "easeInOutQuad", 
			eEaseType.InCubic => "easeOutCubic", 
			eEaseType.OutCubic => "easeOutCubic", 
			eEaseType.InOutCubic => "easeInOutCubic", 
			eEaseType.InQuart => "easeInQuart", 
			eEaseType.OutQuart => "easeOutQuart", 
			eEaseType.InOutQuart => "easeInOutQuart", 
			eEaseType.InQuint => "easeInQuint", 
			eEaseType.OutQuint => "easeOutQuint", 
			eEaseType.InOutQuint => "easeInOutQuint", 
			eEaseType.InSine => "easeInSine", 
			eEaseType.OutSine => "easeOutSine", 
			eEaseType.InOutSine => "easeInOutSine", 
			eEaseType.InExpo => "easeInExpo", 
			eEaseType.OutExpo => "easeOutExpo", 
			eEaseType.InOutExpo => "easeInOutExpo", 
			eEaseType.InCirc => "easeInCirc", 
			eEaseType.OutCirc => "easeOutCirc", 
			eEaseType.InOutCirc => "easeInOutCirc", 
			eEaseType.linear => "linear", 
			eEaseType.InBounce => "easeInBounce", 
			eEaseType.OutBounce => "easeOutBounce", 
			eEaseType.InOutBounce => "easeInOutBounce", 
			eEaseType.InBack => "easeInBack", 
			eEaseType.OutBack => "easeOutBack", 
			eEaseType.InOutBack => "easeInOutBack", 
			eEaseType.InElastic => "easeInElastic", 
			eEaseType.OutElastic => "easeOutElastic", 
			eEaseType.InOutElastic => "easeInOutElastic", 
			_ => "linear", 
		};
	}

	public override void Anim_Init_CheckAttachedNGUIObjects()
	{
		m_UIBasicSprite = base.gameObject.GetComponent<UIBasicSprite>();
		m_UIButton = base.gameObject.GetComponent<UIButton>();
		m_UILabel = base.gameObject.GetComponent<UILabel>();
		m_UIButtonScale = base.gameObject.GetComponent<UIButtonScale>();
		if (base.gameObject.GetComponent<Renderer>() != null)
		{
			if ((bool)m_UIBasicSprite)
			{
				m_FadeOriginal = m_UIBasicSprite.color.a;
			}
			if ((bool)m_UIButton)
			{
				m_FadeOriginal = m_UIButton.defaultColor.a;
			}
			if ((bool)m_UILabel)
			{
				m_FadeOriginal = m_UILabel.color.a;
			}
		}
	}

	public override Bounds Anim_Init_CalculateTotalBounds(Transform trans)
	{
		return NGUIMath.CalculateAbsoluteWidgetBounds(trans);
	}

	public override void Anim_Fade(Transform trans, float FadeValue)
	{
		UISprite component = trans.gameObject.GetComponent<UISprite>();
		if (component != null)
		{
			component.color = new Color(component.color.r, component.color.g, component.color.b, FadeValue);
		}
		UIBasicSprite component2 = trans.gameObject.GetComponent<UIBasicSprite>();
		if (component2 != null)
		{
			component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, FadeValue);
		}
		UIButton component3 = trans.gameObject.GetComponent<UIButton>();
		if (component3 != null)
		{
			component3.state = UIButtonColor.State.Normal;
			component3.defaultColor = new Color(component3.defaultColor.r, component3.defaultColor.g, component3.defaultColor.b, FadeValue);
			component3.hover = new Color(component3.hover.r, component3.hover.g, component3.hover.b, FadeValue);
			component3.pressed = new Color(component3.pressed.r, component3.pressed.g, component3.pressed.b, FadeValue);
			component3.disabledColor = new Color(component3.disabledColor.r, component3.disabledColor.g, component3.disabledColor.b, FadeValue);
			component3.UpdateColor(instant: true);
		}
		UILabel component4 = trans.gameObject.GetComponent<UILabel>();
		if (component4 != null)
		{
			component4.color = new Color(component4.color.r, component4.color.g, component4.color.b, FadeValue);
		}
	}

	public override float Anim_GetFadeValue(Transform trans)
	{
		float result = 1f;
		if (m_UIBasicSprite != null)
		{
			result = m_UIBasicSprite.color.a;
		}
		if ((bool)m_UIButton)
		{
			result = m_UIButton.defaultColor.a;
		}
		if (m_UILabel != null)
		{
			result = m_UILabel.color.a;
		}
		return result;
	}
}
