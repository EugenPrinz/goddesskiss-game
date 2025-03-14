namespace Org.BouncyCastle.Asn1.Nist
{
	public sealed class NistObjectIdentifiers
	{
		public static readonly DerObjectIdentifier NistAlgorithm = new DerObjectIdentifier("2.16.840.1.101.3.4");

		public static readonly DerObjectIdentifier HashAlgs = NistAlgorithm.Branch("2");

		public static readonly DerObjectIdentifier IdSha256 = HashAlgs.Branch("1");

		public static readonly DerObjectIdentifier IdSha384 = HashAlgs.Branch("2");

		public static readonly DerObjectIdentifier IdSha512 = HashAlgs.Branch("3");

		public static readonly DerObjectIdentifier IdSha224 = HashAlgs.Branch("4");

		public static readonly DerObjectIdentifier IdSha512_224 = HashAlgs.Branch("5");

		public static readonly DerObjectIdentifier IdSha512_256 = HashAlgs.Branch("6");

		public static readonly DerObjectIdentifier Aes = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".1"));

		public static readonly DerObjectIdentifier IdAes128Ecb = new DerObjectIdentifier(string.Concat(Aes, ".1"));

		public static readonly DerObjectIdentifier IdAes128Cbc = new DerObjectIdentifier(string.Concat(Aes, ".2"));

		public static readonly DerObjectIdentifier IdAes128Ofb = new DerObjectIdentifier(string.Concat(Aes, ".3"));

		public static readonly DerObjectIdentifier IdAes128Cfb = new DerObjectIdentifier(string.Concat(Aes, ".4"));

		public static readonly DerObjectIdentifier IdAes128Wrap = new DerObjectIdentifier(string.Concat(Aes, ".5"));

		public static readonly DerObjectIdentifier IdAes128Gcm = new DerObjectIdentifier(string.Concat(Aes, ".6"));

		public static readonly DerObjectIdentifier IdAes128Ccm = new DerObjectIdentifier(string.Concat(Aes, ".7"));

		public static readonly DerObjectIdentifier IdAes192Ecb = new DerObjectIdentifier(string.Concat(Aes, ".21"));

		public static readonly DerObjectIdentifier IdAes192Cbc = new DerObjectIdentifier(string.Concat(Aes, ".22"));

		public static readonly DerObjectIdentifier IdAes192Ofb = new DerObjectIdentifier(string.Concat(Aes, ".23"));

		public static readonly DerObjectIdentifier IdAes192Cfb = new DerObjectIdentifier(string.Concat(Aes, ".24"));

		public static readonly DerObjectIdentifier IdAes192Wrap = new DerObjectIdentifier(string.Concat(Aes, ".25"));

		public static readonly DerObjectIdentifier IdAes192Gcm = new DerObjectIdentifier(string.Concat(Aes, ".26"));

		public static readonly DerObjectIdentifier IdAes192Ccm = new DerObjectIdentifier(string.Concat(Aes, ".27"));

		public static readonly DerObjectIdentifier IdAes256Ecb = new DerObjectIdentifier(string.Concat(Aes, ".41"));

		public static readonly DerObjectIdentifier IdAes256Cbc = new DerObjectIdentifier(string.Concat(Aes, ".42"));

		public static readonly DerObjectIdentifier IdAes256Ofb = new DerObjectIdentifier(string.Concat(Aes, ".43"));

		public static readonly DerObjectIdentifier IdAes256Cfb = new DerObjectIdentifier(string.Concat(Aes, ".44"));

		public static readonly DerObjectIdentifier IdAes256Wrap = new DerObjectIdentifier(string.Concat(Aes, ".45"));

		public static readonly DerObjectIdentifier IdAes256Gcm = new DerObjectIdentifier(string.Concat(Aes, ".46"));

		public static readonly DerObjectIdentifier IdAes256Ccm = new DerObjectIdentifier(string.Concat(Aes, ".47"));

		public static readonly DerObjectIdentifier IdDsaWithSha2 = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".3"));

		public static readonly DerObjectIdentifier DsaWithSha224 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".1"));

		public static readonly DerObjectIdentifier DsaWithSha256 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".2"));

		public static readonly DerObjectIdentifier DsaWithSha384 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".3"));

		public static readonly DerObjectIdentifier DsaWithSha512 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".4"));

		private NistObjectIdentifiers()
		{
		}
	}
}
