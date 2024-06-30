using UnityEngine;

namespace Step
{
	public class OnClickEvent : AbstractStepEvent
	{
		private struct DepthEntry
		{
			public int depth;

			public RaycastHit hit;

			public Vector3 point;

			public GameObject go;
		}

		private static Plane m2DPlane = new Plane(Vector3.back, 0f);

		private static DepthEntry mHit = default(DepthEntry);

		private static BetterList<DepthEntry> mHits = new BetterList<DepthEntry>();

		public void OnClick()
		{
			UIButton.current = null;
			_isFinish = true;
		}

		private static bool IsVisible(ref DepthEntry de)
		{
			UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(de.go);
			while (uIPanel != null)
			{
				if (!uIPanel.IsVisible(de.point))
				{
					return false;
				}
				uIPanel = uIPanel.parentPanel;
			}
			return true;
		}

		private static bool IsVisible(Vector3 worldPoint, GameObject go)
		{
			UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(go);
			while (uIPanel != null)
			{
				if (!uIPanel.IsVisible(worldPoint))
				{
					return false;
				}
				uIPanel = uIPanel.parentPanel;
			}
			return true;
		}

		private static Rigidbody FindRootRigidbody(Transform trans)
		{
			while (trans != null)
			{
				if (trans.GetComponent<UIPanel>() != null)
				{
					return null;
				}
				Rigidbody component = trans.GetComponent<Rigidbody>();
				if (component != null)
				{
					return component;
				}
				trans = trans.parent;
			}
			return null;
		}

		private static Rigidbody2D FindRootRigidbody2D(Transform trans)
		{
			while (trans != null)
			{
				if (trans.GetComponent<UIPanel>() != null)
				{
					return null;
				}
				Rigidbody2D component = trans.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					return component;
				}
				trans = trans.parent;
			}
			return null;
		}

