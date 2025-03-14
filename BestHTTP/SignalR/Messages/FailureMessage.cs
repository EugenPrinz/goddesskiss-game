using System.Collections.Generic;

namespace BestHTTP.SignalR.Messages
{
	public sealed class FailureMessage : IServerMessage, IHubMessage
	{
		MessageTypes IServerMessage.Type => MessageTypes.Failure;

		public ulong InvocationId { get; private set; }

		public bool IsHubError { get; private set; }

		public string ErrorMessage { get; private set; }

		public IDictionary<string, object> AdditionalData { get; private set; }

		public string StackTrace { get; private set; }

		public IDictionary<string, object> State { get; private set; }

		void IServerMessage.Parse(object data)
		{
			IDictionary<string, object> dictionary = data as IDictionary<string, object>;
			InvocationId = ulong.Parse(dictionary["I"].ToString());
			if (dictionary.TryGetValue("E", out var value))
			{
				ErrorMessage = value.ToString();
			}
			if (dictionary.TryGetValue("H", out value))
			{
				IsHubError = int.Parse(value.ToString()) == 1;
			}
			if (dictionary.TryGetValue("D", out value))
			{
				AdditionalData = value as IDictionary<string, object>;
			}
			if (dictionary.TryGetValue("T", out value))
			{
				StackTrace = value.ToString();
			}
			if (dictionary.TryGetValue("S", out value))
			{
				State = value as IDictionary<string, object>;
			}
		}
	}
}
