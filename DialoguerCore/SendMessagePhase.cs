using System.Collections.Generic;

namespace DialoguerCore
{
	public class SendMessagePhase : AbstractDialoguePhase
	{
		public readonly string message;

		public readonly string metadata;

		public readonly float duration;

		public readonly float back_r;

		public readonly float back_g;

		public readonly float back_b;

		public readonly float back_a;

		public SendMessagePhase(string message, string metadata, float fadeDuation, float r, float g, float b, float a, List<int> outs)
			: base(outs)
		{
			this.message = message;
			this.metadata = metadata;
			duration = fadeDuation;
			back_r = r;
			back_g = g;
			back_b = b;
			back_a = a;
		}

		protected override void onStart()
		{
			DialoguerEventManager.dispatchOnMessageEvent(message, metadata, duration, back_r, back_g, back_b, back_a);
			base.state = PhaseState.Complete;
		}

		public override string ToString()
		{
			return "Send Message Phase\nMessage: " + message + "\nMetadata: " + metadata + "\n";
		}
	}
}
