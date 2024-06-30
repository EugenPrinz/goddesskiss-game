using System.Collections;
using System.Collections.Generic;

namespace Step.OutGame
{
	public class GetMissionData : AbstractStepAction
	{
		public IntData missionKey;

		public IntData retMissionPoint;

		public BoolData retMissionComplete;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			if (missionKey == null)
			{
				return false;
			}
			if (retMissionPoint == null)
			{
				return false;
			}
			if (retMissionComplete == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			JsonRpcClient.instance.RegisterClass(GetType());
			List<string> list = new List<string>();
			list.Add("dlms");
			JsonRpcClient.instance.SendRequest(this, "GetMissionInfo", list);
		}

		protected override void OnExit()
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
				if (result.missionList[i].missionId == missionKey.value)
				{
					retMissionPoint.value = result.missionList[i].point;
					if (result.missionList[i].complete > 0 && result.missionList[i].receive <= 0)
					{
						retMissionComplete.value = true;
					}
					_isFinish = true;
					break;
				}
			}
			yield return null;
		}
	}
}
