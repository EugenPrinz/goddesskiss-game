public class NetworkManager
{
	public static void send(BasePackage pack)
	{
		ISN_Singleton<GameCenter_RTM>.instance.SendDataToAll(pack.getBytes(), GK_MatchSendDataMode.RELIABLE);
	}
}
