using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebUtil
{
	public static Hashtable DefaultHeader()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["Accept"] = "application/json";
		hashtable["Content-Type"] = "application/json";
		hashtable["User-Agent"] = "(UnityPlayer)";
		return hashtable;
	}

	public static bool CreateHeaderToJSON(ref Dictionary<string, string> header, string sessionID)
	{
		if (header == null)
		{
			header = new Dictionary<string, string>();
		}
		header.Add("Accept", "application/json");
		header.Add("Accept-Encoding", "gzip");
		header.Add("Content-Type", "application/json");
		header.Add("User-Agent", sessionID);
		return true;
	}

	public static bool CreateHeaderToSession(ref Hashtable header, string sessionID)
	{
		if (header == null)
		{
			header = new Hashtable();
		}
		header["Accept"] = "application/json";
		header["Accept-Encoding"] = "gzip";
		header["Content-Type"] = "application/json";
		header["Session-Id"] = sessionID;
		return true;
	}

	public static bool CreateHeaderToGameCode(ref Hashtable header, string strGameCode)
	{
		if (header == null)
		{
			header = new Hashtable();
		}
		header.Add("ContentType", "application/json");
		header.Add("Accept", "application/json");
		header.Add("GameCode", strGameCode);
		return true;
	}

	public static bool InternetConnected()
	{
		bool result = false;
		switch (Application.internetReachability)
		{
		case NetworkReachability.ReachableViaLocalAreaNetwork:
			result = true;
			break;
		case NetworkReachability.ReachableViaCarrierDataNetwork:
			result = true;
			break;
		case NetworkReachability.NotReachable:
			result = false;
			break;
		}
		return result;
	}
}
