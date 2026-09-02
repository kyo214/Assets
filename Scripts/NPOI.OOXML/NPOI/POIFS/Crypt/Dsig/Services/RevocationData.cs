using System;
using System.Collections.Generic;

namespace NPOI.POIFS.Crypt.Dsig.Services;

public class RevocationData
{
	private List<byte[]> crls;

	private List<byte[]> ocsps;

	public RevocationData()
	{
		crls = new List<byte[]>();
		ocsps = new List<byte[]>();
	}

	public void AddCRL(byte[] encodedCrl)
	{
		crls.Add(encodedCrl);
	}

	public void AddCRL(X509CRL crl)
	{
		byte[] encoded;
		try
		{
			encoded = crl.getEncoded();
		}
		catch (CRLException ex)
		{
			throw new ArgumentException("CRL coding error: " + ex.Message, ex);
		}
		AddCRL(encoded);
	}

	public void AddOCSP(byte[] encodedOcsp)
	{
		ocsps.Add(encodedOcsp);
	}

	public List<byte[]> GetCRLs()
	{
		return crls;
	}

	public List<byte[]> GetOCSPs()
	{
		return ocsps;
	}

	public bool HasOCSPs()
	{
		return ocsps.Count > 0;
	}

	public bool HasCRLs()
	{
		return crls.Count > 0;
	}

	public bool HasRevocationDataEntries()
	{
		if (!HasOCSPs())
		{
			return HasCRLs();
		}
		return true;
	}
}
