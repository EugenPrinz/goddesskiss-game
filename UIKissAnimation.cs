using System.Collections;
using UnityEngine;

public class UIKissAnimation : MonoBehaviour
{
	[SerializeField]
	private GameObject Kiss;

	[SerializeField]
	private UISpineAnimation spineAnim;

	[SerializeField]
	private GameObject RingEff;

	[SerializeField]
	private GameObject HeartEff;

	[SerializeField]
	private UICommanderComplete CommanderComplete;

	[SerializeField]
	private GameObject tutoGuide;

	private bool isPress;

	private float PressTime;

	private Vector2 Scale;

	private AnimType animType;

	private Vector3 HeartEffPos = default(Vector3);

	private bool isTuto;

	private const int MaxTouchCnt = 1;

	private int TouchCount;

	public void SetKissAnim(RoCommander _commander)
	{
		if (_commander == null)
		{
			return;
		}
		TouchCount = 0;
		UISetter.SetActive(tutoGuide, active: false);
		spineAnim.TartgetParent = spineAnim.gameObject;
		spineAnim.spinePrefabName_ForKissAnim = _commander.resourceId + "_kiss";
		if (spineAnim.target != null)
		{
			spineAnim.target.transform.localScale = Vector3.one;
			if (_commander.resourceId == "c_005")
			{
				spineAnim.target.transform.localPosition = new Vector3(21.6f, -290f, -23f);
				isTuto = true;
			}
			else
			{
				spineAnim.target.transform.localPosition = new Vector3(0f, -237f, -23f);
			}
		}
		spineAnim.SetAnimation("kiss_01");
		Scale = Vector2.one;
		Kiss.transform.localScale = Scale;
		animType = AnimType.Kiss_01;
		StartCoroutine(PlayKissAnimation());
	}

	private IEnumerator PlayKissAnimation()
	{
		while (true)
		{
			switch (animType)
			{
			case AnimType.Kiss_01:
				PlayAnimationKiss_01();
				break;
			case AnimType.Kiss_02:
				spineAnim.SetAnimation("kiss_02");
				if (isPress)
				{
					PlayAnimationKiss_02();
				}
				else
				{
					RewindPlayAnimationKiss_02();
				}
				break;
			case AnimType.Kiss_03:
				PlayAnimationKiss_03();
				break;
			}
			yield return null;
		}
	}

	private void PlayAnimationKiss_01()
	{
		Scale = new Vector2(Scale.x += Time.deltaTime * 0.5f, Scale.y += Time.deltaTime * 0.5f);
		if (Scale.x < 2f && Scale.y < 2f)
		{
			Kiss.transform.localScale = Scale;
			return;
		}
		Scale = new Vector2(2f, 2f);
		UISetter.SetActive(tutoGuide, isTuto);
		spineAnim.skeletonAnimation.loop = true;
		animType = AnimType.Kiss_02;
	}

	private void PlayAnimationKiss_02()
	{
		if (Scale.x < 2.5f && Scale.y < 2.5f)
		{
			Scale = new Vector2(Scale.x += Time.deltaTime * 0.7f, Scale.y += Time.deltaTime * 0.7f);
			UISetter.SetActive(HeartEff, active: true);
			Kiss.transform.localScale = Scale;
			return;
		}
		Handheld.Vibrate();
		UISetter.SetActive(HeartEff, active: false);
		UISetter.SetActive(RingEff, active: true);
		spineAnim.SetAnimation("kiss_03");
		spineAnim.skeletonAnimation.loop = false;
		animType = AnimType.Kiss_03;
		if (isTuto)
		{
			isTuto = !isTuto;
		}
		UISetter.SetActive(tutoGuide, isTuto);
	}

	private void RewindPlayAnimationKiss_02()
	{
		UISetter.SetActive(HeartEff, active: false);
		if (Scale.x > 2f && Scale.y > 2f)
		{
			Scale = new Vector2(Scale.x -= Time.deltaTime * 0.7f, Scale.y -= Time.deltaTime * 0.7f);
			Kiss.transform.localScale = Scale;
		}
	}

	private void PlayAnimationKiss_03()
	{
		Scale = new Vector2(Scale.x -= Time.deltaTime * 0.5f, Scale.y -= Time.deltaTime * 0.5f);
		if (Scale.x > 1f && Scale.y > 1f)
		{
			Kiss.transform.localScale = Scale;
			return;
		}
		Scale = Vector2.one;
		Kiss.transform.localScale = Vector2.one;
		CommanderComplete.StartOpenAnimation();
		UISetter.SetActive(base.gameObject, active: false);
		animType = AnimType.none;
	}

	public void OnPress()
	{
		isPress = true;
		HeartEffPos = Input.GetTouch(0).position;
		HeartEffPos = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(HeartEffPos.x, HeartEffPos.y, 0f));
		HeartEff.transform.position = HeartEffPos;
		if (TouchCount < 1)
		{
			Handheld.Vibrate();
		}
		TouchCount++;
	}

	public void OnRelease()
	{
		isPress = false;
	}
}
