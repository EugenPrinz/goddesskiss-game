using System;
using System.Collections.Generic;

namespace com.adjust.sdk
{
	public class AdjustEventFailure
	{
		public string Adid { get; set; }

		public string Message { get; set; }

		public string Timestamp { get; set; }

		public string EventToken { get; set; }

		public bool WillRetry { get; set; }

		public Dictionary<string, object> JsonResponse { get; set; }

		public AdjustEventFailure()
		{
		}

		public AdjustEventFailure(Dictionary<string, string> eventFailureDataMap)
		{
			if (eventFailureDataMap != null)
			{
				Adid = AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyAdid);
				Message = AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyMessage);
				Timestamp = AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyTimestamp);
				EventToken = AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyEventToken);
				WillRetry = bool.Parse(AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyWillRetry));
				if (bool.TryParse(AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyWillRetry), out var result))
				{
					WillRetry = result;
				}
				string aJSON = AdjustUtils.TryGetValue(eventFailureDataMap, AdjustUtils.KeyJsonResponse);
				JSONNode jSONNode = JSON.Parse(aJSON);
				if (jSONNode != null && jSONNode.AsObject != null)
				{
					JsonResponse = new Dictionary<string, object>();
					AdjustUtils.WriteJsonResponseDictionary(jSONNode.AsObject, JsonResponse);
				}
			}
		}

		public AdjustEventFailure(string jsonString)
		{
			JSONNode jSONNode = JSON.Parse(jsonString);
			if (!(jSONNode == null))
			{
				Adid = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyAdid);
				Message = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyMessage);
				Timestamp = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyTimestamp);
				EventToken = AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyEventToken);
				WillRetry = Convert.ToBoolean(AdjustUtils.GetJsonString(jSONNode, AdjustUtils.KeyWillRetry));
				JSONNode jSONNode2 = jSONNode[AdjustUtils.KeyJsonResponse];
				if (!(jSONNode2 == null) && !(jSONNode2.AsObject == null))
				{
					JsonResponse = new Dictionary<string, object>();
					AdjustUtils.WriteJsonResponseDictionary(jSONNode2.AsObject, JsonResponse);
				}
			}
		}

		public void BuildJsonResponseFromString(string jsonResponseString)
		{
			JSONNode jSONNode = JSON.Parse(jsonResponseString);
			if (!(jSONNode == null))
			{
				JsonResponse = new Dictionary<string, object>();
				AdjustUtils.WriteJsonResponseDictionary(jSONNode.AsObject, JsonResponse);
			}
		}

		public string GetJsonResponse()
		{
			return AdjustUtils.GetJsonResponseCompact(JsonResponse);
		}
	}
}
