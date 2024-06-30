using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialoguerEditor
{
	[Serializable]
	public class DialogueEditorPhaseObject
	{
		public int id;

		public DialogueEditorPhaseTypes type;

		public object paramaters;

		public string theme;

		public Vector2 position;

		public List<int> outs;

		public bool advanced;

		public string metadata;

		public float startPosX;

		public float startPosY;

		public float endPosX;

		public float endPosY;

		public bool None_p;

		public bool Once_p;

		public bool PingPong_p;

		public float duration_p;

		public float startDelay_p;

		public float startRotationX;

		public float startRotationY;

		public float startRotationZ;

		public float endRotationX;

		public float endRotationY;

		public float endRotationZ;

		public bool None_r;

		public bool Once_r;

		public bool PingPong_r;

		public float duration_r;

		public float startDelay_r;

		public string text;

		public string name;

		public string portrait;

		public float startScaleX;

		public float startScaleY;

		public float endScaleX;

		public float endScaleY;

		public bool None_s;

		public bool Once_s;

		public bool PingPong_s;

		public float duration_s;

		public float startDelay_s;

		public string audio;

		public float audioDelay;

		public Rect rect;

		public bool newWindow;

		public List<string> choices;

		public DialogueEditorWaitTypes waitType;

		public float waitDuration;

		public VariableEditorScopes variableScope;

		public VariableEditorTypes variableType;

		public int variableId;

		public Vector2 variableScrollPosition;

		public VariableEditorSetEquation variableSetEquation;

		public string variableSetValue;

		public VariableEditorGetEquation variableGetEquation;

		public string variableGetValue;

		public string messageName;

		public string CreateObejctPath;

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

		public float duration;

		public float backgraound_r;

		public float backgraound_g;

		public float backgraound_b;

		public float backgraound_a;

		public string voiceClip;

		public DialogueEditorPhaseObject()
		{
			type = DialogueEditorPhaseTypes.EmptyPhase;
			position = Vector2.zero;
			text = string.Empty;
			outs = new List<int>();
			choices = new List<string>();
			waitType = DialogueEditorWaitTypes.Seconds;
		}

		public void addNewOut()
		{
			outs.Add(-1);
		}

		public void removeOut()
		{
			outs.RemoveAt(outs.Count - 1);
		}

		public void addNewChoice()
		{
			addNewOut();
			choices.Add(string.Empty);
		}

		public void removeChoice()
		{
			removeOut();
			choices.RemoveAt(choices.Count - 1);
		}
	}
}
