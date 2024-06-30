using UnityEngine;

public class SoundPocketSfx : MonoBehaviour
{
	public SoundPocketSfxData data;

	public void Set(SoundPocket pocket)
	{
		data = new SoundPocketSfxData();
		data.pocketName = pocket.pocketName;
		for (int i = 0; i < pocket.pocketClips.Count; i++)
		{
			data.clipNames.Add(pocket.pocketClips[i].name);
		}
	}
}
