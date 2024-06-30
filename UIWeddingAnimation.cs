using System.Collections;
using Spine.Unity;
using UnityEngine;

public class UIWeddingAnimation : MonoBehaviour
{
	[SerializeField]
	private Animation WeddingAnimation;

	[SerializeField]
	private GameObject TouchGuide;

	[SerializeField]
	private SkeletonAnimation spine;

	[SerializeField]
	private ClassicRpgManager DialogMrg;

	private bool bPress;

	private bool bTween;

	private void Start()
	{
		StartCoroutine(PlayAnimation_1());
	}

	private void Update()
	{
	}

	public IEnumerator PlayAnimation_1()
	{
		WeddingAnimation.Play("wedding_01");
		while (WeddingAnimation.IsPlaying("wedding_01"))
		{
			yield return null;
		}
		if (PlayerPrefs.GetInt("WeddingGuide", 0) == 0)
		{
			UISetter.SetActive(TouchGuide, active: true);
		}
		spine.loop = true;
		spine.AnimationName = "2";
		bPress = true;
	}

	public IEnumerator PlayAnimation_2()
	{
		SoundManager.PlaySFX("BTN_Kiss_001");
		bPress = false;
		UISetter.SetActive(TouchGuide, active: false);
		spine.loop = false;
		spine.AnimationName = "3";
		PlayerPrefs.SetInt("WeddingGuide", 1);
		WeddingAnimation.Play("wedding_02");
		SoundManager.PlaySFX("SE_Bell_001");
		while (WeddingAnimation.IsPlaying("wedding_02"))
		{
			yield return null;
		}
		yield return new WaitForSeconds(2.1f);
		DialogMrg.EndWeddingEvent();
	}

	public void OnPress()
	{
		if (bPress)
		{
			iTween.ScaleTo(spine.gameObject, iTween.Hash("scale", new Vector3(2.2f, 2.2f, 1f), "islocal", true, "oncomplete", "PlayAnimation_2", "oncompletetarget", base.gameObject, "time", 1f));
			bTween = true;
		}
	}

	public void OnRelease()
	{
		if (bPress && bTween)
		{
			iTween.Stop();
			spine.gameObject.transform.localScale = new Vector3(2f, 2f, 1f);
		}
	}
}
