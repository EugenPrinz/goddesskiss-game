using System;
using System.Collections.Generic;
using DialoguerEditor;

namespace DialoguerCore
{
	public class DialoguerUtils
	{
		private static Dictionary<VariableEditorScopes, string> scopeStrings = new Dictionary<VariableEditorScopes, string>
		{
			{
				VariableEditorScopes.Global,
				"g"
			},
			{
				VariableEditorScopes.Local,
				"l"
			}
		};

		private static Dictionary<VariableEditorTypes, string> typeStrings = new Dictionary<VariableEditorTypes, string>
		{
			{
				VariableEditorTypes.Boolean,
				"b"
			},
			{
				VariableEditorTypes.Float,
				"f"
			},
			{
				VariableEditorTypes.String,
				"s"
			},
			{
				VariableEditorTypes.Argument,
				"a"
			}
		};

		public static string sEmotion;

		public static string sArgument;

		public static string insertTextPhaseStringVariables(string input)
		{
			int dialogueId = 0;
			string input2 = input;
			string empty = string.Empty;
			sEmotion = string.Empty;
			input2 = substituteStringVariable(input2, VariableEditorScopes.Local, VariableEditorTypes.String, dialogueId);
			return substituteStringVariable(input2, VariableEditorScopes.Local, VariableEditorTypes.Argument, dialogueId);
		}

		private static string substituteStringVariable(string input, VariableEditorScopes scope, VariableEditorTypes type, int dialogueId)
		{
			string result = string.Empty;
			string[] separator = new string[1] { "<" + scopeStrings[scope] + typeStrings[type] + ">" };
			string[] separator2 = new string[1] { "</" + scopeStrings[scope] + typeStrings[type] + ">" };
			string[] array = input.Split(separator, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator2, StringSplitOptions.None);
				if (int.TryParse(array2[0], out var result2))
				{
					switch (scope)
					{
					case VariableEditorScopes.Global:
						switch (type)
						{
						case VariableEditorTypes.Boolean:
							array2[0] = Dialoguer.GetGlobalBoolean(result2).ToString();
							break;
						case VariableEditorTypes.Float:
							array2[0] = Dialoguer.GetGlobalFloat(result2).ToString();
							break;
						case VariableEditorTypes.String:
							array2[0] = Dialoguer.GetGlobalString(result2);
							break;
						}
						break;
					case VariableEditorScopes.Local:
						switch (type)
						{
						}
						break;
					}
				}
				if (array2.Length == 2)
				{
					switch (type)
					{
					case VariableEditorTypes.String:
						sEmotion = array2[0];
						break;
					case VariableEditorTypes.Argument:
						sArgument = array2[0];
						break;
					}
					result = array2[1];
				}
				else if (array2.Length == 1)
				{
					result = string.Join(string.Empty, array2);
				}
			}
			return result;
		}
	}
}
