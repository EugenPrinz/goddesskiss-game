using System.Collections.Generic;
using DialoguerCore;
using UnityEngine;

public struct DialoguerTextData
{
	public readonly int dialogueID;

	public readonly int nodeID;

	public readonly string rawText;

	public readonly string theme;

	public readonly bool newWindow;

	public readonly string name;

	public readonly string portrait;

	public readonly float startScaleX;

	public readonly float startScaleY;

	public readonly float endScaleX;

	public readonly float endScaleY;

	public readonly bool None_s;

	public readonly bool Once_s;

	public readonly bool PingPong_s;

	public readonly float duration_s;

	public readonly float startDelay_s;

	public readonly string metadata;

	public readonly float startPosX;

	public readonly float startPosY;

	public readonly float endPosX;

	public readonly float endPosY;

	public readonly bool None_p;

	public readonly bool Once_p;

	public readonly bool PingPong_p;

	public readonly float duration_p;

	public readonly float startDelay_p;

	public readonly float startRotationX;

	public readonly float startRotationY;

	public readonly float startRotationZ;

	public readonly float endRotationX;

	public readonly float endRotationY;

	public readonly float endRotationZ;

	public readonly bool None_r;

	public readonly bool Once_r;

	public readonly bool PingPong_r;

	public readonly float duration_r;

	public readonly float startDelay_r;

	public readonly string audio;

	public readonly float audioDelay;

	public readonly Rect rect;

	public readonly string[] choices;

	private string _cachedText;

	public string strEmotion;

	public string strArgumnet;

	public string createObjectPath;

	public bool isSprite;

	public float EffstartPosX;

	public float EffstartPosY;

	public float EffEndPosX;

	public float EffEndPosY;

	public bool EffNone_p;

	public bool EffOnce_p;

	public bool EffPingPong_p;

	public float EffDuration_p;

	public float EffStartDelay_p;

	public float EffStartRotationX;

	public float EffStartRotationY;

	public float EffStartRotationZ;

	public float EffEndRotationX;

	public float EffEndRotationY;

	public float EffEndRotationZ;

	public bool EffNone_r;

	public bool EffOnce_r;

	public bool EffPingPong_r;

	public float EffDuration_r;

	public float EffStartDelay_r;

	public float EffStartScaleX;

	public float EffStartScaleY;

	public float EffEndScaleX;

	public float EffEndScaleY;

	public bool EffNone_s;

	public bool EffOnce_s;

	public bool EffPingPong_s;

	public float EffDuration_s;

	public float EffStartDelay_s;

	public float EffLifeTime;

	public string EffSoundClip;

	public string VoiceClip;

	public string text
	{
		get
		{
			if (_cachedText == null)
			{
				_cachedText = DialoguerUtils.insertTextPhaseStringVariables(rawText);
				strEmotion = DialoguerUtils.sEmotion;
				strArgumnet = DialoguerUtils.sArgument;
			}
			return _cachedText;
		}
	}

	public bool usingPositionRect => rect.x != 0f || rect.y != 0f || rect.width != 0f || rect.height != 0f;

	public DialoguerTextPhaseType windowType => (choices != null) ? DialoguerTextPhaseType.BranchedText : DialoguerTextPhaseType.Text;

