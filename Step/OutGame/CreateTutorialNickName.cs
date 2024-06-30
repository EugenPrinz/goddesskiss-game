using UnityEngine;

namespace Step.OutGame
{
	public class CreateTutorialNickName : AbstractStepAction
	{
		public IntData nextStep;

		protected UIReceiveUserString popup;

		protected string requestNickName;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			if (nextStep == null)
			{
				return false;
			}
			if (nextStep.value <= RemoteObjectManager.instance.localUser.tutorialStep)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			RoLocalUser localUser = RemoteObjectManager.instance.localUser;
			popup = UIPopup.Create<UIReceiveUserString>("InputUserString");
			popup.SetDefault(localUser.nickname);
			popup.SetLimitLength(10);
			popup.Set(localization: false, "이름 변경", "최대 10글자", null, "확인", null, null);
			popup.onClick = delegate(GameObject popupSender)
			{
				string text = popupSender.name;
				if (text == "OK")
				{
					string nickName = (requestNickName = popup.inputLabel.text);
					RemoteObjectManager.instance.RequestSetNickNameFromTutorial(nickName, nextStep.value);
				}
			};
		}

		protected override void OnUpdate()
		{
			if (RemoteObjectManager.instance.localUser.tutorialStep == nextStep.value)
			{
				RemoteObjectManager.instance.localUser.nickname = requestNickName;
				UIManager.instance.RefreshOpenedUI();
				popup.Close();
				_isFinish = true;
			}
		}
	}
}
