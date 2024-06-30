namespace Org.BouncyCastle.Crypto.Parameters
{
	public abstract class DsaKeyParameters : AsymmetricKeyParameter
	{
		private readonly DsaParameters parameters;

		public DsaParameters Parameters => parameters;

		protected DsaKeyParameters(bool isPrivate, DsaParameters parameters)
			: base(isPrivate)
		{
			this.parameters = parameters;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is DsaKeyParameters other))
			{
				return false;
			}
			return Equals(other);
		}

		protected bool Equals(DsaKeyParameters other)
		{
			return object.Equals(parameters, other.parameters) && Equals((AsymmetricKeyParameter)other);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			if (parameters != null)
			{
				num ^= parameters.GetHashCode();
			}
			return num;
		}
	}
}
