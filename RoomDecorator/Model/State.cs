namespace RoomDecorator.Model
{
	public class State
	{
		protected Character _owner;

		public State(Character owner)
		{
			_owner = owner;
		}

		public virtual void OnEnter()
		{
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void OnExit()
		{
		}
	}
}
