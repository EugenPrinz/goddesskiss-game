public static class PlayServiceUtil
{
	public static GPAchievementState GetAchievementStateById(int code)
	{
		return code switch
		{
			0 => GPAchievementState.STATE_UNLOCKED, 
			1 => GPAchievementState.STATE_REVEALED, 
			_ => GPAchievementState.STATE_HIDDEN, 
		};
	}

	public static GPAchievementType GetAchievementTypeById(int code)
	{
		if (code == 0)
		{
			return GPAchievementType.TYPE_STANDARD;
		}
		return GPAchievementType.TYPE_INCREMENTAL;
	}
}
