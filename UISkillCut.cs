using UnityEngine;

public class UISkillCut : MonoBehaviour
{
	public GameObjectPool pool;

	public Transform targetView;

	private void OnEnable()
	{
		while (base.transform.childCount > 1)
		{
			Transform child = base.transform.GetChild(1);
			pool.Release(child.gameObject);
		}
	}

	public void Create(CutInDataSet cutInData)
	{
		CutInSkill cutInSkill = pool.Create<CutInSkill>(targetView);
		cutInSkill.Init(cutInData);
	}
}
