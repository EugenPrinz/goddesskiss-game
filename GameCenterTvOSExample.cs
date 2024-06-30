using UnityEngine;

public class GameCenterTvOSExample : MonoBehaviour
{
	private int hiScore = 200;

	private string TEST_LEADERBOARD_1 = "your.ios.leaderbord1.id";

	private string TEST_LEADERBOARD_2 = "combined.board.1";

	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";

	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";

	private static bool IsInitialized;

	private static long LB2BestScores;

	private void Start()
	{
		GameCenterManager.OnAuthFinished += OnAuthFinished;
		GameCenterManager.OnScoreSubmitted += OnScoreSubmitted;
		GameCenterManager.OnAchievementsProgress += HandleOnAchievementsProgress;
		GameCenterManager.OnAchievementsReset += HandleOnAchievementsReset;
		GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;
		GameCenterManager.RegisterAchievement(TEST_ACHIEVEMENT_1_ID);
		GameCenterManager.RegisterAchievement(TEST_ACHIEVEMENT_2_ID);
		GameCenterManager.Init();
	}

	private void OnAuthFinished(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.Player.Id + "\nAlias: " + GameCenterManager.Player.Alias);
			GameCenterManager.LoadLeaderboardInfo(TEST_LEADERBOARD_1);
		}
		else
		{
			IOSNativePopUpManager.showMessage("Game Center ", "Player authentication failed");
		}
	}

	public void ShowAchivemnets()
	{
		GameCenterManager.ShowAchievements();
	}

	public void SubmitAchievement()
	{
		GameCenterManager.SubmitAchievement(GameCenterManager.GetAchievementProgress(TEST_ACHIEVEMENT_1_ID) + 2.432f, TEST_ACHIEVEMENT_1_ID);
	}

	public void ResetAchievements()
	{
		GameCenterManager.ResetAchievements();
	}

	public void ShowLeaderboards()
	{
		GameCenterManager.ShowLeaderboards();
	}

	public void ShowLeaderboardByID()
	{
		GameCenterManager.OnFriendsListLoaded += delegate
		{
		};
		GameCenterManager.RetrieveFriends();
	}

	public void ReportScore()
	{
		hiScore++;
		GameCenterManager.ReportScore(hiScore, TEST_LEADERBOARD_1, 17L);
	}

	private void OnScoreSubmitted(GK_LeaderboardResult result)
	{
		if (result.IsSucceeded)
		{
			GK_Score currentPlayerScore = result.Leaderboard.GetCurrentPlayerScore(GK_TimeSpan.ALL_TIME, GK_CollectionType.GLOBAL);
			IOSNativePopUpManager.showMessage("Leaderboard " + currentPlayerScore.LongScore, "Score: " + currentPlayerScore.LongScore + "\nRank:" + currentPlayerScore.Rank);
		}
	}

	private void OnAchievementsLoaded(ISN_Result result)
	{
		ISN_Logger.Log("OnAchievementsLoaded");
		ISN_Logger.Log(result.IsSucceeded);
		if (!result.IsSucceeded)
		{
			return;
		}
		ISN_Logger.Log("Achievements were loaded from iOS Game Center");
		foreach (GK_AchievementTemplate achievement in GameCenterManager.Achievements)
		{
			ISN_Logger.Log(achievement.Id + ":  " + achievement.Progress);
		}
	}

	private void HandleOnAchievementsReset(ISN_Result obj)
	{
		ISN_Logger.Log("All Achievements were reset");
	}

	private void HandleOnAchievementsProgress(GK_AchievementProgressResult result)
	{
		if (result.IsSucceeded)
		{
			GK_AchievementTemplate achievement = result.Achievement;
			ISN_Logger.Log(achievement.Id + ":  " + achievement.Progress);
		}
	}
}
