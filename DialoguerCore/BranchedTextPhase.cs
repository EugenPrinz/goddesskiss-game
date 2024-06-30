using System.Collections.Generic;
using UnityEngine;

namespace DialoguerCore
{
	public class BranchedTextPhase : TextPhase
	{
		public readonly List<string> choices;

		public BranchedTextPhase(string text, List<string> choices, string themeName, bool newWindow, string name, string portrait, float StartScaleX, float StartScaleY, float endScaleX, float endScaleY, bool none_s, bool once_s, bool pingpong_s, float duration_s, float startDelay_s, string metadata, float startPosX, float startPosY, float endPosX, float endPosY, bool none_p, bool once_p, bool pingpong_p, float duration_p, float startDelay_p, float startRotationX, float startRotationY, float startRotationZ, float endRotationX, float endRotationY, float endRotationZ, bool none_r, bool once_r, bool pingpong_r, float duration_r, float startDelay_r, string audio, float audioDelay, Rect rect, List<int> outs, int dialogueID, int nodeID, string createObjPath, bool isSprite, float effStartPosX, float effStartPosY, float effEndPosX, float effEndPosY, bool none_ep, bool once_ep, bool pingpong_ep, float duration_ep, float delay_ep, float effStartRotationX, float effStartRotationY, float effStartRotationZ, float effEndRotationX, float effEndRotationY, float effEndRotationZ, bool none_er, bool once_er, bool pingpong_er, float duration_er, float delay_er, float effStartScaleX, float effStartScaleY, float effEndScaleX, float effEndScaleY, bool none_es, bool once_es, bool pingpong_es, float duration_es, float delay_es, float lifeTime, string soundClip, string voiceClip)
			: base(text, themeName, newWindow, name, portrait, StartScaleX, StartScaleY, endScaleX, endScaleY, none_s, once_s, pingpong_s, duration_s, startDelay_s, metadata, startPosX, startPosY, endPosX, endPosY, none_p, once_p, pingpong_p, duration_p, startDelay_p, startRotationX, startRotationY, startRotationZ, endRotationX, endRotationY, endRotationZ, none_r, once_r, pingpong_r, duration_r, startDelay_r, audio, audioDelay, rect, outs, choices, dialogueID, nodeID, createObjPath, isSprite, effStartPosX, effStartPosY, effEndPosX, effEndPosY, none_ep, once_ep, pingpong_ep, duration_ep, delay_ep, effStartRotationX, effStartRotationY, effStartRotationZ, effEndRotationX, effEndRotationY, effEndRotationZ, none_er, once_er, pingpong_er, duration_er, delay_er, effStartScaleX, effStartScaleY, effEndScaleX, effEndScaleY, none_es, once_es, pingpong_es, duration_es, delay_es, lifeTime, soundClip, voiceClip)
		{
			this.choices = choices;
		}

		public override string ToString()
		{
			string text = string.Empty;
			for (int i = 0; i < choices.Count; i++)
			{
				string text2 = text;
				text = text2 + i + ": " + choices[i] + " : Out " + outs[i] + "\n";
			}
			return "Branched Text Phase" + data.ToString() + "\n" + text;
		}
	}
}
