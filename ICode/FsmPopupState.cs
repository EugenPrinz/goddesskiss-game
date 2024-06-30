using System;
using UnityEngine;

namespace ICode
{
	[Serializable]
	public class FsmPopupState : FsmVariable
	{
		public enum EPopupState
		{
			NONE,
			CAMP,
			GACHA,
			SUPPLY_BASE,
			SUPPLY_BASE_FORMATION,
			WORLD_MAP,
			WARMEMORIAL,
			SITUATION_ROOM,
			SITUATION_ROOM_SEA_FLOOREXPLORE,
			UNIT_LABORATORY,
			UNIT_LABORATORY_DETAIL,
			MILITARY_ACADEMY,
			HEADQUARTERS,
			SWEEP,
			METROBANK,
			COMMANDER_DETAIL,
			BATTLE_READY,
			BATTLE_COMMANDER_SELECT,
			SITUATION_ROOM_SEA_ROBBER_SWEEP,
			COMMANDERLIST
		}

		[SerializeField]
		private EPopupState value;

		public EPopupState Value
		{
			get
			{
				return value;
			}
			set
			{
				SetValue(value);
			}
		}

		public override Type VariableType => typeof(int);

		public override void SetValue(object value)
		{
			this.value = (EPopupState)value;
			if (m_OnVariableChange != null)
			{
				m_OnVariableChange.Invoke(this.value);
			}
		}

		public override object GetValue()
		{
			return value;
		}

		public static implicit operator EPopupState(FsmPopupState value)
		{
			return value.Value;
		}

		public static implicit operator FsmPopupState(EPopupState value)
		{
			FsmPopupState fsmPopupState = ScriptableObject.CreateInstance<FsmPopupState>();
			fsmPopupState.SetValue(value);
			return fsmPopupState;
		}
	}
}
