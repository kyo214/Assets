using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Doozy.Runtime.Common.Extensions;

public static class StringExtensions
{
	public static string RemoveWhitespaces(this string target)
	{
		return Regex.Replace(target, "\\s+", "");
	}

	public static string ConvertWhitespacesToSingleSpaces(this string target)
	{
		return Regex.Replace(target, "\\s+", " ");
	}

	public static string ReverseSlash(this string target, int direction)
	{
		return direction switch
		{
			0 => target.Replace("/", "\\"), 
			1 => target.Replace("\\", "/"), 
			_ => target, 
		};
	}

	public static string LeftOf(this string target, char c)
	{
		int num = target.IndexOf(c);
		if (num < 0)
		{
			return target;
		}
		return target.Substring(0, num);
	}

	public static string RightOf(this string target, char c)
	{
		int num = target.IndexOf(c);
		if (num != -1)
		{
			return target.Substring(num + 1);
		}
		return target;
	}

	public static string RemoveLastCharacter(this string target)
	{
		if (target.Length <= 0)
		{
			return target;
		}
		return target.Substring(0, target.Length - 1);
	}

	public static string RemoveLast(this string target, int numberOfCharactersToRemove)
	{
		if (!target.IsNullOrEmpty())
		{
			return target.Substring(0, target.Length - numberOfCharactersToRemove);
		}
		return string.Empty;
	}

	public static string RemoveFirstCharacter(this string target)
	{
		return target.Substring(1);
	}

	public static string RemoveFirst(this string target, int numberOfCharactersToRemove)
	{
		return target.Substring(numberOfCharactersToRemove);
	}

