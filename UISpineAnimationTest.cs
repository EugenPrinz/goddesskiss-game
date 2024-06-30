using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UISpineAnimationTest : MonoBehaviour
{
	private float timer;

	private float waitingTime = 1.8f;

	private float fingerStartTime;

	private Vector2 fingerStartPos = Vector2.zero;

	private bool isSwipe;

	private float minSwipeDist = 100f;

	private float maxSwipeTime = 2f;

	public UISpineAnimation spine;

	private string commanderId;

	public bool bShowSpine;

	private bool _enableVoice = true;

	private string spineBundleName;

	public bool enableVoice
	{
		get
		{
			return _enableVoice;
		}
		set
		{
			_enableVoice = value;
			UISetter.SetVoice(spine, _enableVoice);
		}
	}

	private void Start()
	{
		Set();
	}

	public void CreateMainSpine()
	{
		Set();
	}

	public void Set(string strCommanderID)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoUser localUser = RemoteObjectManager.instance.localUser;
		string costumeThumbnailName = regulation.GetCostumeThumbnailName(int.Parse(localUser.thumbnailId));
		string[] array = costumeThumbnailName.Split('_');
		string resourceId = array[0] + "_" + array[1];
		if (base.gameObject.activeSelf && spine != null)
		{
			StartCoroutine(CreateSpineFromCache(resourceId));
		}
	}

	public void Set()
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoUser localUser = RemoteObjectManager.instance.localUser;
		string costumeThumbnailName = regulation.GetCostumeThumbnailName(int.Parse(localUser.thumbnailId));
		string[] array = costumeThumbnailName.Split('_');
		string resourceId = array[0] + "_" + array[1];
		if (base.gameObject.activeSelf && spine != null)
		{
			StartCoroutine(CreateSpineFromCache(resourceId));
		}
	}

	public void Set(RoCommander commander)
	{
		if (commander != null && base.gameObject.activeSelf && spine != null)
		{
			StartCoroutine(CreateSpineFromCache(commander.resourceId));
		}
	}

	public IEnumerator SetCommander(RoCommander commander)
	{
		if (commander != null && base.gameObject.activeSelf && spine != null)
		{
			yield return StartCoroutine(CreateSpineFromCache(commander.resourceId));
		}
	}

	private IEnumerator CreateSpineFromCache(string resourceId)
	{
		if (spine != null)
		{
			if (!string.IsNullOrEmpty(spineBundleName) && spineBundleName != resourceId + ".assetbundle")
			{
				AssetBundleManager.DeleteAssetBundle(spineBundleName);
			}
			if (!AssetBundleManager.HasAssetBundle(resourceId + ".assetbundle"))
			{
				yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceId + ".assetbundle"));
				UISetter.SetActive(spine, active: false);
			}
			UISetter.SetActive(spine, active: true);
			if (base.gameObject.activeSelf)
			{
				UISetter.SetSpine(spine, resourceId);
			}
			spine.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			spineBundleName = resourceId + ".assetbundle";
			UISetter.SetVoice(spine, enableVoice);
			spine.target.GetComponent<UIInteraction>().mainDisplay = true;
			SetInteraction();
		}
		yield return null;
	}

	public void SetInteraction(RoCommander commander = null)
	{
		if (commander == null)
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(int.Parse(RemoteObjectManager.instance.localUser.thumbnailId));
			if (commanderCostumeDataRow != null)
			{
				commander = RemoteObjectManager.instance.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
				if (commander != null)
				{
					spine.target.GetComponent<UIInteraction>().favorStep = commander.favorRewardStep;
					spine.target.GetComponent<UIInteraction>().marry = commander.marry;
				}
			}
		}
		else
		{
			spine.target.GetComponent<UIInteraction>().favorStep = commander.favorRewardStep;
			spine.target.GetComponent<UIInteraction>().marry = commander.marry;
		}
	}

	public void Set(RoRecruit.Entry entry)
	{
		if (entry != null)
		{
			Set(entry.commander);
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		}
	}

	public void Hide()
	{
		bShowSpine = false;
		UIManager.instance.world.mainCommand.CloseLeftMainSpine();
	}

	public void Show()
	{
		bShowSpine = true;
		UIManager.instance.world.mainCommand.StartLeftMainSpine();
	}

	private void Update()
	{
		if (Input.touchCount <= 0)
		{
			return;
		}
		if (UIPopup.openedPopups.Count > 0 && !(UIPopup.openedPopups[0] is UICamp))
		{
			isSwipe = false;
			return;
		}
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			switch (touch.phase)
			{
			case TouchPhase.Began:
				isSwipe = true;
				fingerStartTime = Time.time;
				fingerStartPos = touch.position;
				break;
			case TouchPhase.Canceled:
				isSwipe = false;
				break;
			case TouchPhase.Ended:
			{
				float num = Time.time - fingerStartTime;
				float magnitude = (touch.position - fingerStartPos).magnitude;
				if (!isSwipe || !(num < maxSwipeTime) || !(magnitude > minSwipeDist))
				{
					break;
				}
				Vector2 vector = touch.position - fingerStartPos;
				Vector2 zero = Vector2.zero;
				zero = ((!(Mathf.Abs(vector.x) > Mathf.Abs(vector.y))) ? (Vector2.up * Mathf.Sign(vector.y)) : (Vector2.right * Mathf.Sign(vector.x)));
				if (zero.x != 0f)
				{
					if (zero.x > 0f)
					{
						Hide();
					}
					else
					{
						Show();
					}
				}
				if (zero.y != 0f && !(zero.y > 0f))
				{
				}
				break;
			}
			}
		}
	}
}
