using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.POIFS.Crypt;
using NPOI.Util;
using Org.BouncyCastle.Security;

namespace NPOI.XSSF.UserModel.Helpers;

public class XSSFPasswordHelper
{
	private XSSFPasswordHelper()
	{
	}

	public static void SetPassword(CT_SheetProtection xobj, string password, HashAlgorithm hashAlgo, string prefix)
	{
		if (password == null)
		{
			xobj.password = null;
			xobj.algorithmName = null;
			xobj.hashValue = null;
			xobj.saltValue = null;
			xobj.spinCount = null;
		}
		else if (hashAlgo == null)
		{
			int num = CryptoFunctions.CreateXorVerifier1(password);
			xobj.password = $"{num:X4}".ToUpper();
		}
		else
		{
			byte[] array = new SecureRandom().GenerateSeed(16);
			int spinCount = 100000;
			byte[] inArray = CryptoFunctions.HashPassword(password, hashAlgo, array, spinCount, iteratorFirst: false);
			xobj.algorithmName = hashAlgo.jceId;
			xobj.hashValue = Convert.ToBase64String(inArray);
			xobj.saltValue = Convert.ToBase64String(array);
			xobj.spinCount = spinCount.ToString() ?? "";
		}
	}

	public static void SetPassword(XmlNode xobj, string password, HashAlgorithm hashAlgo, string prefix)
	{
		XPathNavigator xPathNavigator = xobj.CreateNavigator();
		if (password == null)
		{
			if (xPathNavigator.MoveToAttribute("password", prefix))
			{
				xPathNavigator.DeleteSelf();
			}
			if (xPathNavigator.MoveToAttribute("algorithmName", prefix))
			{
				xPathNavigator.DeleteSelf();
			}
			if (xPathNavigator.MoveToAttribute("hashValue", prefix))
			{
				xPathNavigator.DeleteSelf();
			}
			if (xPathNavigator.MoveToAttribute("saltValue", prefix))
			{
				xPathNavigator.DeleteSelf();
			}
			if (xPathNavigator.MoveToAttribute("spinCount", prefix))
			{
				xPathNavigator.DeleteSelf();
			}
		}
		else if (hashAlgo == null)
		{
			int num = CryptoFunctions.CreateXorVerifier1(password);
			xPathNavigator.CreateAttribute(prefix, "password", null, $"{num:X4}".ToUpper());
		}
		else
		{
			byte[] array = new SecureRandom().GenerateSeed(16);
			int spinCount = 100000;
			byte[] inArray = CryptoFunctions.HashPassword(password, hashAlgo, array, spinCount, iteratorFirst: false);
			xPathNavigator.CreateAttribute(prefix, "algorithmName", null, hashAlgo.jceId);
			xPathNavigator.CreateAttribute(prefix, "hashValue", null, Convert.ToBase64String(inArray));
			xPathNavigator.CreateAttribute(prefix, "saltValue", null, Convert.ToBase64String(array));
			xPathNavigator.CreateAttribute(prefix, "spinCount", null, spinCount.ToString() ?? "");
		}
	}

	public static bool ValidatePassword(CT_SheetProtection xobj, string password, string prefix)
	{
		if (password == null)
		{
			return false;
		}
		string password2 = xobj.password;
		string algorithmName = xobj.algorithmName;
		string hashValue = xobj.hashValue;
		string saltValue = xobj.saltValue;
		string spinCount = xobj.spinCount;
		if (password2 != null)
		{
			int num = int.Parse(password2, NumberStyles.HexNumber);
			int num2 = CryptoFunctions.CreateXorVerifier1(password);
			return num == num2;
		}
		if (hashValue == null || algorithmName == null || saltValue == null || spinCount == null)
		{
			return false;
		}
		byte[] a = Convert.FromBase64String(hashValue);
		HashAlgorithm hashAlgorithm = HashAlgorithm.FromString(algorithmName);
		byte[] salt = Convert.FromBase64String(saltValue);
		int spinCount2 = int.Parse(spinCount);
		byte[] b = CryptoFunctions.HashPassword(password, hashAlgorithm, salt, spinCount2, iteratorFirst: false);
		return Arrays.Equals(a, b);
	}

	public static bool ValidatePassword(XmlNode xobj, string password, string prefix)
	{
		if (password == null)
		{
			return false;
		}
		XPathNavigator xPathNavigator = xobj.CreateNavigator();
		xPathNavigator.MoveToAttribute("password", prefix);
		string value = xPathNavigator.Value;
		xPathNavigator.MoveToAttribute("algorithmName", prefix);
		string value2 = xPathNavigator.Value;
		xPathNavigator.MoveToAttribute("hashValue", prefix);
		string value3 = xPathNavigator.Value;
		xPathNavigator.MoveToAttribute("saltValue", prefix);
		string value4 = xPathNavigator.Value;
		xPathNavigator.MoveToAttribute("spinCount", prefix);
		string value5 = xPathNavigator.Value;
		if (value != null)
		{
			int num = int.Parse(value, NumberStyles.HexNumber);
			int num2 = CryptoFunctions.CreateXorVerifier1(password);
			return num == num2;
		}
		if (value3 == null || value2 == null || value4 == null || value5 == null)
		{
			return false;
		}
		byte[] a = Convert.FromBase64String(value3);
		HashAlgorithm hashAlgorithm = HashAlgorithm.FromString(value2);
		byte[] salt = Convert.FromBase64String(value4);
		int spinCount = int.Parse(value5);
		byte[] b = CryptoFunctions.HashPassword(password, hashAlgorithm, salt, spinCount, iteratorFirst: false);
		return Arrays.Equals(a, b);
	}

	private static XmlQualifiedName GetAttrName(string prefix, string name)
	{
		if (string.IsNullOrEmpty(prefix))
		{
			return new XmlQualifiedName(name);
		}
		return new XmlQualifiedName(prefix + char.ToUpper(name[0]) + name.Substring(1));
	}
}
