using System;
using System.Collections.Generic;
using System.Reflection;
using Shared.Regulation;
using UnityEngine;

public class UIPanelBase : MonoBehaviour
{
	public delegate void OnClickDelegate(GameObject sender);

	private static LinkedList<GameObject> _OpenedUIList = new LinkedList<GameObject>();

	private static Dictionary<UIPanel, UIPanelBase> _holdDepthTargetDict = new Dictionary<UIPanel, UIPanelBase>();

	private static Dictionary<UIPanel, int> _originalDepthDict = new Dictionary<UIPanel, int>();

	public OnClickDelegate onClick;

	private int _lockClickCount;

	private readonly int addPanelDepth = 100;

	public bool dontMoveToFront;

	public bool holdDepth;

	public GameObject root;

	public UIPanel panel;

	public int originalDepth;

	protected List<UIInnerPartBase> _InnerPartsList = new List<UIInnerPartBase>();

	protected bool _popupState;

	private static readonly float zPosOffset = 2f;

	protected RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	protected Regulation regulation => RemoteObjectManager.instance.regulation;

	protected RemoteObjectManager network => RemoteObjectManager.instance;

	protected UIManager.World uiWorld => UIManager.instance.world;

	protected UIManager.Battle uiBattle => UIManager.instance.battle;

	public bool blockClick => _lockClickCount > 0;

	public bool isActive => (!(root == null)) ? root.activeSelf : base.gameObject.activeSelf;

	protected virtual void Awake()
	{
		if (root != null)
		{
			panel = root.GetComponent<UIPanel>();
			if (panel != null)
			{
				originalDepth = panel.depth;
			}
			if (holdDepth && panel != null)
			{
				_holdDepthTargetDict.Add(panel, this);
			}
		}
		_InnerPartsList = _CollectInnerParts();
	}

	public void LockClick()
	{
		_lockClickCount++;
	}

	public void UnlockClick()
	{
		_lockClickCount--;
		if (_lockClickCount < 0)
		{
			_lockClickCount = 0;
		}
	}

	protected virtual void OnEnable()
	{
		if (!dontMoveToFront)
		{
		}
	}

	protected virtual void OnDisable()
	{
		if (panel != null)
		{
			panel.depth = originalDepth;
		}
		if (dontMoveToFront)
		{
			return;
		}
		for (LinkedListNode<GameObject> linkedListNode = _OpenedUIList.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
		{
			if (linkedListNode.Value == root)
			{
				_OpenedUIList.Remove(linkedListNode);
				break;
			}
		}
	}

	protected void SetPopupState(bool state)
	{
		_popupState = state;
		if (_popupState)
		{
			OnEnablePopup();
		}
		else
		{
			OnDisablePopup();
		}
	}

	protected virtual void OnEnablePopup()
	{
	}

	protected virtual void OnDisablePopup()
	{
	}

	private void _CollectOnClickStatistics(string key)
	{
	}

	public virtual void OnClick(GameObject sender)
	{
		if (!blockClick)
		{
			string key = sender.name;
			_CollectOnClickStatistics(key);
			if (onClick != null)
			{
				onClick(sender);
			}
		}
	}

	protected void SendOnClickToInnerParts(GameObject sender)
	{
		_InnerPartsList.ForEach(delegate(UIInnerPartBase part)
		{
			if (part.isActive)
			{
				part.OnClick(sender, this);
			}
		});
	}

	protected void SendOnInitToInnerParts()
	{
		_InnerPartsList.ForEach(delegate(UIInnerPartBase part)
		{
			part.OnInit(this);
		});
	}

	protected void SendOnRefreshToInnerParts()
	{
		_InnerPartsList.ForEach(delegate(UIInnerPartBase part)
		{
			if (part.isActive)
			{
				part.OnRefresh();
			}
		});
	}

	public virtual void OnRefresh()
	{
	}

	public void MoveToFront()
	{
		if (_OpenedUIList.Count <= 0 || !(_OpenedUIList.Last.Value == root))
		{
			_OpenedUIList.AddLast(root);
			NormalizePanelDepths();
		}
	}

	public void NormalizePanelDepths()
	{
		UIRoot uIRoot = UIRoot.list[0];
		UIPanel[] componentsInChildren = uIRoot.GetComponentsInChildren<UIPanel>();
		int num = componentsInChildren.Length;
		if (num <= 0)
		{
			return;
		}
		Array.Sort(componentsInChildren, UIPanel.CompareFunc);
		int num2 = 0;
		int depth = componentsInChildren[0].depth;
		for (int i = 0; i < num; i++)
		{
			UIPanel uIPanel = componentsInChildren[i];
			if (!_originalDepthDict.ContainsKey(uIPanel))
			{
				_originalDepthDict.Add(uIPanel, uIPanel.depth);
			}
			if (_holdDepthTargetDict.ContainsKey(uIPanel))
			{
				uIPanel.depth = _originalDepthDict[uIPanel];
				break;
			}
			if (uIPanel.depth == depth)
			{
				uIPanel.depth = num2;
				continue;
			}
			depth = uIPanel.depth;
			num2 = (uIPanel.depth = num2 + 1);
		}
	}

	protected static void SetPanelZPosition(UIPanel p, float originZPos)
	{
		Vector3 position = p.transform.position;
		position.z = originZPos - (float)p.depth * zPosOffset;
	}

	public List<UIPanelBase> GetPanelBaseList(bool includeSelf)
	{
		return FindContainedPanelBase(this, includeSelf);
	}

	public static List<UIPanelBase> FindContainedPanelBase(object parent, bool includeParent = true)
	{
		List<UIPanelBase> list = new List<UIPanelBase>();
		if (parent == null)
		{
			return list;
		}
		Type type = parent.GetType();
		FieldInfo[] fields = type.GetFields();
		Type typeFromHandle = typeof(UIPanelBase);
		if (includeParent && type.IsSubclassOf(typeFromHandle))
		{
			list.Add(parent as UIPanelBase);
		}
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.GetType().IsSubclassOf(typeFromHandle))
			{
				UIPanelBase uIPanelBase = fieldInfo.GetValue(parent) as UIPanelBase;
				if (uIPanelBase != null)
				{
					list.Add(uIPanelBase);
				}
			}
		}
		return list;
	}

	private List<UIInnerPartBase> _CollectInnerParts()
	{
		List<UIInnerPartBase> list = new List<UIInnerPartBase>();
		Type type = GetType();
		FieldInfo[] fields = type.GetFields();
		for (int i = 0; i < fields.Length; i++)
		{
			if (fields[i].FieldType.IsSubclassOf(typeof(UIInnerPartBase)))
			{
				list.Add(fields[i].GetValue(this) as UIInnerPartBase);
			}
		}
		return list;
	}
}
