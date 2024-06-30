namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ElGamalKeyParameters : AsymmetricKeyParameter
	{
		private readonly ElGamalParameters parameters;

		public ElGamalParameters Parameters => parameters;

		protected ElGamalKeyParameters(bool isPrivate, ElGamalParameters parameters)
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
			if (!(obj is ElGamalKeyParameters other))
			{
				return false;
			}
			return Equals(other);
		}

		protected bool Equals(ElGamalKeyParameters other)
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
