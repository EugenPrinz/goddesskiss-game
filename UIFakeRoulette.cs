using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFakeRoulette : UIPanelBase
{
	public UIPanel clipPanel;

	public UIGrid grid;

	public AnimationCurve animationCurve;

	public float playDuration = 3f;

	public int loop = 10;

	public int minLoop = 8;

	public int maxLoop = 12;

	private List<GameObject> itemList;

	public GameObject roulletItemPrefab;

	private Vector3 position;

	public void SetList(List<int> list)
	{
		if (itemList != null)
		{
			int num = itemList.Count - 1;
			while (0 <= num)
			{
				Object.DestroyImmediate(itemList[num]);
				itemList[num] = null;
				num--;
			}
			itemList.Clear();
			grid.transform.localPosition = position;
		}
		else
		{
			itemList = new List<GameObject>();
			position = grid.transform.localPosition;
		}
		int num2 = 0;
		for (int i = 0; i <= list[list.Count - 1]; i++)
		{
			GameObject gameObject = null;
			if (i == list[num2])
			{
				gameObject = Object.Instantiate(roulletItemPrefab);
				RoulletListItem component = gameObject.GetComponent<RoulletListItem>();
				component.Set(list[num2]);
				num2++;
			}
			itemList.Add(gameObject);
		}
		grid.AddChild(itemList[9].transform);
		grid.AddChild(itemList[10].transform);
		grid.AddChild(itemList[1].transform);
		grid.AddChild(itemList[2].transform);
		grid.AddChild(itemList[3].transform);
		grid.AddChild(itemList[4].transform);
		grid.AddChild(itemList[5].transform);
		grid.AddChild(itemList[6].transform);
		grid.AddChild(itemList[7].transform);
		grid.AddChild(itemList[8].transform);
		for (int j = 0; j < itemList.Count; j++)
		{
			if (itemList[j] != null)
			{
				itemList[j].transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		grid.repositionNow = true;
	}

	public IEnumerator Play(int targetIndex, int startIndex = -1)
	{
		if (itemList.Count <= targetIndex)
		{
			yield break;
		}
		List<GameObject> trimList = new List<GameObject>();
		int finalTargetIndex = targetIndex;
		int targetIndexOffset = 0;
		int startIndexOffset = 0;
		for (int i = 0; i < itemList.Count; i++)
		{
			GameObject gameObject = itemList[i];
			if (gameObject == null)
			{
				if (i != finalTargetIndex && i < finalTargetIndex)
				{
					targetIndexOffset++;
				}
				if (i < startIndex)
				{
					startIndexOffset++;
				}
			}
			else
			{
				trimList.Add(gameObject);
			}
		}
		finalTargetIndex -= targetIndexOffset;
		startIndex -= startIndexOffset;
		if (loop <= 0)
		{
			loop = Random.Range(minLoop, maxLoop);
		}
		Transform t = grid.transform;
		Vector3 gridPos = t.localPosition;
		gridPos.x = 0f;
		t.localPosition = gridPos;
		grid.enabled = false;
		int siblingIdx = 0;
		trimList.ForEach(delegate(GameObject go)
		{
			go.transform.SetSiblingIndex(siblingIdx++);
		});
		float boardW = clipPanel.width;
		float cellW = grid.cellWidth;
		float totalItemW = (float)trimList.Count * cellW;
		startIndex = ((startIndex >= 0) ? startIndex : Random.Range(0, trimList.Count));
		LinkedList<GameObject> ringList = new LinkedList<GameObject>(trimList);
		LinkedListNode<GameObject> node6 = null;
		int move = ringList.Count / 2 - startIndex;
		int firstIdx = startIndex + move;
		float offset2 = (float)(((move < 0) ? 1 : (-1)) * move) * cellW;
		while (move != 0)
		{
			if (move < 0)
			{
				node6 = ringList.First;
				ringList.RemoveFirst();
				ringList.AddLast(node6);
				move++;
			}
			else if (move > 0)
			{
				node6 = ringList.Last;
				ringList.RemoveLast();
				ringList.AddFirst(node6);
				move--;
			}
		}
		node6 = ringList.First;
		siblingIdx = 0;
		while (node6 != null)
		{
			Transform transform = node6.Value.transform;
			transform.localPosition = new Vector3((float)siblingIdx * cellW, 0f, 0f);
			transform.SetSiblingIndex(siblingIdx++);
			node6 = node6.Next;
		}
		offset2 = (gridPos.x = (float)(-firstIdx) * cellW);
		t.localPosition = gridPos;
		int idxDist = trimList.Count - startIndex + finalTargetIndex;
		float targetDist2 = (float)idxDist * cellW;
		targetDist2 += (float)loop * totalItemW;
		float playTime = 0f;
		float preVal = 0f;
		while (true)
		{
			float dt = Time.deltaTime;
			playTime += dt;
			if (playDuration <= playTime)
			{
				break;
			}
			float currEval = animationCurve.Evaluate(playTime / playDuration);
			float dir = currEval - preVal;
			preVal = currEval;
			float moveDist = targetDist2 * currEval;
			gridPos.x = 0f - moveDist + offset2;
			t.localPosition = gridPos;
			float clipPanelX = clipPanel.transform.localPosition.x;
			while (dir != 0f)
			{
				if (dir > 0f)
				{
					GameObject value = ringList.First.Value;
					Transform transform2 = value.transform;
					float x = transform2.localPosition.x;
					float num = x + cellW * 0.5f + t.localPosition.x;
					if (clipPanelX - boardW <= num)
					{
						break;
					}
					num = ringList.Last.Value.transform.localPosition.x + cellW;
					node6 = ringList.First;
					ringList.RemoveFirst();
					ringList.AddLast(node6);
					transform2.localPosition = new Vector3(num, 0f, 0f);
				}
				else if (dir < 0f)
				{
					GameObject value2 = ringList.Last.Value;
					Transform transform3 = value2.transform;
					float x2 = transform3.localPosition.x;
					float num2 = x2 - cellW * 0.5f + t.localPosition.x;
					if (clipPanelX + boardW >= num2)
					{
						break;
					}
					num2 = ringList.First.Value.transform.localPosition.x - cellW;
					node6 = ringList.Last;
					ringList.RemoveLast();
					ringList.AddFirst(node6);
					transform3.localPosition = new Vector3(num2, 0f, 0f);
				}
			}
			yield return null;
		}
		gridPos.x = 0f - targetDist2 + offset2;
		t.localPosition = gridPos;
	}
}
