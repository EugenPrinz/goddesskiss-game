using System;

public class GK_Score
{
	private int _Rank;

	private long _Score;

	private long _Context;

	private string _PlayerId;

	private string _LeaderboardId;

	private GK_CollectionType _Collection;

	private GK_TimeSpan _TimeSpan;

	public int Rank => _Rank;

	public long LongScore => _Score;

	public float CurrencyScore => (float)_Score / 100f;

	public float DecimalFloat_1 => (float)_Score / 10f;

	public float DecimalFloat_2 => (float)_Score / 100f;

	public float DecimalFloat_3 => (float)_Score / 100f;

	public long Context => _Context;

	public TimeSpan Minutes => System.TimeSpan.FromMinutes(_Score);

	public TimeSpan Seconds => System.TimeSpan.FromSeconds(_Score);

	public TimeSpan Milliseconds => System.TimeSpan.FromMilliseconds(_Score);

	public string PlayerId => _PlayerId;

	public GK_Player Player => GameCenterManager.GetPlayerById(PlayerId);

	public string LeaderboardId => _LeaderboardId;

	public GK_Leaderboard Leaderboard => GameCenterManager.GetLeaderboard(LeaderboardId);

	public GK_CollectionType Collection => _Collection;

	public GK_TimeSpan TimeSpan => _TimeSpan;

	[Obsolete("rank is deprecated, plase use Rank instead")]
	public int rank => _Rank;

	[Obsolete("score is deprecated, plase use LongScore instead")]
	public long score => _Score;

	[Obsolete("playerId is deprecated, plase use PlayerId instead")]
	public string playerId => _PlayerId;

	[Obsolete("leaderboardId is deprecated, plase use LeaderboardId instead")]
	public string leaderboardId => _LeaderboardId;

	[Obsolete("timeSpan is deprecated, plase use TimeSpan instead")]
	public GK_TimeSpan timeSpan => _TimeSpan;

	[Obsolete("collection is deprecated, plase use Collection instead")]
	public GK_CollectionType collection => _Collection;

	public GK_Score(long vScore, int vRank, long vContext, GK_TimeSpan vTimeSpan, GK_CollectionType sCollection, string lid, string pid)
	{
		_Score = vScore;
		_Rank = vRank;
		_Context = vContext;
		_PlayerId = pid;
		_LeaderboardId = lid;
		_TimeSpan = vTimeSpan;
		_Collection = sCollection;
	}
}
