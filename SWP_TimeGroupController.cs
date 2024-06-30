using System.Collections.Generic;
using UnityEngine;

public class SWP_TimeGroupController : MonoBehaviour
{
	private enum SoundType
	{
		PauseSound,
		SlowDownSound,
		SpeedUpSound,
		BackToNormalSound
	}

	public delegate void OnGroupPauseEvent(SWP_InternalTimedClass _TimedClass);

	public delegate void OnGroupSlowDownEvent(SWP_InternalTimedClass _TimedClass);

	public delegate void OnGroupSpeedUpEvent(SWP_InternalTimedClass _TimedClass);

	public delegate void OnGroupResumeEvent(SWP_InternalTimedClass _TimedClass);

	public float ControllerSpeedPercent = 100f;

	public float ControllerSpeedZeroToOne = 1f;

	public int GroupID = 1;

	public bool EnableSound = true;

	public float SoundVolume = 1f;

	public AudioClip PauseSound;

	public AudioClip SlowDownSound;

	public AudioClip SpeedUpSound;

	public AudioClip ResumeSound;

	private AudioSource asAudio;

	public event OnGroupPauseEvent OnGroupPause;

	public event OnGroupSlowDownEvent OnGroupSlowDown;

	public event OnGroupSpeedUpEvent OnGroupSpeedUp;

	public event OnGroupResumeEvent OnGroupResume;

	private void Awake()
	{
		asAudio = GetComponent<AudioSource>();
	}

	private void Start()
	{
		SoundVolume = Mathf.Clamp(SoundVolume, 0f, 1f);
		if (asAudio == null || !asAudio.enabled)
		{
			EnableSound = false;
		}
	}

	private void BroadcastEvents(string _EventName)
	{
		Dictionary<int, SWP_InternalTimedGameObject>.Enumerator enumerator = SWP_TimedGameObject.timedObjects.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.SendMessage(_EventName, SendMessageOptions.DontRequireReceiver);
		}
	}

	protected virtual void BroadcastEvents(string _EventName, object _PassedObject)
	{
		Dictionary<int, SWP_InternalTimedGameObject>.Enumerator enumerator = SWP_TimedGameObject.timedObjects.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.SendMessage(_EventName, _PassedObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	public float TimedDeltaTime()
	{
		if (ControllerSpeedPercent != 0f)
		{
			return Time.deltaTime / (100f / ControllerSpeedPercent);
		}
		return 0f;
	}

	protected void SetControllerSpeed(float _NewSpeed)
	{
		ControllerSpeedPercent = _NewSpeed;
		ControllerSpeedZeroToOne = ((ControllerSpeedPercent != 0f) ? (ControllerSpeedPercent / 100f) : 0f);
	}

	public virtual void PauseGroupTime()
	{
		SetControllerSpeed(0f);
		SWP_InternalTimedClass sWP_InternalTimedClass = new SWP_InternalTimedClass(GroupID, ControllerSpeedPercent);
		BroadcastEvents("OnGroupPauseBroadcast", sWP_InternalTimedClass);
		if (this.OnGroupPause != null)
		{
			this.OnGroupPause(sWP_InternalTimedClass);
		}
		PlayTimeControlSound(SoundType.PauseSound, SoundVolume);
	}

	public virtual void SlowDownGroupTime(float _NewTime)
	{
		SetControllerSpeed(_NewTime);
		SWP_InternalTimedClass sWP_InternalTimedClass = new SWP_InternalTimedClass(GroupID, ControllerSpeedPercent);
		BroadcastEvents("OnGroupSlowDownBroadcast", sWP_InternalTimedClass);
		if (this.OnGroupSlowDown != null)
		{
			this.OnGroupSlowDown(sWP_InternalTimedClass);
		}
		PlayTimeControlSound(SoundType.SlowDownSound, SoundVolume);
	}

	public virtual void SpeedUpGroupTime(float _NewTime)
	{
		SetControllerSpeed(_NewTime);
		SWP_InternalTimedClass sWP_InternalTimedClass = new SWP_InternalTimedClass(GroupID, ControllerSpeedPercent);
		BroadcastEvents("OnGroupSpeedUpBroadcast", sWP_InternalTimedClass);
		if (this.OnGroupSpeedUp != null)
		{
			this.OnGroupSpeedUp(sWP_InternalTimedClass);
		}
		PlayTimeControlSound(SoundType.SpeedUpSound, SoundVolume);
	}

	public virtual void ResumeGroupTime()
	{
		SetControllerSpeed(100f);
		SWP_InternalTimedClass sWP_InternalTimedClass = new SWP_InternalTimedClass(GroupID, ControllerSpeedPercent);
		BroadcastEvents("OnGroupResumeBroadcast", sWP_InternalTimedClass);
		if (this.OnGroupResume != null)
		{
			this.OnGroupResume(sWP_InternalTimedClass);
		}
		PlayTimeControlSound(SoundType.BackToNormalSound, SoundVolume);
	}

	private void PlayTimeControlSound(SoundType _SoundType, float _SoundVolume)
	{
		if (EnableSound && !(asAudio == null) && asAudio.enabled)
		{
			if (_SoundType == SoundType.PauseSound && PauseSound != null)
			{
				asAudio.PlayOneShot(PauseSound, _SoundVolume);
			}
			else if (_SoundType == SoundType.SlowDownSound && SlowDownSound != null)
			{
				asAudio.PlayOneShot(SlowDownSound, _SoundVolume);
			}
			else if (_SoundType == SoundType.SpeedUpSound && SpeedUpSound != null)
			{
				asAudio.PlayOneShot(SpeedUpSound, _SoundVolume);
			}
			else if (_SoundType == SoundType.BackToNormalSound && ResumeSound != null)
			{
				asAudio.PlayOneShot(ResumeSound, _SoundVolume);
			}
		}
	}
}
