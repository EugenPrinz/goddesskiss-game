using System;
using System.Collections.Generic;

public class GK_RTM_Match
{
	private int _ExpectedPlayerCount;

	private List<GK_Player> _Players = new List<GK_Player>();

	public int ExpectedPlayerCount => _ExpectedPlayerCount;

	public List<GK_Player> Players => _Players;

	public GK_RTM_Match(string matchData)
	{
		string[] array = matchData.Split(new string[1] { "|%|" }, StringSplitOptions.None);
		_ExpectedPlayerCount = Convert.ToInt32(array[0]);
		string[] array2 = IOSNative.ParseArray(array[1]);
		string[] array3 = array2;
		foreach (string playerID in array3)
		{
			GK_Player playerById = GameCenterManager.GetPlayerById(playerID);
			_Players.Add(playerById);
		}
	}
}
