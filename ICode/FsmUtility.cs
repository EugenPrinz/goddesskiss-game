using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICode.Actions;
using ICode.Conditions;
using UnityEngine;

namespace ICode
{
	public static class FsmUtility
	{
		public static ICodeBehaviour[] GetBehaviours(this GameObject gameObject)
		{
			return gameObject.GetComponents<ICodeBehaviour>();
		}

		public static ICodeBehaviour[] GetBehaviours(this GameObject gameObject, bool includeChildren)
		{
			if (includeChildren)
			{
				return gameObject.GetComponentsInChildren<ICodeBehaviour>();
			}
			return gameObject.GetBehaviours();
		}

		public static ICodeBehaviour GetBehaviour(this GameObject gameObject)
		{
			return gameObject.GetComponent<ICodeBehaviour>();
		}

		public static ICodeBehaviour GetBehaviour(this GameObject gameObject, int group)
		{
			ICodeBehaviour[] components = gameObject.GetComponents<ICodeBehaviour>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].group == group)
					{
						return components[i];
					}
				}
			}
			return null;
		}

		public static ICodeBehaviour AddBehaviour(this GameObject gameObject, StateMachine stateMachine)
		{
			ICodeBehaviour codeBehaviour = gameObject.AddComponent<ICodeBehaviour>();
			codeBehaviour.stateMachine = stateMachine;
			codeBehaviour.EnableStateMachine();
			return codeBehaviour;
		}

		public static ICodeBehaviour AddBehaviour(this GameObject gameObject, StateMachine stateMachine, int group, bool replaceIfExists)
		{
			ICodeBehaviour codeBehaviour = gameObject.GetBehaviour(group);
			if (codeBehaviour != null && replaceIfExists)
			{
				codeBehaviour.stateMachine = stateMachine;
				codeBehaviour.EnableStateMachine();
			}
			else
			{
				codeBehaviour = gameObject.AddBehaviour(stateMachine);
				codeBehaviour.group = group;
			}
			return codeBehaviour;
		}

		public static Node Copy(Node original)
		{
			Node node = (Node)ScriptableObject.CreateInstance(original.GetType());
			node.color = original.color;
			node.comment = original.comment;
			node.Name = original.Name;
			node.position = original.position;
			node.Parent = original.Parent;
			node.hideFlags = original.hideFlags;
			node.IsStartNode = original.IsStartNode;
			if (original is StateMachine)
			{
				StateMachine stateMachine = node as StateMachine;
				StateMachine stateMachine2 = original as StateMachine;
				stateMachine.Variables = CopyVariables(stateMachine2.Variables);
				stateMachine.Nodes = CopyNodes(stateMachine2.Nodes, stateMachine);
			}
			else if (original is State)
			{
				State state = node as State;
				State state2 = original as State;
				state.IsSequence = state2.IsSequence;
				state.Actions = CopyExecutableNodes<StateAction>(state2.Actions);
			}
			Transition[] transitions = original.Transitions;
			foreach (Transition origTransition in transitions)
			{
				Node node2 = node.Parent.Nodes.ToList().Find((Node x) => x.Name == origTransition.ToNode.Name);
				if (node2 != null)
				{
					Transition transition = ScriptableObject.CreateInstance<Transition>();
					transition.hideFlags = HideFlags.HideInHierarchy;
					transition.ToNode = node2;
					transition.FromNode = node;
					transition.Conditions = CopyExecutableNodes<Condition>(origTransition.Conditions);
					node.Transitions = ArrayUtility.Add(node.Transitions, transition);
				}
			}
			return node;
		}

		private static Node[] CopyNodes(Node[] nodes, StateMachine parent)
		{
			List<Node> list = new List<Node>();
			foreach (Node node in nodes)
			{
				Type type = node.GetType();
				Node node2 = (Node)ScriptableObject.CreateInstance(type);
				node2.color = node.color;
				node2.comment = node.comment;
				node2.Name = node.Name;
				node2.Parent = parent;
				node2.position = node.position;
				node2.IsStartNode = node.IsStartNode;
				node2.hideFlags = HideFlags.HideInHierarchy;
				if (typeof(State).IsAssignableFrom(type))
				{
					State state = node2 as State;
					state.IsSequence = (node as State).IsSequence;
					state.Actions = CopyExecutableNodes<StateAction>((node as State).Actions);
				}
				else
				{
					StateMachine stateMachine = node2 as StateMachine;
					stateMachine.Nodes = CopyNodes((node as StateMachine).Nodes, stateMachine);
				}
				list.Add(node2);
			}
			foreach (Node original in nodes)
			{
				Transition[] transitions = original.Transitions;
				foreach (Transition origTransition in transitions)
				{
					Transition transition = ScriptableObject.CreateInstance<Transition>();
					Node node3 = list.Find((Node x) => x.Name == original.Name);
					transition.hideFlags = HideFlags.HideInHierarchy;
					transition.ToNode = list.ToList().Find((Node x) => x.Name == origTransition.ToNode.Name);
					transition.FromNode = node3;
					transition.Conditions = CopyExecutableNodes<Condition>(origTransition.Conditions);
					node3.Transitions = ArrayUtility.Add(node3.Transitions, transition);
				}
			}
			return list.ToArray();
		}

		public static ExecutableNode Copy(ExecutableNode original)
		{
			ExecutableNode executableNode = (ExecutableNode)ScriptableObject.CreateInstance(original.GetType());
			executableNode.name = original.name;
			executableNode.hideFlags = original.hideFlags;
			FieldInfo[] fields = original.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				object value = fieldInfo.GetValue(original);
				fieldInfo.SetValue(executableNode, CopyFields(value));
			}
			return executableNode;
		}

		public static T[] CopyExecutableNodes<T>(ExecutableNode[] nodes)
		{
			List<T> list = new List<T>();
			foreach (ExecutableNode executableNode in nodes)
			{
				if (executableNode == null)
				{
					continue;
				}
				ExecutableNode executableNode2 = UnityEngine.Object.Instantiate(executableNode);
				executableNode2.hideFlags = HideFlags.HideInHierarchy;
				executableNode2.name = executableNode.name;
				FieldInfo[] fields = executableNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo[] array = fields;
				foreach (FieldInfo fieldInfo in array)
				{
					if (fieldInfo.FieldType.IsSubclassOf(typeof(FsmVariable)) && fieldInfo.GetValue(executableNode) == null)
					{
						FsmVariable value = (FsmVariable)ScriptableObject.CreateInstance(fieldInfo.FieldType);
						fieldInfo.SetValue(executableNode, value);
					}
					object value2 = fieldInfo.GetValue(executableNode);
					fieldInfo.SetValue(executableNode2, CopyFields(value2));
				}
				list.Add((T)(object)executableNode2);
			}
			return list.ToArray();
		}

		private static object CopyFields(object source)
		{
			if (source == null)
			{
				return null;
			}
			Type type = source.GetType();
			if (type.IsValueType || type == typeof(string) || type == typeof(GameObject) || (typeof(UnityEngine.Object).IsAssignableFrom(type) && !typeof(FsmVariable).IsAssignableFrom(type)))
			{
				return source;
			}
			if (type.IsSubclassOf(typeof(ScriptableObject)))
			{
				ScriptableObject scriptableObject = UnityEngine.Object.Instantiate(source as ScriptableObject);
				scriptableObject.hideFlags = HideFlags.HideInHierarchy;
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableObject)))
					{
						object value = fieldInfo.GetValue(source);
						if (value != null)
						{
							fieldInfo.SetValue(scriptableObject, CopyFields(value));
						}
					}
				}
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.CanWrite && propertyInfo.PropertyType.IsSubclassOf(typeof(ScriptableObject)))
					{
						object value2 = propertyInfo.GetValue(source, null);
						if (value2 != null)
						{
							propertyInfo.SetValue(scriptableObject, CopyFields(value2), null);
						}
					}
				}
				return scriptableObject;
			}
			if (type.IsArray)
			{
				Array array = source as Array;
				Type elementType = type.GetElementType();
				Array array2 = Array.CreateInstance(elementType, array.Length);
				for (int k = 0; k < array.Length; k++)
				{
					array2.SetValue(CopyFields(array.GetValue(k)), k);
				}
				return Convert.ChangeType(array2, source.GetType());
			}
			if (typeof(IList).IsAssignableFrom(type) && type.IsGenericType)
			{
				IList list = source as IList;
				Type type2 = type.GetElementType();
				if (type2 == null)
				{
					type2 = type.GetGenericArguments()[0];
				}
				IList list2 = (IList)typeof(List<>).MakeGenericType(type2).GetConstructor(Type.EmptyTypes).Invoke(null);
				for (int l = 0; l < list.Count; l++)
				{
					list2.Add(CopyFields(list[l]));
				}
				return list2;
			}
			return null;
		}

		private static FsmVariable[] CopyVariables(FsmVariable[] variables)
		{
			List<FsmVariable> list = new List<FsmVariable>();
			foreach (FsmVariable original in variables)
			{
				FsmVariable fsmVariable = UnityEngine.Object.Instantiate(original);
				fsmVariable.hideFlags = HideFlags.HideInHierarchy;
				list.Add(fsmVariable);
			}
			return list.ToArray();
		}

		public static Node FindNode(StateMachine root, string name)
		{
			if (root.Name == name)
			{
				return root;
			}
			Node[] nodesRecursive = root.NodesRecursive;
			foreach (Node node in nodesRecursive)
			{
				if (node.Name == name)
				{
					return node;
				}
			}
			return null;
		}

		public static bool NodeExists(StateMachine stateMachine, string name)
		{
			StateMachine root = stateMachine.Root;
			if (FindNode(root, name) == null)
			{
				return false;
			}
			return true;
		}

		public static Type GetVariableType(Type type)
		{
			if (type == null)
			{
				return null;
			}
			if (type == typeof(string))
			{
				return typeof(FsmString);
			}
			if (type == typeof(bool))
			{
				return typeof(FsmBool);
			}
			if (type == typeof(Color))
			{
				return typeof(FsmColor);
			}
			if (type == typeof(float))
			{
				return typeof(FsmFloat);
			}
			if (type == typeof(GameObject))
			{
				return typeof(FsmGameObject);
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				return typeof(FsmObject);
			}
			if (type == typeof(int))
			{
				return typeof(FsmInt);
			}
			if (type == typeof(Vector2))
			{
				return typeof(FsmVector2);
			}
			if (type == typeof(Vector3))
			{
				return typeof(FsmVector3);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return typeof(FsmArray);
			}
			return null;
		}

		public static T GetRandom<T>(this IList list)
		{
			if (list != null && list.Count > 0)
			{
				return (T)list[UnityEngine.Random.Range(0, list.Count)];
			}
			return default(T);
		}

		public static Vector3 GetPosition(FsmGameObject gameObject, FsmVector3 fsmVector3)
		{
			if (gameObject.Value == null)
			{
				return fsmVector3.Value;
			}
			return fsmVector3.IsNone ? gameObject.Value.transform.position : gameObject.Value.transform.TransformPoint(fsmVector3.Value);
		}

		public static T[] FindAll<T>(bool includeInactive) where T : Component
		{
			if (includeInactive)
			{
				UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(T));
				List<T> list = new List<T>();
				UnityEngine.Object[] array2 = array;
				foreach (UnityEngine.Object @object in array2)
				{
					if (@object is T && @object.hideFlags == HideFlags.None)
					{
						list.Add((T)@object);
					}
				}
				return list.ToArray();
			}
			return (T[])UnityEngine.Object.FindObjectsOfType(typeof(T));
		}

		public static GameObject FindChild(this GameObject target, string name, bool includeInactive)
		{
			if (target != null)
			{
				if ((target.name == name && includeInactive) || (target.name == name && !includeInactive && target.activeInHierarchy))
				{
					return target;
				}
				for (int i = 0; i < target.transform.childCount; i++)
				{
					GameObject gameObject = target.transform.GetChild(i).gameObject.FindChild(name, includeInactive);
					if (gameObject != null)
					{
						return gameObject;
					}
				}
			}
			return null;
		}

		public static GameObject FindClosestByName(this GameObject target, string name)
		{
			Transform[] source = UnityEngine.Object.FindObjectsOfType<Transform>();
			List<GameObject> list = (from x in source
				select x.gameObject into y
				where y.name == name
				select y).ToList();
			GameObject result = null;
			float num = float.PositiveInfinity;
			Vector3 position = target.transform.position;
			foreach (GameObject item in list)
			{
				float sqrMagnitude = (item.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num && item.transform != target.transform)
				{
					result = item;
					num = sqrMagnitude;
				}
			}
			return result;
		}

		public static GameObject FindClosestByTag(this GameObject target, string tag)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
			GameObject result = null;
			float num = float.PositiveInfinity;
			Vector3 position = target.transform.position;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num && gameObject.transform != target.transform)
				{
					result = gameObject;
					num = sqrMagnitude;
				}
			}
			return result;
		}

		public static bool CompareFloat(float first, float second, FloatComparer comparer)
		{
			return comparer switch
			{
				FloatComparer.Less => first < second, 
				FloatComparer.Greater => first > second, 
				FloatComparer.Equal => Mathf.Approximately(first, second), 
				FloatComparer.GreaterOrEqual => first >= second, 
				FloatComparer.LessOrEqual => first <= second, 
				FloatComparer.NotEqual => !Mathf.Approximately(first, second), 
				_ => false, 
			};
		}
	}
}
