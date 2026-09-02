using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toked;

public class MathFunc : MonoBehaviour
{
	public static Vector3 AngleToVector3(float angle)
	{
		float f = angle * (MathF.PI / 180f);
		float x = Mathf.Sin(f);
		float z = Mathf.Cos(f);
		return new Vector3(x, 0f, z).normalized;
	}

	public static int ExtractNumber(string input)
	{
		Match match = Regex.Match(input, "\\d+");
		if (match.Success)
		{
			return int.Parse(match.Value);
		}
		return 0;
	}

	public static int ConvertStringToInt(string input)
	{
		char[] array = input.ToUpper().ToCharArray();
		bool flag = false;
		string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890";
		for (int i = 0; i < array.Length; i++)
		{
			flag = false;
			for (int j = 0; j < text.Length; j++)
			{
				if (array[i] == text[j])
				{
					array[i] = (j % 10).ToString()[0];
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				array[i] = ' ';
			}
		}
		text = new string(array).Replace(" ", "").Substring(0, 6);
		return int.Parse(text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(Vector3 a, Vector3 b)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSameYPos(Vector3 a, Vector3 b)
	{
		float num = a.x - b.x;
		float num2 = a.y - a.y;
		float num3 = a.z - b.z;
		return Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	public static int GetSecond(float interval)
	{
		return Mathf.FloorToInt(interval % 60f);
	}

	public static int GetMinute(float interval)
	{
		return Mathf.FloorToInt(interval / 60f);
	}

	public static int GetMinuteHour(float interval)
	{
		return Mathf.FloorToInt(interval / 60f % 60f);
	}

	public static int GetHour(float interval)
	{
		return Mathf.FloorToInt(interval / 3600f);
	}

	public static string GetHourMinuteSecond(float interval)
	{
		return GetHour(interval).ToString("D2") + " : " + GetMinuteHour(interval).ToString("D2") + " : " + GetSecond(interval).ToString("D2");
	}

	public static Vector3 IsoDirection(Vector3 theDirection)
	{
		return Matrix4x4.Rotate(Quaternion.Euler(0f, CameraGame.Instance.camRotate, 0f)).MultiplyPoint3x4(theDirection);
	}

	public static Vector3 GetTargetPosition(Vector3 posInit, Vector3 posHit, float percentDistToTarget)
	{
		Vector3 vector = posHit - posInit;
		Vector3 vector2 = vector * percentDistToTarget;
		return posInit + (vector - vector2);
	}

	public static ulong EncodeVector3ToULong(Vector3 v)
	{
		long num = Mathf.RoundToInt(v.x * 100f) + 32768;
		ulong num2 = (ulong)(Mathf.RoundToInt(v.y * 100f) + 32768);
		ulong num3 = (ulong)(Mathf.RoundToInt(v.z * 100f) + 32768);
		return (ulong)(num + (long)(num2 * 65536)) + num3 * 4294967296L;
	}

	public static Vector3 DecodeVector3FromULong(ulong i)
	{
		ulong num = i / 4294967296L;
		ulong num2 = (i - num * 4294967296L) / 65536;
		return new Vector3(((float)(i - num2 * 65536 - num * 4294967296L) - 32768f) / 100f, ((float)num2 - 32768f) / 100f, ((float)num - 32768f) / 100f);
	}

	public static string[] SplitString(string input, char separator)
	{
		if (string.IsNullOrEmpty(input))
		{
			throw new ArgumentException("Input string cannot be null or empty.", "input");
		}
		return input.Split(separator);
	}

	public static float PositionToFloat(Vector3 pos)
	{
		return pos.x * 0.01f + pos.z * 0.01f;
	}

	public static Vector3 FloatToPosition(float value)
	{
		float x = value * 100f * 0.5f;
		float z = value * 100f * 0.5f;
		return new Vector3(x, 0f, z);
	}

	public static Vector3 CalculateParabolicVelocity(Vector3 destination, Vector3 currentPosition, float arcProjectory = 0.6f, float timeFactor = 0.3f)
	{
		float num = Mathf.Abs(Physics.gravity.y);
		Vector3 vector = destination - currentPosition;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		float value = Mathf.Sqrt(vector2.magnitude) * timeFactor;
		value = Mathf.Clamp(value, 0.3f, 1.8f);
		Vector3 vector3 = vector2 / value;
		float num2 = (vector.y + 0.5f * num * value * value) / value;
		float num3 = Mathf.Lerp(1f, 1.5f, arcProjectory);
		num2 *= num3;
		return vector3 + Vector3.up * num2;
	}

	public static void Shuffle<T>(T[] array)
	{
		for (int num = array.Length - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			T val = array[num];
			array[num] = array[num2];
			array[num2] = val;
		}
	}

	public static void SplitUlongToArrInt(ulong value, out int low, out int high)
	{
		low = (int)(value & 0xFFFFFFFFu);
		high = (int)(value >> 32);
	}

	public static ulong JoinArrIntToUlong(int low, int high)
	{
		return (uint)low | ((ulong)(uint)high << 32);
	}

	public static int JoinArrInt(int digit0, int digit1, int digit2, int digit3, int value0, int value1, int value2, int value3)
	{
		int num = Pow10(digit1);
		int num2 = Pow10(digit2);
		int num3 = Pow10(digit3);
		return ((value0 * num + value1) * num2 + value2) * num3 + value3;
	}

	public static void Split(int packed, int digit0, int digit1, int digit2, int digit3, out int value0, out int value1, out int value2, out int value3)
	{
		int num = Pow10(digit3);
		int num2 = Pow10(digit2);
		int num3 = Pow10(digit1);
		value3 = packed % num;
		packed /= num;
		value2 = packed % num2;
		packed /= num2;
		value1 = packed % num3;
		packed /= num3;
		value0 = packed;
	}

	private static int Pow10(int digit)
	{
		int num = 1;
		for (int i = 0; i < digit; i++)
		{
			num *= 10;
		}
		return num;
	}

	public static int[] ObjectToInt32<T>(T data)
	{
		string s = JsonUtility.ToJson(data);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		int[] array = new int[(bytes.Length + 3) / 4];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		Debug.Log("Total Leaderboard Data " + array.Length);
		return array;
	}

	public static int[] ObjectToInt32Compressed<T>(T data)
	{
		string s = JsonUtility.ToJson(data);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal))
		{
			deflateStream.Write(bytes, 0, bytes.Length);
		}
		byte[] array = memoryStream.ToArray();
		int[] array2 = new int[(array.Length + 3) / 4];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Debug.Log("Total Leaderboard Data V2 " + array2.Length);
		return array2;
	}

	public static T Int32ToObject<T>(int[] details)
	{
		byte[] array = new byte[details.Length * 4];
		Buffer.BlockCopy(details, 0, array, 0, array.Length);
		return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(array).TrimEnd('\0'));
	}

	public static T Int32CompressedToObject<T>(int[] details)
	{
		if (details == null || details.Length == 0)
		{
			Debug.LogError("Data int[] kosong atau null!");
			return default;
		}
		byte[] array = new byte[details.Length * 4];
		Buffer.BlockCopy(details, 0, array, 0, array.Length);
		MemoryStream memoryStream = null;
		MemoryStream memoryStream2 = null;
		DeflateStream deflateStream = null;
		try
		{
			memoryStream = new MemoryStream(array);
			memoryStream2 = new MemoryStream();
			deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress);
			deflateStream.CopyTo(memoryStream2);
			byte[] bytes = memoryStream2.ToArray();
			return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bytes));
		}
		catch (IOException)
		{
			return default;
		}
		catch (Exception)
		{
			return default;
		}
		finally
		{
			deflateStream?.Dispose();
			memoryStream2?.Dispose();
			memoryStream?.Dispose();
		}
	}
}
