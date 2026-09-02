using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.EdEC;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Security;

public sealed class PrivateKeyFactory
{
	private PrivateKeyFactory()
	{
	}

	public static AsymmetricKeyParameter CreateKey(byte[] privateKeyInfoData)
	{
		return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(privateKeyInfoData)));
	}

	public static AsymmetricKeyParameter CreateKey(Stream inStr)
	{
		return CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromStream(inStr)));
	}

	public static AsymmetricKeyParameter CreateKey(PrivateKeyInfo keyInfo)
	{
		AlgorithmIdentifier privateKeyAlgorithm = keyInfo.PrivateKeyAlgorithm;
		DerObjectIdentifier algorithm = privateKeyAlgorithm.Algorithm;
		if (algorithm.Equals(PkcsObjectIdentifiers.RsaEncryption) || algorithm.Equals(X509ObjectIdentifiers.IdEARsa) || algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss) || algorithm.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
		{
			RsaPrivateKeyStructure instance = RsaPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey());
			return new RsaPrivateCrtKeyParameters(instance.Modulus, instance.PublicExponent, instance.PrivateExponent, instance.Prime1, instance.Prime2, instance.Exponent1, instance.Exponent2, instance.Coefficient);
		}
		if (algorithm.Equals(PkcsObjectIdentifiers.DhKeyAgreement))
		{
			DHParameter dHParameter = new DHParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
			DerInteger obj = (DerInteger)keyInfo.ParsePrivateKey();
			return new DHPrivateKeyParameters(parameters: new DHParameters(l: dHParameter.L?.IntValue ?? 0, p: dHParameter.P, g: dHParameter.G, q: null), x: obj.Value, algorithmOid: algorithm);
		}
		if (algorithm.Equals(OiwObjectIdentifiers.ElGamalAlgorithm))
		{
			ElGamalParameter elGamalParameter = new ElGamalParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
			return new ElGamalPrivateKeyParameters(((DerInteger)keyInfo.ParsePrivateKey()).Value, new ElGamalParameters(elGamalParameter.P, elGamalParameter.G));
		}
		if (algorithm.Equals(X9ObjectIdentifiers.IdDsa))
		{
			DerInteger obj2 = (DerInteger)keyInfo.ParsePrivateKey();
			Asn1Encodable parameters = privateKeyAlgorithm.Parameters;
			DsaParameters parameters2 = null;
			if (parameters != null)
			{
				DsaParameter instance2 = DsaParameter.GetInstance(parameters.ToAsn1Object());
				parameters2 = new DsaParameters(instance2.P, instance2.Q, instance2.G);
			}
			return new DsaPrivateKeyParameters(obj2.Value, parameters2);
		}
		if (algorithm.Equals(X9ObjectIdentifiers.IdECPublicKey))
		{
			X962Parameters instance3 = X962Parameters.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object());
			X9ECParameters x9ECParameters = ((!instance3.IsNamedCurve) ? new X9ECParameters((Asn1Sequence)instance3.Parameters) : ECKeyPairGenerator.FindECCurveByOid((DerObjectIdentifier)instance3.Parameters));
			BigInteger key = ECPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey()).GetKey();
			if (instance3.IsNamedCurve)
			{
				return new ECPrivateKeyParameters("EC", key, (DerObjectIdentifier)instance3.Parameters);
			}
			ECDomainParameters parameters3 = new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
			return new ECPrivateKeyParameters(key, parameters3);
		}
		if (algorithm.Equals(CryptoProObjectIdentifiers.GostR3410x2001))
		{
			Gost3410PublicKeyAlgParameters instance4 = Gost3410PublicKeyAlgParameters.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object());
			X9ECParameters byOidX = ECGost3410NamedCurves.GetByOidX9(instance4.PublicKeyParamSet);
			if (byOidX == null)
			{
				throw new ArgumentException("Unrecognized curve OID for GostR3410x2001 private key");
			}
			Asn1Object asn1Object = keyInfo.ParsePrivateKey();
			ECPrivateKeyStructure eCPrivateKeyStructure = ((!(asn1Object is DerInteger)) ? ECPrivateKeyStructure.GetInstance(asn1Object) : new ECPrivateKeyStructure(byOidX.N.BitLength, ((DerInteger)asn1Object).PositiveValue));
			return new ECPrivateKeyParameters("ECGOST3410", eCPrivateKeyStructure.GetKey(), instance4.PublicKeyParamSet);
		}
		if (algorithm.Equals(CryptoProObjectIdentifiers.GostR3410x94))
		{
			Gost3410PublicKeyAlgParameters instance5 = Gost3410PublicKeyAlgParameters.GetInstance(privateKeyAlgorithm.Parameters);
			Asn1Object asn1Object2 = keyInfo.ParsePrivateKey();
			BigInteger x = ((!(asn1Object2 is DerInteger)) ? new BigInteger(1, Arrays.Reverse(Asn1OctetString.GetInstance(asn1Object2).GetOctets())) : DerInteger.GetInstance(asn1Object2).PositiveValue);
			return new Gost3410PrivateKeyParameters(x, instance5.PublicKeyParamSet);
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_X25519))
		{
			return new X25519PrivateKeyParameters(GetRawKey(keyInfo, X25519PrivateKeyParameters.KeySize), 0);
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_X448))
		{
			return new X448PrivateKeyParameters(GetRawKey(keyInfo, X448PrivateKeyParameters.KeySize), 0);
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_Ed25519))
		{
			return new Ed25519PrivateKeyParameters(GetRawKey(keyInfo, Ed25519PrivateKeyParameters.KeySize), 0);
		}
		if (algorithm.Equals(EdECObjectIdentifiers.id_Ed448))
		{
			return new Ed448PrivateKeyParameters(GetRawKey(keyInfo, Ed448PrivateKeyParameters.KeySize), 0);
		}
		if (algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512) || algorithm.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256))
		{
			Gost3410PublicKeyAlgParameters instance6 = Gost3410PublicKeyAlgParameters.GetInstance(keyInfo.PrivateKeyAlgorithm.Parameters);
			ECGost3410Parameters eCGost3410Parameters = null;
			BigInteger bigInteger = null;
			Asn1Object asn1Object3 = keyInfo.PrivateKeyAlgorithm.Parameters.ToAsn1Object();
			if (asn1Object3 is Asn1Sequence && (Asn1Sequence.GetInstance(asn1Object3).Count == 2 || Asn1Sequence.GetInstance(asn1Object3).Count == 3))
			{
				X9ECParameters byOidX2 = ECGost3410NamedCurves.GetByOidX9(instance6.PublicKeyParamSet);
				eCGost3410Parameters = new ECGost3410Parameters(new ECNamedDomainParameters(instance6.PublicKeyParamSet, byOidX2), instance6.PublicKeyParamSet, instance6.DigestParamSet, instance6.EncryptionParamSet);
				Asn1OctetString privateKeyData = keyInfo.PrivateKeyData;
				if (privateKeyData.GetOctets().Length == 32 || privateKeyData.GetOctets().Length == 64)
				{
					byte[] bytes = Arrays.Reverse(privateKeyData.GetOctets());
					bigInteger = new BigInteger(1, bytes);
				}
				else
				{
					Asn1Encodable asn1Encodable = keyInfo.ParsePrivateKey();
					if (asn1Encodable is DerInteger)
					{
						bigInteger = DerInteger.GetInstance(asn1Encodable).PositiveValue;
					}
					else
					{
						byte[] bytes2 = Arrays.Reverse(Asn1OctetString.GetInstance(asn1Encodable).GetOctets());
						bigInteger = new BigInteger(1, bytes2);
					}
				}
			}
			else
			{
				X962Parameters instance7 = X962Parameters.GetInstance(keyInfo.PrivateKeyAlgorithm.Parameters);
				if (instance7.IsNamedCurve)
				{
					DerObjectIdentifier instance8 = DerObjectIdentifier.GetInstance(instance7.Parameters);
					X9ECParameters byOid = ECNamedCurveTable.GetByOid(instance8);
					eCGost3410Parameters = new ECGost3410Parameters(new ECNamedDomainParameters(instance8, byOid), instance6.PublicKeyParamSet, instance6.DigestParamSet, instance6.EncryptionParamSet);
				}
				else if (instance7.IsImplicitlyCA)
				{
					eCGost3410Parameters = null;
				}
				else
				{
					X9ECParameters instance9 = X9ECParameters.GetInstance(instance7.Parameters);
					eCGost3410Parameters = new ECGost3410Parameters(new ECNamedDomainParameters(algorithm, instance9), instance6.PublicKeyParamSet, instance6.DigestParamSet, instance6.EncryptionParamSet);
				}
				Asn1Encodable asn1Encodable2 = keyInfo.ParsePrivateKey();
				bigInteger = ((!(asn1Encodable2 is DerInteger)) ? ECPrivateKeyStructure.GetInstance(asn1Encodable2).GetKey() : DerInteger.GetInstance(asn1Encodable2).Value);
			}
			return new ECPrivateKeyParameters(bigInteger, new ECGost3410Parameters(eCGost3410Parameters, instance6.PublicKeyParamSet, instance6.DigestParamSet, instance6.EncryptionParamSet));
		}
		throw new SecurityUtilityException("algorithm identifier in private key not recognised");
	}

	private static byte[] GetRawKey(PrivateKeyInfo keyInfo, int expectedSize)
	{
		byte[] octets = Asn1OctetString.GetInstance(keyInfo.ParsePrivateKey()).GetOctets();
		if (expectedSize != octets.Length)
		{
			throw new SecurityUtilityException("private key encoding has incorrect length");
		}
		return octets;
	}

	public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, EncryptedPrivateKeyInfo encInfo)
	{
		return CreateKey(PrivateKeyInfoFactory.CreatePrivateKeyInfo(passPhrase, encInfo));
	}

	public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, byte[] encryptedPrivateKeyInfoData)
	{
		return DecryptKey(passPhrase, Asn1Object.FromByteArray(encryptedPrivateKeyInfoData));
	}

	public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Stream encryptedPrivateKeyInfoStream)
	{
		return DecryptKey(passPhrase, Asn1Object.FromStream(encryptedPrivateKeyInfoStream));
	}

	private static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Asn1Object asn1Object)
	{
		return DecryptKey(passPhrase, EncryptedPrivateKeyInfo.GetInstance(asn1Object));
	}

	public static byte[] EncryptKey(DerObjectIdentifier algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
	{
		return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
	}

	public static byte[] EncryptKey(string algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
	{
		return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
	}
}
