using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConquestBattleResultUserListItem : UIItemBase
{
	[Serializable]
	public class UserData : UIInnerPartBase
	{
		public UISprite icon;

		public UILabel level;

		public UILabel name;

		public UILabel state;

		public List<UIBattleUnitStatus> commanderList;

		public GameObject winIcon;

		public GameObject loseIcon;

		public TweenPosition startTween;

		public TweenPosition endTween;

		public TweenPosition downTween;

		public TweenRotation rotateTween;

		public UIShake shake;

		[HideInInspector]
		public Protocols.GetConquestBattle.EntryInfo userInfo;

		[HideInInspector]
		public Dictionary<string, Protocols.GetConquestBattle.BattleEntryInfo> battleInfo;

		[HideInInspector]
		public Dictionary<string, Protocols.GetConquestBattle.ResultInfo> resultInfo;

		public void BattleShake()
		{
			for (int i = 0; i < commanderList.Count; i++)
			{
				commanderList[i].GetComponent<UIShake>().Begin();
			}
		}

		public void BattleShake2()
		{
			shake.enabled = true;
		}

		public void BattleResult()
		{
			for (int i = 0; i < userInfo.deck.Count; i++)
			{
				Protocols.ConquestStageUser.Deck deck = userInfo.deck[i];
				int resultHp = 0;
				int maxHp = 1;
				if (resultInfo.ContainsKey(deck.cid))
				{
					resultHp = resultInfo[deck.cid].hp;
					maxHp = battleInfo[deck.cid].maxHp;
				}
				commanderList[i].StartConquestBattleResultHpAnimation(resultHp, maxHp);
			}
		}

		public void StartTween()
		{
			startTween.Play(forward: true);
		}

		public void EndTween()
		{
			shake.Stop();
			shake.enabled = false;
			endTween.Play(forward: true);
		}

		public void DownTween()
		{
			downTween.Play(forward: true);
		}

		public void RotateTween()
		{
			rotateTween.Play(forward: true);
		}
	}

	public UserData blueUser;

	public UserData redUser;

	public GameObject vs;

	public UIPanel panel;

	public UISprite bg;

	public GameObject battleEffect;

	public GameObject boomEffect;

	public GameObject replayBtn;

	private int state;

	[HideInInspector]
	public int prevState;

	public int Set(Protocols.GetConquestBattle.EntryInfo blue, Protocols.GetConquestBattle.EntryInfo red, Protocols.GetConquestBattle.BattleEntry battleEntry, Protocols.GetConquestBattle.Result result, string replayId, int prevState)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(blueUser.root, blue != null);
		UISetter.SetActive(redUser.root, red != null);
		UISetter.SetGameObjectName(replayBtn, $"Replay-{replayId}");
		this.prevState = prevState;
		if (blue != null && red != null)
		{
			if (result.blue.Count > 0)
			{
				state = 1;
			}
			else if (result.red.Count > 0)
			{
				state = 2;
			}
		}
		else if (blue != null)
		{
			state = 1;
		}
		else if (red != null)
		{
			state = 2;
		}
		if (blue != null)
		{
			blueUser.userInfo = blue;
			UISetter.SetLabel(blueUser.level, blue.level);
			UISetter.SetLabel(blueUser.name, blue.name);
			if (battleEntry != null)
			{
				blueUser.resultInfo = result.blue;
				blueUser.battleInfo = battleEntry.blue;
				for (int i = 0; i < blue.deck.Count; i++)
				{
					Protocols.ConquestStageUser.Deck deck = blue.deck[i];
					Protocols.GetConquestBattle.BattleEntryInfo battleEntryInfo = null;
					if (battleEntry.blue.ContainsKey(deck.cid))
					{
						battleEntryInfo = battleEntry.blue[deck.cid];
					}
					RoCommander roCommander = RoCommander.Create(deck.cid, deck.level, deck.grade, deck.cls, deck.costume, 0, deck.marry, deck.transcendence);
					blueUser.commanderList[i].Set(roCommander, battleEntryInfo?.hp ?? 0, battleEntryInfo?.maxHp ?? 1);
				}
			}
			else
			{
				for (int j = 0; j < blue.deck.Count; j++)
				{
					Protocols.ConquestStageUser.Deck deck2 = blue.deck[j];
					RoCommander roCommander2 = RoCommander.Create(deck2.cid, deck2.level, deck2.grade, deck2.cls, deck2.costume, 0, deck2.marry, deck2.transcendence);
					blueUser.commanderList[j].Set(roCommander2, 1, 1);
				}
			}
		}
		if (red != null)
		{
			redUser.userInfo = red;
			UISetter.SetLabel(redUser.level, red.level);
			UISetter.SetLabel(redUser.name, red.name);
			if (battleEntry != null)
			{
				redUser.resultInfo = result.red;
				redUser.battleInfo = battleEntry.red;
				for (int k = 0; k < red.deck.Count; k++)
				{
					Protocols.ConquestStageUser.Deck deck3 = red.deck[k];
					Protocols.GetConquestBattle.BattleEntryInfo battleEntryInfo2 = null;
					if (battleEntry.red.ContainsKey(deck3.cid))
					{
						battleEntryInfo2 = battleEntry.red[deck3.cid];
					}
					RoCommander roCommander3 = RoCommander.Create(deck3.cid, deck3.level, deck3.grade, deck3.cls, deck3.costume, 0, deck3.marry, deck3.transcendence);
					redUser.commanderList[k].Set(roCommander3, battleEntryInfo2?.hp ?? 0, battleEntryInfo2?.maxHp ?? 1);
				}
			}
			else
			{
				for (int l = 0; l < red.deck.Count; l++)
				{
					Protocols.ConquestStageUser.Deck deck4 = red.deck[l];
					RoCommander roCommander4 = RoCommander.Create(deck4.cid, deck4.level, deck4.grade, deck4.cls, deck4.costume, 0, deck4.marry, deck4.transcendence);
					redUser.commanderList[l].Set(roCommander4, 1, 1);
				}
			}
		}
		UISetter.SetActive(blueUser.root, active: false);
		UISetter.SetActive(redUser.root, active: false);
		UISetter.SetActive(vs, active: false);
		UISetter.SetActive(replayBtn, active: false);
		return state;
	}

	public IEnumerator BattleAnimation()
	{
		blueUser.root.transform.localPosition = ((prevState != 1) ? new Vector3(blueUser.root.transform.localPosition.x, blueUser.root.transform.localPosition.y, blueUser.root.transform.localPosition.z) : new Vector3(blueUser.root.transform.localPosition.x, 183f, blueUser.root.transform.localPosition.z));
		blueUser.root.transform.localRotation = ((prevState == 1) ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 90f, 0f, 0f));
		redUser.root.transform.localPosition = ((prevState != 2) ? new Vector3(redUser.root.transform.localPosition.x, redUser.root.transform.localPosition.y, redUser.root.transform.localPosition.z) : new Vector3(redUser.root.transform.localPosition.x, 183f, redUser.root.transform.localPosition.z));
		redUser.root.transform.localRotation = ((prevState == 2) ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 90f, 0f, 0f));
		SoundManager.PlaySFX("BTN_Battle_001");
		if (blueUser.userInfo != null)
		{
			UISetter.SetActive(blueUser.root, active: true);
			if (prevState == 1)
			{
				blueUser.DownTween();
			}
			else
			{
				blueUser.RotateTween();
			}
		}
		if (redUser.userInfo != null)
		{
			UISetter.SetActive(redUser.root, active: true);
			if (prevState == 2)
			{
				redUser.DownTween();
			}
			else
			{
				redUser.RotateTween();
			}
		}
		yield return new WaitForSeconds(0.5f);
		if (blueUser.userInfo != null && redUser.userInfo != null)
		{
			UISetter.SetActive(vs, active: true);
			yield return new WaitForSeconds(0.2f);
			blueUser.StartTween();
			redUser.StartTween();
			yield return new WaitForSeconds(0.5f);
			blueUser.BattleShake2();
			redUser.BattleShake2();
			SoundManager.PlaySFX("BTN_Difficulty_001");
			UISetter.SetActive(battleEffect, active: true);
			yield return new WaitForSeconds(1f);
			UISetter.SetActive(battleEffect, active: false);
			SoundManager.PlaySFX("SE_StartBattle_001");
			UISetter.SetActive(boomEffect, active: true);
			yield return new WaitForSeconds(0.3f);
			blueUser.EndTween();
			redUser.EndTween();
			yield return new WaitForSeconds(0.5f);
			blueUser.BattleResult();
			redUser.BattleResult();
			UISetter.SetActive(replayBtn, active: true);
			yield return new WaitForSeconds(0.2f);
		}
		SoundManager.PlaySFX("BTN_Sweep_001");
		if (state != 3)
		{
			UISetter.SetActive(blueUser.winIcon, state == 1);
			UISetter.SetActive(redUser.winIcon, state == 2);
			yield return new WaitForSeconds(0.2f);
			UISetter.SetActive(blueUser.loseIcon, state != 1);
			UISetter.SetActive(redUser.loseIcon, state != 2);
		}
		else
		{
			UISetter.SetActive(blueUser.winIcon, blueUser.userInfo != null);
			UISetter.SetActive(redUser.winIcon, redUser.userInfo != null);
		}
		yield return null;
	}

	public void Skip()
	{
		if (blueUser.userInfo != null)
		{
			UISetter.SetActive(blueUser.root, active: true);
			blueUser.root.transform.localPosition = new Vector3(blueUser.root.transform.localPosition.x, 0f, blueUser.root.transform.localPosition.z);
			blueUser.root.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		}
		if (redUser.userInfo != null)
		{
			UISetter.SetActive(redUser.root, active: true);
			redUser.root.transform.localPosition = new Vector3(redUser.root.transform.localPosition.x, 0f, redUser.root.transform.localPosition.z);
			redUser.root.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		}
		if (blueUser.userInfo != null && redUser.userInfo != null)
		{
			UISetter.SetActive(vs, active: true);
			UISetter.SetActive(replayBtn, active: true);
			for (int i = 0; i < blueUser.userInfo.deck.Count; i++)
			{
				Protocols.ConquestStageUser.Deck deck = blueUser.userInfo.deck[i];
				int resultHp = 0;
				int maxHp = 1;
				if (blueUser.resultInfo.ContainsKey(deck.cid))
				{
					resultHp = blueUser.resultInfo[deck.cid].hp;
					maxHp = blueUser.battleInfo[deck.cid].maxHp;
				}
				blueUser.commanderList[i].Skip(resultHp, maxHp);
			}
			for (int j = 0; j < blueUser.userInfo.deck.Count; j++)
			{
				Protocols.ConquestStageUser.Deck deck2 = redUser.userInfo.deck[j];
				int resultHp2 = 0;
				int maxHp2 = 1;
				if (redUser.resultInfo.ContainsKey(deck2.cid))
				{
					resultHp2 = redUser.resultInfo[deck2.cid].hp;
					maxHp2 = redUser.battleInfo[deck2.cid].maxHp;
				}
				redUser.commanderList[j].Skip(resultHp2, maxHp2);
			}
		}
		if (state != 3)
		{
			UISetter.SetActive(blueUser.winIcon, state == 1);
			UISetter.SetActive(redUser.winIcon, state == 2);
			UISetter.SetActive(blueUser.loseIcon, state != 1);
			UISetter.SetActive(redUser.loseIcon, state != 2);
		}
		else
		{
			UISetter.SetActive(blueUser.winIcon, blueUser.userInfo != null);
			UISetter.SetActive(redUser.winIcon, redUser.userInfo != null);
		}
	}
}