	public DialoguerTextData(string text, string themeName, bool newWindow, string name, string portrait, float startScaleX, float startScaleY, float endScaleX, float endScaleY, bool none_s, bool once_s, bool pingpong_s, float duration_s, float startDelay_s, string metadata, float startPosX, float startPosY, float endPosX, float endPosY, bool none_p, bool once_p, bool pingpong_p, float duration_p, float startDelay_p, float startRotationX, float startRotationY, float startRotationZ, float endRotationX, float endRotationY, float endRotationZ, bool none_r, bool once_r, bool pingpong_r, float duration_r, float startDelay_r, string audio, float audioDelay, Rect rect, List<string> choices, int dialogueID, int nodeID, string createObjPath, bool isSprite, float effStartPosX, float effStartPosY, float effEndPosX, float effEndPosY, bool none_ep, bool once_ep, bool pingpong_ep, float duration_ep, float delay_ep, float effStartRotationX, float effStartRotationY, float effStartRotationZ, float effEndRotationX, float effEndRotationY, float effEndRotationZ, bool none_er, bool once_er, bool pingpong_er, float duration_er, float delay_er, float effStartScaleX, float effStartScaleY, float effEndScaleX, float effEndScaleY, bool none_es, bool once_es, bool pingpong_es, float duration_es, float delay_es, float lifeTime, string clip, string voiceClip)
	{
		this.dialogueID = dialogueID;
		this.nodeID = nodeID;
		rawText = text;
		theme = themeName;
		this.newWindow = newWindow;
		this.name = name;
		this.portrait = portrait;
		this.metadata = metadata;
		this.audio = audio;
		this.audioDelay = audioDelay;
		this.rect = new Rect(rect.x, rect.y, rect.width, rect.height);
		if (choices != null)
		{
			string[] array = choices.ToArray();
			this.choices = array.Clone() as string[];
		}
		else
		{
			this.choices = null;
		}
		strArgumnet = null;
		strEmotion = null;
		_cachedText = null;
		createObjectPath = createObjPath;
		this.isSprite = isSprite;
		EffstartPosX = effStartPosX;
		EffstartPosY = effStartPosY;
		EffEndPosX = effEndPosX;
		EffEndPosY = effEndPosY;
		EffNone_p = none_ep;
		EffOnce_p = once_ep;
		EffPingPong_p = pingpong_ep;
		EffDuration_p = duration_ep;
		EffStartDelay_p = delay_ep;
		EffStartRotationX = effStartRotationX;
		EffStartRotationY = effStartRotationY;
		EffStartRotationZ = effStartRotationZ;
		EffEndRotationX = effEndRotationX;
		EffEndRotationY = effEndRotationY;
		EffEndRotationZ = effEndRotationZ;
		EffNone_r = none_er;
		EffOnce_r = once_er;
		EffPingPong_r = pingpong_er;
		EffDuration_r = duration_er;
		EffStartDelay_r = delay_er;
		EffStartScaleX = effStartScaleX;
		EffStartScaleY = effStartScaleY;
		EffEndScaleX = effEndScaleX;
		EffEndScaleY = effEndScaleY;
		EffNone_s = none_es;
		EffOnce_s = once_es;
		EffPingPong_s = pingpong_es;
		EffDuration_s = duration_es;
		EffStartDelay_s = delay_es;
		this.startPosX = startPosX;
		this.startPosY = startPosY;
		this.endPosX = endPosX;
		this.endPosY = endPosY;
		None_p = none_p;
		Once_p = once_p;
		PingPong_p = pingpong_p;
		this.duration_p = duration_p;
		this.startDelay_p = startDelay_p;
		this.startScaleX = startScaleX;
		this.startScaleY = startScaleY;
		this.endScaleX = endScaleX;
		this.endScaleY = endScaleY;
		None_s = none_s;
		Once_s = once_s;
		PingPong_s = pingpong_s;
		this.duration_s = duration_s;
		this.startDelay_s = startDelay_s;
		this.startRotationX = startRotationX;
		this.startRotationY = startRotationY;
		this.startRotationZ = startRotationZ;
		this.endRotationX = endRotationX;
		this.endRotationY = endRotationY;
		this.endRotationZ = endRotationZ;
		None_r = none_r;
		Once_r = once_r;
		PingPong_r = pingpong_r;
		this.duration_r = duration_r;
		this.startDelay_r = startDelay_r;
		EffLifeTime = lifeTime;
		EffSoundClip = clip;
		VoiceClip = voiceClip;
	}

	public override string ToString()
	{
		return "\nTheme ID: " + theme + "\nNew Window: " + newWindow.ToString() + "\nName: " + name + "\nPortrait: " + portrait + "\nMetadata: " + metadata + "\nAudio Clip: " + audio + "\nAudio Delay: " + audioDelay.ToString() + "\nRect: " + rect.ToString() + "\nRaw Text: " + rawText + "\nDialogue ID:" + dialogueID + "\nNode ID:" + nodeID + "\nArgumnet:" + strArgumnet + "\nEmotion: " + strEmotion + "\nObjectPath: " + createObjectPath;
	}
}
