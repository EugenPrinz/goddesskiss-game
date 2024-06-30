using System;
using System.Collections;
using System.Collections.Generic;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetMissionCompleteState : StateAction
	{
		public FsmInt missionKey;

		[Shared]
		public FsmBool store;

		public override void OnEnter()
		{
			JsonRpcClient.instance.RegisterClass(GetType());
			List<string> list = new List<string>();
			list.Add("dlms");
			JsonRpcClient.instance.SendRequest(this, "GetMissionInfo", list);
		}

		public override void OnExit()
		{
			JsonRpcClient.instance.UnregisterClass(GetType());
		}

		private IEnumerator OnJsonRpcRequestError(JsonRpcClient.Request request, string result, int code, string message)
		{
			yield return null;
		}

		[JsonRpcClient.Request("http://gk.flerogames.com/checkData.php", "1103", true, true)]
		public void GetMissionInfo(List<string> type)
		{
		}

		private IEnumerator GetMissionInfoResult(JsonRpcClient.Request request, Protocols.MissionInfo result)
		{
			if (result.missionList == null)
			{
				yield break;
			}
			for (int i = 0; i < result.missionList.Count; i++)
			{
				if (result.missionList[i].missionId == missionKey.Value)
				{
					if (result.missionList[i].complete > 0 && result.missionList[i].receive <= 0)
					{
						store.Value = true;
					}
					Finish();
					break;
				}
			}
			yield return null;
		}
	}
}
