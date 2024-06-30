using UnityEngine;
using UnityEngine.EventSystems;

namespace Unitycoding
{
	public class DistanceTriggerHandler : MonoBehaviour
	{
		public enum TriggerType
		{
			OnMouseDown,
			OnTriggerEnter,
			All
		}

		[Tag]
		public string triggerTag = "Player";

		public TriggerType triggerType;

		public float distance = 2f;

		protected GameObject triggerGameObject;

		protected virtual void Start()
		{
			GameObject gameObject = new GameObject("Distance Trigger");
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			DistanceTrigger distanceTrigger = gameObject.AddComponent<DistanceTrigger>();
			distanceTrigger.triggerTag = triggerTag;
			distanceTrigger.distance = distance;
			distanceTrigger.onTriggerEnter = new DistanceTrigger.TriggerEvent();
			distanceTrigger.onTriggerEnter.AddListener(delegate(GameObject go)
			{
				triggerGameObject = go;
				base.enabled = true;
				if (triggerType == TriggerType.OnTriggerEnter || triggerType == TriggerType.All)
				{
					Execute();
				}
			});
			distanceTrigger.onTriggerExit = new DistanceTrigger.TriggerEvent();
			distanceTrigger.onTriggerExit.AddListener(delegate(GameObject go)
			{
				triggerGameObject = go;
				base.enabled = false;
			});
			base.enabled = false;
		}

		protected virtual void Execute()
		{
		}

		private void OnMouseDown()
		{
			if (!base.enabled || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
			{
				OutOfRange();
			}
			else if (triggerType == TriggerType.OnMouseDown || triggerType == TriggerType.All)
			{
				Execute();
			}
		}

		protected virtual void OutOfRange()
		{
		}
	}
}
