using System;

public class GK_TBM_Participant
{
	private string _PlayerId;

	private string _MatchId;

	private DateTime _TimeoutDate;

	private DateTime _LastTurnDate;

	private GK_TurnBasedParticipantStatus _Status;

	private GK_TurnBasedMatchOutcome _MatchOutcome;

	public string PlayerId => _PlayerId;

	public GK_Player Player => GameCenterManager.GetPlayerById(_PlayerId);

	public string MatchId => _MatchId;

	public DateTime TimeoutDate => _TimeoutDate;

	public DateTime LastTurnDate => _LastTurnDate;

	public GK_TurnBasedParticipantStatus Status => _Status;

	public GK_TurnBasedMatchOutcome MatchOutcome => _MatchOutcome;

	public GK_TBM_Participant(string playerId, string status, string outcome, string timeoutDate, string lastTurnDate)
	{
		_PlayerId = playerId;
		_TimeoutDate = DateTime.Parse(timeoutDate);
		_LastTurnDate = DateTime.Parse(lastTurnDate);
		_Status = (GK_TurnBasedParticipantStatus)Convert.ToInt32(status);
		_MatchOutcome = (GK_TurnBasedMatchOutcome)Convert.ToInt32(outcome);
	}

	public void SetOutcome(GK_TurnBasedMatchOutcome outcome)
	{
		if (Player != null)
		{
			_MatchOutcome = outcome;
			ISN_Singleton<GameCenter_TBM>.Instance.UpdateParticipantOutcome(MatchId, (int)_MatchOutcome, _PlayerId);
		}
	}

	public void SetMatchId(string matchId)
	{
		_MatchId = matchId;
	}
}