		public static bool Raycast(Vector3 inPos)
		{
			string[] array = new string[2] { "_InputLcok_", "_Waiting_" };
			for (int i = 0; i < UICamera.list.size; i++)
			{
				UICamera uICamera = UICamera.list.buffer[i];
				if (!uICamera.enabled || !NGUITools.GetActive(uICamera.gameObject))
				{
					continue;
				}
				UICamera.currentCamera = uICamera.cachedCamera;
				Vector3 vector = UICamera.currentCamera.ScreenToViewportPoint(inPos);
				if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
				{
					continue;
				}
				Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
				int layerMask = UICamera.currentCamera.cullingMask & (int)uICamera.eventReceiverMask;
				float enter = ((!(uICamera.rangeDistance > 0f)) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uICamera.rangeDistance);
				if (uICamera.eventType == UICamera.EventType.World_3D)
				{
					if (!Physics.Raycast(ray, out UICamera.lastHit, enter, layerMask))
					{
						continue;
					}
					UICamera.lastWorldPosition = UICamera.lastHit.point;
					UICamera.hoveredObject = UICamera.lastHit.collider.gameObject;
					if (!UICamera.list[0].eventsGoToColliders)
					{
						Rigidbody rigidbody = FindRootRigidbody(UICamera.hoveredObject.transform);
						if (rigidbody != null)
						{
							UICamera.hoveredObject = rigidbody.gameObject;
						}
					}
					return true;
				}
				if (uICamera.eventType == UICamera.EventType.UI_3D)
				{
					RaycastHit[] array2 = Physics.RaycastAll(ray, enter, layerMask);
					if (array2.Length > 1)
					{
						for (int j = 0; j < array2.Length; j++)
						{
							GameObject gameObject = array2[j].collider.gameObject;
							UIWidget component = gameObject.GetComponent<UIWidget>();
							if (component != null)
							{
								if (!component.isVisible || (component.hitCheck != null && !component.hitCheck(array2[j].point)))
								{
									continue;
								}
							}
							else
							{
								UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
								if (uIRect != null && uIRect.finalAlpha < 0.001f)
								{
									continue;
								}
							}
							mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
							if (mHit.depth != int.MaxValue)
							{
								mHit.hit = array2[j];
								mHit.point = array2[j].point;
								mHit.go = array2[j].collider.gameObject;
								mHits.Add(mHit);
							}
						}
						mHits.Sort((DepthEntry r1, DepthEntry r2) => r2.depth.CompareTo(r1.depth));
						for (int k = 0; k < mHits.size; k++)
						{
							if (!IsVisible(ref mHits.buffer[k]))
							{
								continue;
							}
							bool flag = false;
							for (int l = 0; l < array.Length; l++)
							{
								if (mHits[k].go.name.StartsWith(array[l]))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								UICamera.lastHit = mHits[k].hit;
								UICamera.hoveredObject = mHits[k].go;
								UICamera.lastWorldPosition = mHits[k].point;
								mHits.Clear();
								return true;
							}
						}
						mHits.Clear();
					}
					else
					{
						if (array2.Length != 1)
						{
							continue;
						}
						GameObject gameObject2 = array2[0].collider.gameObject;
						UIWidget component2 = gameObject2.GetComponent<UIWidget>();
						if (component2 != null)
						{
							if (!component2.isVisible || (component2.hitCheck != null && !component2.hitCheck(array2[0].point)))
							{
								continue;
							}
						}
						else
						{
							UIRect uIRect2 = NGUITools.FindInParents<UIRect>(gameObject2);
							if (uIRect2 != null && uIRect2.finalAlpha < 0.001f)
							{
								continue;
							}
						}
						if (IsVisible(array2[0].point, array2[0].collider.gameObject))
						{
							UICamera.lastHit = array2[0];
							UICamera.lastWorldPosition = array2[0].point;
							UICamera.hoveredObject = UICamera.lastHit.collider.gameObject;
							return true;
						}
					}
				}
				else
				{
					if (uICamera.eventType == UICamera.EventType.World_2D)
					{
						if (!m2DPlane.Raycast(ray, out enter))
						{
							continue;
						}
						Vector3 point = ray.GetPoint(enter);
						Collider2D collider2D = Physics2D.OverlapPoint(point, layerMask);
						if (!collider2D)
						{
							continue;
						}
						UICamera.lastWorldPosition = point;
						UICamera.hoveredObject = collider2D.gameObject;
						if (!uICamera.eventsGoToColliders)
						{
							Rigidbody2D rigidbody2D = FindRootRigidbody2D(UICamera.hoveredObject.transform);
							if (rigidbody2D != null)
							{
								UICamera.hoveredObject = rigidbody2D.gameObject;
							}
						}
						return true;
					}
					if (uICamera.eventType != UICamera.EventType.UI_2D || !m2DPlane.Raycast(ray, out enter))
					{
						continue;
					}
					UICamera.lastWorldPosition = ray.GetPoint(enter);
					Collider2D[] array3 = Physics2D.OverlapPointAll(UICamera.lastWorldPosition, layerMask);
					if (array3.Length > 1)
					{
						for (int m = 0; m < array3.Length; m++)
						{
							GameObject gameObject3 = array3[m].gameObject;
							UIWidget component3 = gameObject3.GetComponent<UIWidget>();
							if (component3 != null)
							{
								if (!component3.isVisible || (component3.hitCheck != null && !component3.hitCheck(UICamera.lastWorldPosition)))
								{
									continue;
								}
							}
							else
							{
								UIRect uIRect3 = NGUITools.FindInParents<UIRect>(gameObject3);
								if (uIRect3 != null && uIRect3.finalAlpha < 0.001f)
								{
									continue;
								}
							}
							mHit.depth = NGUITools.CalculateRaycastDepth(gameObject3);
							if (mHit.depth != int.MaxValue)
							{
								mHit.go = gameObject3;
								mHit.point = UICamera.lastWorldPosition;
								mHits.Add(mHit);
							}
						}
						mHits.Sort((DepthEntry r1, DepthEntry r2) => r2.depth.CompareTo(r1.depth));
						for (int n = 0; n < mHits.size; n++)
						{
							if (IsVisible(ref mHits.buffer[n]))
							{
								UICamera.hoveredObject = mHits[n].go;
								mHits.Clear();
								return true;
							}
						}
						mHits.Clear();
					}
					else
					{
						if (array3.Length != 1)
						{
							continue;
						}
						GameObject gameObject4 = array3[0].gameObject;
						UIWidget component4 = gameObject4.GetComponent<UIWidget>();
						if (component4 != null)
						{
							if (!component4.isVisible || (component4.hitCheck != null && !component4.hitCheck(UICamera.lastWorldPosition)))
							{
								continue;
							}
						}
						else
						{
							UIRect uIRect4 = NGUITools.FindInParents<UIRect>(gameObject4);
							if (uIRect4 != null && uIRect4.finalAlpha < 0.001f)
							{
								continue;
							}
						}
						if (IsVisible(UICamera.lastWorldPosition, gameObject4))
						{
							UICamera.hoveredObject = gameObject4;
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
