using System;
using System.Collections.Generic;
using UnityEngine;

public class UIListViewBase : MonoBehaviour
{
	public UIScrollView scrollView;

	public UIGrid grid;

	public UIItemBase itemPrefab;

	public Transform collectRoot;

	public bool autoDeselect = true;

	public List<UIItemBase> itemList { get; protected set; }

	public string itemIdPrefix { get; set; }

	public UIItemBase lastSelectedItem { get; private set; }

	public string lastSelectedItemPureId
	{
		get
		{
			if (lastSelectedItem == null)
			{
				return null;
			}
			return GetPureId(lastSelectedItem.name);
		}
	}

	private GameObject _itemParent
	{
		get
		{
			if (grid != null)
			{
				return grid.gameObject;
			}
			if (collectRoot != null)
			{
				return collectRoot.gameObject;
			}
			return base.gameObject;
		}
	}

	private void _OnInitItem(GameObject go, int wrapIndex, int realIndex)
	{
	}

	public void CreateItem(int count, string idPrefix, Action<UIItemBase, int> initMember = null)
	{
		if (idPrefix == null)
		{
			idPrefix = string.Empty;
		}
		itemIdPrefix = idPrefix;
		ResizeItemList(count);
		for (int i = 0; i < count; i++)
		{
			UIItemBase uIItemBase = itemList[i];
			UISetter.SetGameObjectName(uIItemBase.gameObject, idPrefix + i);
			initMember?.Invoke(uIItemBase, i);
		}
	}

