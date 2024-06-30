using UnityEngine;

public class UIOptionField : MonoBehaviour
{
	protected bool status;

	public UIEventListener on;

	public UIEventListener off;

	public virtual bool Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
			UpdateStatus();
		}
	}

	private void Awake()
	{
		on.onClick = delegate
		{
			SoundManager.PlaySFX("BTN_Norma_001");
			Status = false;
		};
		off.onClick = delegate
		{
			SoundManager.PlaySFX("BTN_Norma_001");
			Status = true;
		};
	}

	public virtual void UpdateStatus()
	{
		if (status)
		{
			on.gameObject.SetActive(value: true);
			off.gameObject.SetActive(value: false);
		}
		else
		{
			off.gameObject.SetActive(value: true);
		}
	}
}