	public static string RemoveAllSpecialCharacters(this string target)
	{
		StringBuilder stringBuilder = new StringBuilder(target.Length);
		foreach (char item in target.Where(char.IsLetterOrDigit))
		{
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	public static string RemoveAllEmptyLines(this string target)
	{
		return Regex.Replace(target, "^\\s*$\\n|\\r", string.Empty, RegexOptions.Multiline).TrimEnd();
	}

	public static string ReplaceLineFeeds(this string target)
	{
		return Regex.Replace(target, "^[\\r\\n]+|\\.|[\\r\\n]+$", "");
	}

	public static bool IsNull(this string target)
	{
		return target == null;
	}

	public static bool IsNullOrEmpty(this string target)
	{
		return string.IsNullOrEmpty(target);
	}

	public static bool IsMinLength(this string target, int minCharLength)
	{
		if (target != null)
		{
			return target.Length >= minCharLength;
		}
		return false;
	}

	public static bool IsMaxLength(this string target, int maxCharLength)
	{
		if (target != null)
		{
			return target.Length <= maxCharLength;
		}
		return false;
	}

	public static bool IsLength(this string target, int minCharLength, int maxCharLength)
	{
		if (target != null && target.Length >= minCharLength)
		{
			return target.Length <= minCharLength;
		}
		return false;
	}

	public static int? GetLength(string target)
	{
		return target?.Length;
	}

	public static string Left(this string target, int length)
	{
		if (!string.IsNullOrEmpty(target))
		{
			if (length >= 0 && length <= target.Length)
			{
				return target.Substring(0, length);
			}
			throw new ArgumentOutOfRangeException("length", "Length cannot be higher than total string length or less than 0");
		}
		throw new ArgumentNullException("target");
	}

	public static string Right(this string target, int length)
	{
		if (!string.IsNullOrEmpty(target))
		{
			if (length >= 0 && length <= target.Length)
			{
				return target.Substring(target.Length - length);
			}
			throw new ArgumentOutOfRangeException("length", "Length cannot be higher than total string length or less than 0");
		}
		throw new ArgumentNullException("target");
	}

	public static bool DoesNotStartWith(this string target, string prefix)
	{
		if (target != null && prefix != null)
		{
			return !target.StartsWith(prefix, StringComparison.InvariantCulture);
		}
		return true;
	}

	public static bool DoesNotEndWith(this string target, string suffix)
	{
		if (target != null && suffix != null)
		{
			return !target.EndsWith(suffix, StringComparison.InvariantCulture);
		}
		return true;
	}

	public static string RemovePrefix(this string target, string prefix, bool ignoreCase = true)
	{
		if (string.IsNullOrEmpty(target) || !(ignoreCase ? target.StartsWithIgnoreCase(prefix) : target.StartsWith(prefix)))
		{
			return target;
		}
		return target.Substring(prefix.Length, target.Length - prefix.Length);
	}

	public static string RemoveSuffix(this string target, string suffix, bool ignoreCase = true)
	{
		if (string.IsNullOrEmpty(target) || !(ignoreCase ? target.EndsWithIgnoreCase(suffix) : target.EndsWith(suffix)))
		{
			return string.Empty;
		}
		return target.Substring(0, target.Length - suffix.Length);
	}

	public static string AppendSuffixIfMissing(this string target, string suffix, bool ignoreCase = true)
	{
		if (!string.IsNullOrEmpty(target) && !(ignoreCase ? target.EndsWithIgnoreCase(suffix) : target.EndsWith(suffix)))
		{
			return target + suffix;
		}
		return target;
	}

	public static string AppendPrefixIfMissing(this string target, string prefix, bool ignoreCase = true)
	{
		if (!string.IsNullOrEmpty(target) && !(ignoreCase ? target.StartsWithIgnoreCase(prefix) : target.StartsWith(prefix)))
		{
			return prefix + target;
		}
		return target;
	}

	public static string Capitalize(this string target)
	{
		if (target.Length != 0)
		{
			return target.Substring(0, 1).ToUpper() + target.Substring(1).ToLower();
		}
		return target;
	}

	public static string FirstCharacter(this string target)
	{
		if (string.IsNullOrEmpty(target))
		{
			return null;
		}
		if (target.Length < 1)
		{
			return target;
		}
		return target.Substring(0, 1);
	}

	public static string LastCharacter(this string target)
	{
		if (string.IsNullOrEmpty(target))
		{
			return null;
		}
		if (target.Length < 1)
		{
			return target;
		}
		return target.Substring(target.Length - 1, 1);
	}

	public static bool EndsWithIgnoreCase(this string target, string suffix)
	{
		if (target != null)
		{
			if (suffix != null)
			{
				if (target.Length >= suffix.Length)
				{
					return target.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);
				}
				return false;
			}
			throw new ArgumentNullException("suffix", "Suffix parameter is null");
		}
		throw new ArgumentNullException("target", "Target parameter is null");
	}

	public static bool StartsWithIgnoreCase(this string target, string prefix)
	{
		if (target != null)
		{
			if (prefix != null)
			{
				if (target.Length >= prefix.Length)
				{
					return target.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
				}
				return false;
			}
			throw new ArgumentNullException("prefix", "Prefix parameter is null");
		}
		throw new ArgumentNullException("target", "Target parameter is null");
	}

	public static string Replace(this string target, params char[] chars)
	{
		return chars.Aggregate(target, (string current, char c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));
	}

	public static string RemoveChars(this string target, params char[] chars)
	{
		StringBuilder stringBuilder = new StringBuilder(target.Length);
		foreach (char item in target.Where((char c) => !chars.Contains(c)))
		{
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	public static bool IsEmailAddress(this string target)
	{
		return Regex.Match(target, "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$").Success;
	}

	public static string Reverse(this string target)
	{
		char[] array = new char[target.Length];
		int num = target.Length - 1;
		int num2 = 0;
		while (num >= 0)
		{
			array[num2] = target[num];
			num--;
			num2++;
		}
		target = new string(array);
		return target;
	}

	public static int CountOccurrences(this string target, string stringToMatch)
	{
		return Regex.Matches(target, stringToMatch, RegexOptions.IgnoreCase).Count;
	}

	public static bool IsAlpha(this string target)
	{
		if (!string.IsNullOrEmpty(target))
		{
			return target.Trim().Replace(" ", "").All(char.IsLetter);
		}
		return false;
	}

	public static bool IsAlphaNumeric(this string target)
	{
		if (!string.IsNullOrEmpty(target))
		{
			return target.Trim().Replace(" ", "").All(char.IsLetterOrDigit);
		}
		return false;
	}

	public static string Encrypt(this string target, string key)
	{
		return BitConverter.ToString(new RSACryptoServiceProvider(new CspParameters
		{
			KeyContainerName = key
		})
		{
			PersistKeyInCsp = true
		}.Encrypt(Encoding.UTF8.GetBytes(target), fOAEP: true));
	}

	public static string Decrypt(this string target, string key)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters
		{
			KeyContainerName = key
		})
		{
			PersistKeyInCsp = true
		};
		byte[] rgb = Array.ConvertAll(target.Split(new string[1] { "-" }, StringSplitOptions.None), (string s) => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber)));
		byte[] bytes = rSACryptoServiceProvider.Decrypt(rgb, fOAEP: true);
		return Encoding.UTF8.GetString(bytes);
	}

	public static int GetByteSize(this string target, Encoding encoding)
	{
		if (target != null)
		{
			if (encoding != null)
			{
				return encoding.GetByteCount(target);
			}
			throw new ArgumentNullException("encoding");
		}
		throw new ArgumentNullException("target");
	}
}