	public void CollectExistItem(string findItemIdPrefix, Action<UIItemBase, int> initMember = null)
	{
		if (itemList != null)
		{
			itemList.Clear();
		}
		else
		{
			itemList = new List<UIItemBase>();
		}
		Transform transform = collectRoot;
		if (transform == null)
		{
			if (grid == null)
			{
				return;
			}
			transform = grid.transform;
		}
		itemIdPrefix = findItemIdPrefix;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name.StartsWith(findItemIdPrefix))
			{
				UIItemBase component = child.GetComponent<UIItemBase>();
				if (!(component == null))
				{
					itemList.Add(component);
					initMember?.Invoke(component, i);
				}
			}
		}
	}

	public UIItemBase FindItem(Predicate<UIItemBase> match)
	{
		return itemList.Find(match);
	}

	public List<UIItemBase> FindAllItem(Predicate<UIItemBase> match)
	{
		return itemList.FindAll(match);
	}

	public UIItemBase FindItem(string id)
	{
		if (string.IsNullOrEmpty(id) || itemList == null)
		{
			return null;
		}
		id = GetFullItemId(id);
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemList[i].gameObject.name == id)
			{
				return itemList[i];
			}
		}
		return null;
	}

	public UIItemBase FindNearItem(string id)
	{
		if (string.IsNullOrEmpty(id) || itemList == null)
		{
			return null;
		}
		id = GetFullItemId(id);
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemList[i].gameObject.name == id)
			{
				if (i + 1 < itemList.Count)
				{
					return itemList[i + 1];
				}
				if (i - 1 >= 0)
				{
					return itemList[i - 1];
				}
				return null;
			}
		}
		return null;
	}

	public void SetSelection(string id, bool selected)
	{
		if (itemList == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(id))
		{
			itemList.ForEach(delegate(UIItemBase target)
			{
				target.SetSelection(selected);
			});
			lastSelectedItem = null;
			return;
		}
		id = GetFullItemId(id);
		UIItemBase uIItemBase = FindItem(id);
		if (selected && autoDeselect && lastSelectedItem != null && uIItemBase != lastSelectedItem)
		{
			lastSelectedItem.SetSelection(selected: false);
		}
		if (uIItemBase != null)
		{
			uIItemBase.SetSelection(selected);
		}
		lastSelectedItem = ((!selected) ? lastSelectedItem : uIItemBase);
	}

	public void RemoveItem(string id, bool changeSelect = false)
	{
		if (!string.IsNullOrEmpty(id) && itemList != null)
		{
			id = GetFullItemId(id);
			UIItemBase uIItemBase = FindItem(id);
			if (uIItemBase == lastSelectedItem)
			{
				lastSelectedItem = null;
			}
			itemList.Remove(uIItemBase);
			uIItemBase.gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(uIItemBase.gameObject);
			_RepositionItem();
		}
	}

	public void RemoveItem(UIItemBase item, bool changeSelect = false)
	{
		if (itemList != null)
		{
			if (item == lastSelectedItem)
			{
				lastSelectedItem = null;
			}
			itemList.Remove(item);
			item.gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(item.gameObject);
			_RepositionItem();
		}
	}

	private void _RepositionItem()
	{
		if (grid != null)
		{
			grid.enabled = true;
			grid.Reposition();
		}
	}

	public string CombineId(string id)
	{
		return itemIdPrefix + id;
	}

	public void ResizeItemList(int count)
	{
		lastSelectedItem = null;
		if (itemList == null)
		{
			itemList = new List<UIItemBase>();
		}
		if (itemList.Count > 0 && !itemList[0].GetType().Equals(itemPrefab.GetType()))
		{
			itemList.ForEach(delegate(UIItemBase item)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			});
			itemList.Clear();
		}
		if (itemList.Count == count)
		{
			UISetter.SetActive(itemPrefab, active: false);
			return;
		}
		List<UIItemBase> list = new List<UIItemBase>(count);
		if (itemList == null)
		{
			itemList = list;
			return;
		}
		if (itemList.Count > count)
		{
			for (int i = 0; i < count; i++)
			{
				list.Add(itemList[i]);
			}
			for (int j = count; j < itemList.Count; j++)
			{
				UnityEngine.Object.Destroy(itemList[j].gameObject);
			}
		}
		else if (itemList.Count < count)
		{
			for (int k = 0; k < itemList.Count; k++)
			{
				list.Add(itemList[k]);
			}
			GameObject itemParent = _itemParent;
			for (int l = itemList.Count; l < count; l++)
			{
				list.Add(_CreateItem(itemParent));
			}
		}
		itemList = list;
		UISetter.SetActive(itemPrefab, active: false);
		_RepositionItem();
	}

	private UIItemBase _CreateItem(GameObject parent)
	{
		GameObject gameObject = NGUITools.AddChild(parent, itemPrefab.gameObject);
		gameObject.SetActive(value: true);
		return gameObject.GetComponent<UIItemBase>();
	}

	public string GetFullItemId(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		if (!string.IsNullOrEmpty(itemIdPrefix) && !id.StartsWith(itemIdPrefix))
		{
			id = CombineId(id);
		}
		return id;
	}

	public string GetPureId(string itemId)
	{
		if (string.IsNullOrEmpty(itemId))
		{
			return null;
		}
		if (!itemId.StartsWith(itemIdPrefix))
		{
			return itemId;
		}
		return itemId.Substring(itemIdPrefix.Length);
	}

	public bool StartWithItemPrefix(string itemId)
	{
		if (string.IsNullOrEmpty(itemId))
		{
			return false;
		}
		return itemId.StartsWith(itemIdPrefix);
	}

	public bool Contains(string id)
	{
		if (string.IsNullOrEmpty(id) || itemList == null || itemList.Count <= 0)
		{
			return false;
		}
		string fullId = GetFullItemId(id);
		return itemList.Find((UIItemBase item) => item.gameObject.name == fullId) != null;
	}

	public bool Contains(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		return itemList.Find((UIItemBase item) => item.gameObject == go) != null;
	}

	public void ResetPosition()
	{
		if (!(scrollView == null) && !(scrollView.panel == null))
		{
			scrollView.DisableSpring();
			Vector3 localPosition = scrollView.transform.localPosition;
			localPosition.x = 0f;
			localPosition.y = 0f;
			scrollView.transform.localPosition = localPosition;
			scrollView.panel.clipOffset = Vector3.zero;
		}
	}
}
