using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T00_MeshAnimation : MonoBehaviour
{
	public enum TestMode
	{
		None,
		BoneAnimation,
		MeshAnimation,
		NoAnimation
	}

	public Animation prototype;

	private List<string> _animNames;

	private List<Animation> _cloneList;

	private TestMode _testMode;

	private Text _currFpsText;

	private Text _cloneCountText;

	private Text _minFpsText;

	private Slider _minFpsSlider;

	private const int MaxConfirmCount = 15;

	private int confirmCount;

	private void Start()
	{
		_animNames = new List<string>();
		_cloneList = new List<Animation>();
		foreach (AnimationState item in prototype)
		{
			_animNames.Add(item.name);
		}
		_currFpsText = GameObject.Find("Canvas/Button B/Text").GetComponent<Text>();
		_cloneCountText = GameObject.Find("Canvas/Button C/Text").GetComponent<Text>();
		_minFpsText = GameObject.Find("Canvas/Button D/Text").GetComponent<Text>();
		_minFpsSlider = GameObject.Find("Canvas/Slider").GetComponent<Slider>();
	}

	public void ResetTest()
	{
		foreach (Animation clone in _cloneList)
		{
			Object.Destroy(clone.gameObject);
		}
		_cloneList = new List<Animation>();
		prototype.gameObject.SetActive(value: true);
		_testMode = TestMode.None;
	}

	private void _AddClone()
	{
		int num = 1500;
		HideFlags hideFlags = HideFlags.HideInHierarchy;
		int sampleRate = 20;
		float num2 = 1.1f;
		int num3 = 10;
		if (_cloneList.Count >= num)
		{
			return;
		}
		int count = _cloneList.Count;
		GameObject gameObject = Object.Instantiate(prototype.gameObject);
		gameObject.gameObject.SetActive(value: true);
		gameObject.hideFlags = hideFlags;
		Animation component = gameObject.GetComponent<Animation>();
		int num4 = count / num3;
		int num5 = count % num3;
		Vector3 zero = Vector3.zero;
		zero.x = (float)num5 - 0.5f * (float)num3;
		zero.z = num4;
		zero.x += num2 * 0.5f;
		zero.x *= num2;
		zero.z *= num2;
		component.transform.position = zero;
		if (_testMode == TestMode.MeshAnimation)
		{
			MeshAnimation.AddToAnimation(component, sampleRate, -1, _animNames.ToArray());
		}
		else if (_testMode == TestMode.NoAnimation)
		{
			MeshAnimation[] array = MeshAnimation.AddToAnimation(component, sampleRate, -1, _animNames.ToArray());
			MeshAnimation[] array2 = array;
			foreach (MeshAnimation meshAnimation in array2)
			{
				meshAnimation.enabled = false;
			}
		}
		_cloneList.Add(component);
	}

	private void _RemoveClone()
	{
		int num = _cloneList.Count - 1;
		if (num >= 0)
		{
			Object.Destroy(_cloneList[num].gameObject);
			_cloneList.RemoveAt(num);
		}
	}

	public void TestBoneAnimation()
	{
		ResetTest();
		prototype.gameObject.SetActive(value: false);
		_testMode = TestMode.BoneAnimation;
	}

	public void TestMeshAnimation()
	{
		ResetTest();
		prototype.gameObject.SetActive(value: false);
		_testMode = TestMode.MeshAnimation;
	}

	public void TestNoAnimation()
	{
		ResetTest();
		prototype.gameObject.SetActive(value: false);
		_testMode = TestMode.NoAnimation;
	}

	public void RefreshMinFps()
	{
		int num = (int)_minFpsSlider.value;
		_minFpsText.text = $"Min FPS: {num:D2}";
	}

	private static AnimationState _GetPlayingMaxWeightAnimationState(Animation anim)
	{
		AnimationState animationState = null;
		foreach (AnimationState item in anim)
		{
			if (anim.IsPlaying(item.name) && (animationState == null || animationState.weight < item.weight))
			{
				animationState = item;
			}
		}
		return animationState;
	}

	private void Update()
	{
		float value = _minFpsSlider.value;
		float num = Mathf.Ceil(1f / Time.smoothDeltaTime);
		confirmCount += ((num >= value) ? 1 : (-1));
		confirmCount = Mathf.Clamp(confirmCount, -15, 15);
		if (_testMode != 0)
		{
			switch (confirmCount)
			{
			case 15:
				_AddClone();
				confirmCount -= 3;
				break;
			case -15:
				_RemoveClone();
				confirmCount += 3;
				break;
			}
		}
		_currFpsText.text = $"Current FPS: {num:00}";
		_cloneCountText.text = $"Count: {_cloneList.Count,5}";
		foreach (Animation clone in _cloneList)
		{
			if (_testMode == TestMode.NoAnimation)
			{
				clone.Stop();
				continue;
			}
			AnimationState animationState = _GetPlayingMaxWeightAnimationState(clone);
			if (!(animationState != null) || !(animationState.normalizedTime <= 1f))
			{
				int index = Random.Range(0, _animNames.Count);
				string animation = _animNames[index];
				clone.Play(animation, PlayMode.StopAll);
			}
		}
	}
}
