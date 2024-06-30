using Shared.Battle;

namespace Step
{
	public class SetSkillSp : AbstractStepAction
	{
		public UnitData unitData;

		public int SkillIdx;

		protected M04_Tutorial main;

		protected Unit unit;

		public override bool Enter()
		{
			main = (M04_Tutorial)UIManager.instance.battle.Main;
			if (main == null)
			{
				return false;
			}
			if (unitData == null)
			{
				return false;
			}
			unit = unitData.unit;
			if (unit == null)
			{
				return false;
			}
			if (unit.skills[SkillIdx] == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			unit.skills[SkillIdx]._sp = unit.skills[SkillIdx].SkillDataRow.maxSp;
			unit.skills[SkillIdx]._curSp = unit.skills[SkillIdx]._sp;
			unitData.unitRenderer.ui.SetAnimateSkill(unit.skills[SkillIdx].SkillDataRow.maxSp, unit.skills[SkillIdx]._sp);
			unitData.unitRenderer.ui.OnActivedSkill(enable: true);
			unitData.unitRenderer.ui.UpdateSkillAmount();
			Manager<UIBattleUnitController>.GetInstance().UpdateUI();
		}
	}
}
