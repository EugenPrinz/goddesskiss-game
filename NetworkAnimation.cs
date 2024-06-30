using UnityEngine;

public class NetworkAnimation : MonoBehaviour
{
	private static NetworkAnimation _instance;

	public GameObject obj;

	public MessageTextAlpha2D floatingText;

	public UIToastPopup ToastPop;

	public static NetworkAnimation Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(NetworkAnimation)) as NetworkAnimation;
				if (_instance == null)
				{
					UIRoot uIRoot = Object.FindObjectOfType(typeof(UIRoot)) as UIRoot;
					Object.Instantiate(Resources.Load("Prefabs/UI/NetworkAnimation") as GameObject).transform.parent = uIRoot.transform;
					_instance = Object.FindObjectOfType(typeof(NetworkAnimation)) as NetworkAnimation;
				}
			}
			return _instance;
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void Awake()
	{
		Off();
	}

	private void Start()
	{
		base.gameObject.transform.localScale = Vector2.one;
		base.gameObject.transform.localPosition = Vector2.zero;
	}

	public void On()
	{
		UISetter.SetActive(obj, active: true);
	}

	public void Off()
	{
		UISetter.SetActive(obj, active: false);
	}

	public void CreateFloatingText(Vector3 _position, string _text)
	{
		floatingText.initAndStart(_text);
	}

	public void CreateFloatingText(string _text)
	{
		floatingText.initAndStart(_text);
	}

	public void CreateFloatingText_OnlyUIToast(string _text, string spriteName)
	{
		ToastPop.initAndStart(_text);
		ToastPop.ToastImg.spriteName = spriteName;
	}
}
