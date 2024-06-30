using System;
using System.Collections.Generic;

[Serializable]
public class SoundPocketSfxData
{
	public string pocketName = "Pocket";

	public List<string> clipNames;

	public int autoPrepoolAmount;

	public float autoBaseVolume = 1f;

	public float autoVolumeVariation;

	public float autoPitchVariation;

	public SoundPocketSfxData()
	{
		clipNames = new List<string>();
	}
}
