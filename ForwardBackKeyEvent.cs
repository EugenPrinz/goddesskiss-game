using System.Collections.Generic;
using UnityEngine;

public class ForwardBackKeyEvent : MonoBehaviour
{
	public static LinkedList<ForwardBackKeyEvent> stack = new LinkedList<ForwardBackKeyEvent>();

	private static int _blockCount = 0;

	public GameObject tweener;

	public List<EventDelegate> onPressed = new List<EventDelegate>();

	public bool lockDestroy;

	private bool _push;

	private bool _call;

	private static bool bDoubleUpdateLock = false;

	private UIPanelBase _uiBase;

	public static bool block => _blockCount > 0;

	public static void Lock()
	{
		_blockCount++;
	}

	public static void Unlock()
	{
		_blockCount--;
	}

	public static void DTouchLock()
	{
		bDoubleUpdateLock = true;
	}

	public static void DTouchUnLock()
	{
		bDoubleUpdateLock = false;
	}

	public static bool IsDoubleTouchLock()
	{
		return bDoubleUpdateLock;
	}

	private void Start()
	{
		_uiBase = GetComponent<UIPanelBase>();
		_Push();
	}

	private void OnEnable()
	{
		_Push();
	}

	private void OnDisable()
	{
		_Pop();
	}

	private void OnDestroy()
	{
		_Pop();
	}

	private void _Push()
	{
		if (!_push)
		{
			stack.AddLast(this);
			_push = true;
			_call = false;
		}
	}

	private void _Pop()
	{
		if (!_push)
		{
			return;
		}
		_push = false;
		if (stack.Count <= 0)
		{
			return;
		}
		if (stack.Last.Value != this)
		{
			for (LinkedListNode<ForwardBackKeyEvent> linkedListNode = stack.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				if (linkedListNode.Value == this)
				{
					stack.Remove(linkedListNode);
					break;
				}
			}
		}
		else
		{
			stack.RemoveLast();
		}
	}

	private void Update()
	{
		if (onPressed != null && stack.Count > 0 && !(stack.Last.Value != this) && !block && !_call && _push && !UIForwardTween.IsTweenTarget(tweener) && (!(_uiBase != null) || !_uiBase.blockClick) && !bDoubleUpdateLock && Input.GetKeyDown(KeyCode.Escape))
		{
			if (!lockDestroy)
			{
				_call = true;
				_Pop();
			}
			EventDelegate.Execute(onPressed);
		}
	}
}
