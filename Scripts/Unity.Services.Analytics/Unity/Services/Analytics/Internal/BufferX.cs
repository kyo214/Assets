using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unity.Services.Analytics.Internal;

internal class BufferX : IBuffer, IBufferDebug
{
	private const long k_UploadBatchMaximumSizeInBytes = 4194304L;

	private const string k_MillisecondDateFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";

	private readonly byte[] k_WorkingBuffer;

	private readonly char[] k_WorkingCharacterBuffer;

	private readonly byte[] k_PayloadHeader;

	private readonly byte[] k_HeaderEventName;

	private readonly byte[] k_HeaderUserName;

	private readonly byte[] k_HeaderSessionID;

	private readonly byte[] k_HeaderEventUUID;

	private readonly byte[] k_HeaderTimestamp;

	private readonly byte[] k_HeaderEventVersion;

	private readonly byte[] k_HeaderInstallationID;

	private readonly byte[] k_HeaderPlayerID;

	private readonly byte[] k_HeaderOpenEventParams;

	private readonly byte[] k_CloseEvent;

	private readonly byte k_Quote;

	private readonly byte[] k_QuoteColon;

	private readonly byte[] k_QuoteComma;

	private readonly byte[] k_Comma;

	private readonly byte[] k_OpenBrace;

	private readonly byte[] k_CloseBraceComma;

	private readonly byte[] k_OpenBracket;

	private readonly byte[] k_CloseBracketComma;

	private readonly byte k_Colon;

	private readonly byte k_Dash;

	private readonly byte k_Space;

	private readonly byte k_Point;

	private readonly byte k_Positive;

	private readonly byte k_Negative;

	private readonly byte[] k_True;

	private readonly byte[] k_False;

	private readonly byte[] k_Int2CharacterByte;

	private readonly long[] k_Order;

	private readonly IBufferSystemCalls m_SystemCalls;

	private readonly IDiskCache m_DiskCache;

	private readonly IIdentityManager m_UserIdentity;

	private readonly ISessionManager m_Session;

	private readonly List<EventSummary> m_EventSummaries;

	private string m_CurrentEventId;

	private string m_CurrentEventName;

	private DateTime m_CurrentEventTimestamp;

	private MemoryStream m_SpareBuffer;

	private MemoryStream m_Buffer;

	public int Length => (int)m_Buffer.Length;

	internal int EventsRecorded => m_EventSummaries.Count;

	internal IReadOnlyList<EventSummary> EventSummaries => m_EventSummaries;

	internal byte[] RawContents => m_Buffer.ToArray();

	public event Action<string, string, DateTime, byte[]> EventRecorded;

	public event Action<HashSet<string>> EventsClearing;

	public event Action<HashSet<string>> EventsCleared;

	public BufferX(IBufferSystemCalls eventIdGenerator, IDiskCache diskCache, IIdentityManager userIdentity, ISessionManager session)
	{
		m_Buffer = new MemoryStream(4194304);
		m_SpareBuffer = new MemoryStream(4194304);
		m_EventSummaries = new List<EventSummary>();
		m_SystemCalls = eventIdGenerator;
		m_DiskCache = diskCache;
		m_UserIdentity = userIdentity;
		m_Session = session;
		k_WorkingBuffer = new byte[4194304];
		k_WorkingCharacterBuffer = new char[4194304];
		k_PayloadHeader = Encoding.UTF8.GetBytes("{\"eventList\":[");
		k_HeaderEventName = Encoding.UTF8.GetBytes("{\"eventName\":\"");
		k_HeaderUserName = Encoding.UTF8.GetBytes("\",\"userID\":\"");
		k_HeaderSessionID = Encoding.UTF8.GetBytes("\",\"sessionID\":\"");
		k_HeaderEventUUID = Encoding.UTF8.GetBytes("\",\"eventUUID\":\"");
		k_HeaderTimestamp = Encoding.UTF8.GetBytes("\",\"eventTimestamp\":\"");
		k_HeaderEventVersion = Encoding.UTF8.GetBytes("\"eventVersion\":");
		k_HeaderInstallationID = Encoding.UTF8.GetBytes("\"unityInstallationID\":\"");
		k_HeaderPlayerID = Encoding.UTF8.GetBytes("\"unityPlayerID\":\"");
		k_HeaderOpenEventParams = Encoding.UTF8.GetBytes("\"eventParams\":{");
		k_CloseEvent = Encoding.UTF8.GetBytes("}},");
		k_Quote = Encoding.UTF8.GetBytes("\"")[0];
		k_QuoteColon = Encoding.UTF8.GetBytes("\":");
		k_QuoteComma = Encoding.UTF8.GetBytes("\",");
		k_Comma = Encoding.UTF8.GetBytes(",");
		k_OpenBrace = Encoding.UTF8.GetBytes("{");
		k_CloseBraceComma = Encoding.UTF8.GetBytes("},");
		k_OpenBracket = Encoding.UTF8.GetBytes("[");
		k_CloseBracketComma = Encoding.UTF8.GetBytes("],");
		k_Colon = Encoding.UTF8.GetBytes(":")[0];
		k_Dash = Encoding.UTF8.GetBytes("-")[0];
		k_Space = Encoding.UTF8.GetBytes(" ")[0];
		k_Point = Encoding.UTF8.GetBytes(".")[0];
		k_Positive = Encoding.UTF8.GetBytes("+")[0];
		k_Negative = Encoding.UTF8.GetBytes("-")[0];
		k_True = Encoding.UTF8.GetBytes("true");
		k_False = Encoding.UTF8.GetBytes("false");
		k_Int2CharacterByte = new byte[10] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
		k_Order = new long[19]
		{
			1L, 10L, 100L, 1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 1000000000L,
			10000000000L, 100000000000L, 1000000000000L, 10000000000000L, 100000000000000L, 1000000000000000L, 10000000000000000L, 100000000000000000L, 1000000000000000000L
		};
		ClearBuffer();
	}

	private void WriteString(in string value)
	{
		int bytes = Encoding.UTF8.GetBytes(value, 0, Mathf.Min(value.Length, k_WorkingBuffer.Length), k_WorkingBuffer, 0);
		m_Buffer.Write(k_WorkingBuffer, 0, bytes);
	}

	private void WriteLong(in long value)
	{
		int count = SerializeLong(in value, in k_WorkingBuffer, 0, 0);
		m_Buffer.Write(k_WorkingBuffer, 0, count);
	}

	private void WriteByte(in byte value)
	{
		m_Buffer.WriteByte(value);
	}

	private void WriteBytes(in byte[] bytes)
	{
		m_Buffer.Write(bytes, 0, bytes.Length);
	}

	private void WriteName(string name)
	{
		if (name != null)
		{
			WriteByte(in k_Quote);
			WriteString(in name);
			WriteBytes(in k_QuoteColon);
		}
	}

	private void WriteDateTime(DateTime dateTime)
	{
		SerializeLong((long)dateTime.Year, in k_WorkingBuffer, 0, 4);
		k_WorkingBuffer[4] = k_Dash;
		SerializeLong((long)dateTime.Month, in k_WorkingBuffer, 5, 2);
		k_WorkingBuffer[7] = k_Dash;
		SerializeLong((long)dateTime.Day, in k_WorkingBuffer, 8, 2);
		k_WorkingBuffer[10] = k_Space;
		SerializeLong((long)dateTime.Hour, in k_WorkingBuffer, 11, 2);
		k_WorkingBuffer[13] = k_Colon;
		SerializeLong((long)dateTime.Minute, in k_WorkingBuffer, 14, 2);
		k_WorkingBuffer[16] = k_Colon;
		SerializeLong((long)dateTime.Second, in k_WorkingBuffer, 17, 2);
		k_WorkingBuffer[19] = k_Point;
		SerializeLong((long)dateTime.Millisecond, in k_WorkingBuffer, 20, 3);
		k_WorkingBuffer[23] = k_Space;
		TimeSpan timeZoneUtcOffset = m_SystemCalls.GetTimeZoneUtcOffset(dateTime);
		k_WorkingBuffer[24] = ((timeZoneUtcOffset.Ticks < 0) ? k_Negative : k_Positive);
		SerializeLong((long)Mathf.Abs(timeZoneUtcOffset.Hours), in k_WorkingBuffer, 25, 2);
		k_WorkingBuffer[27] = k_Colon;
		SerializeLong((long)Mathf.Abs(timeZoneUtcOffset.Minutes), in k_WorkingBuffer, 28, 2);
		m_Buffer.Write(k_WorkingBuffer, 0, 30);
	}

	private int SerializeLong(in long number, in byte[] buffer, in int startIndex, in int minimumLength)
	{
		if (number == 0L)
		{
			for (int i = 0; i <= minimumLength; i++)
			{
				buffer[startIndex + i] = k_Int2CharacterByte[0];
			}
			return Mathf.Max(1, minimumLength);
		}
		long num = Math.Abs(number);
		int num2 = Mathf.Max(b: (int)(Math.Log10(Math.Max(num, 0.5)) + 1.0), a: minimumLength);
		int num3 = startIndex;
		int num4 = num2;
		if (number < 0)
		{
			buffer[num3] = k_Negative;
			num3++;
			num4++;
		}
		long num5 = num;
		for (int num6 = num2; num6 > 0; num6--)
		{
			long num7 = num5 / k_Order[num6 - 1];
			num5 %= k_Order[num6 - 1];
			buffer[num3 + num2 - num6] = k_Int2CharacterByte[num7];
		}
		return num4;
	}

	public void PushStandardEventStart(string name, int version)
	{
		PushCommonEventStart(name);
		WriteBytes(in k_HeaderEventVersion);
		WriteLong((long)version);
		WriteBytes(in k_Comma);
		WriteBytes(in k_HeaderInstallationID);
		WriteString(m_UserIdentity.InstallId);
		WriteBytes(in k_QuoteComma);
		if (!string.IsNullOrEmpty(m_UserIdentity.PlayerId))
		{
			WriteBytes(in k_HeaderPlayerID);
			WriteString(m_UserIdentity.PlayerId);
			WriteBytes(in k_QuoteComma);
		}
		WriteBytes(in k_HeaderOpenEventParams);
	}

	public void PushCustomEventStart(string name)
	{
		PushCommonEventStart(name);
		WriteBytes(in k_HeaderOpenEventParams);
	}

	private void PushCommonEventStart(string name)
	{
		m_CurrentEventTimestamp = m_SystemCalls.Now();
		m_CurrentEventId = m_SystemCalls.GenerateGuid();
		m_CurrentEventName = name;
		WriteBytes(in k_HeaderEventName);
		WriteString(in m_CurrentEventName);
		WriteBytes(in k_HeaderUserName);
		WriteString(m_UserIdentity.UserId);
		WriteBytes(in k_HeaderSessionID);
		WriteString(m_Session.SessionId);
		WriteBytes(in k_HeaderEventUUID);
		WriteString(in m_CurrentEventId);
		WriteBytes(in k_HeaderTimestamp);
		WriteDateTime(m_CurrentEventTimestamp);
		WriteBytes(in k_QuoteComma);
	}

	private void StripTrailingCommaIfNecessary()
	{
		m_Buffer.Seek(-1L, SeekOrigin.End);
		if ((ushort)m_Buffer.ReadByte() == 44)
		{
			m_Buffer.Seek(-1L, SeekOrigin.Current);
			m_Buffer.SetLength(m_Buffer.Length - 1);
		}
	}

	public void PushEndEvent()
	{
		StripTrailingCommaIfNecessary();
		WriteBytes(in k_CloseEvent);
		int num = (int)m_Buffer.Length;
		int num2 = ((m_EventSummaries.Count > 0) ? m_EventSummaries[m_EventSummaries.Count - 1].EndIndex : 0);
		int num3 = num;
		if ((long)(num3 - num2) > 4194304L)
		{
			Debug.LogWarning($"Detected event that would be too big to upload (greater than {4096L}KB in size), discarding it to prevent blockage.");
			int num4 = ((m_EventSummaries.Count > 0) ? m_EventSummaries[m_EventSummaries.Count - 1].EndIndex : 0);
			m_Buffer.SetLength(num4);
			m_Buffer.Position = num4;
			return;
		}
		m_EventSummaries.Add(new EventSummary
		{
			StartIndex = num2,
			EndIndex = num3,
			Id = m_CurrentEventId
		});
		if (EventRecorded != null)
		{
			long position = m_Buffer.Position;
			m_Buffer.Seek(num2, SeekOrigin.Begin);
			int num5 = num3 - num2;
			byte[] array = new byte[num5];
			m_Buffer.Read(array, 0, num5);
			m_Buffer.Seek(position, SeekOrigin.Begin);
			EventRecorded(m_CurrentEventId, m_CurrentEventName, m_CurrentEventTimestamp, array);
		}
	}

	public void PushObjectStart(string name)
	{
		WriteName(name);
		WriteBytes(in k_OpenBrace);
	}

	public void PushObjectEnd()
	{
		StripTrailingCommaIfNecessary();
		WriteBytes(in k_CloseBraceComma);
	}

	public void PushArrayStart(string name)
	{
		WriteName(name);
		WriteBytes(in k_OpenBracket);
	}

	public void PushArrayEnd()
	{
		StripTrailingCommaIfNecessary();
		WriteBytes(in k_CloseBracketComma);
	}

	public void PushDouble(string name, double value)
	{
		WriteName(name);
		WriteString(value.ToString(CultureInfo.InvariantCulture));
		WriteBytes(in k_Comma);
	}

	public void PushFloat(string name, float value)
	{
		WriteName(name);
		WriteString(value.ToString(CultureInfo.InvariantCulture));
		WriteBytes(in k_Comma);
	}

	public void PushString(string name, string value)
	{
		if (Encoding.UTF8.GetByteCount(value) < k_WorkingBuffer.Length)
		{
			int num = 0;
			for (int i = 0; i < value.Length; i++)
			{
				num += ProcessCharacterOntoWorkingBuffer(num, value[i]);
				if (num >= k_WorkingCharacterBuffer.Length)
				{
					Debug.LogWarning("String value for field " + name + " is too long, it will not be recorded.\nValue:\n" + value.Substring(0, 128) + "...");
					break;
				}
			}
			if (num < k_WorkingCharacterBuffer.Length)
			{
				WriteName(name);
				WriteByte(in k_Quote);
				int bytes = Encoding.UTF8.GetBytes(k_WorkingCharacterBuffer, 0, num, k_WorkingBuffer, 0);
				m_Buffer.Write(k_WorkingBuffer, 0, bytes);
				WriteBytes(in k_QuoteComma);
			}
		}
		else
		{
			Debug.LogWarning("String value for field \"" + name + "\" is too long, it will not be recorded.\nValue:\n" + value.Substring(0, 128) + "...");
		}
	}

	private int ProcessCharacterOntoWorkingBuffer(int index, char character)
	{
		if (char.IsControl(character))
		{
			int num = 0;
			int num2 = Convert.ToInt32(character);
			string text = $"\\u{num2:X4}";
			for (int i = 0; i < text.Length; i++)
			{
				k_WorkingCharacterBuffer[index + i] = text[i];
				num++;
			}
			return num;
		}
		if (character == '"' || character == '\\')
		{
			k_WorkingCharacterBuffer[index] = '\\';
			k_WorkingCharacterBuffer[index + 1] = character;
			return 2;
		}
		k_WorkingCharacterBuffer[index] = character;
		return 1;
	}

	public void PushInt64(string name, long value)
	{
		WriteName(name);
		WriteLong(in value);
		WriteBytes(in k_Comma);
	}

	public void PushInt(string name, int value)
	{
		PushInt64(name, value);
	}

	public void PushBool(string name, bool value)
	{
		WriteName(name);
		if (value)
		{
			WriteBytes(in k_True);
		}
		else
		{
			WriteBytes(in k_False);
		}
		WriteBytes(in k_Comma);
	}

	public void PushTimestamp(string name, DateTime value)
	{
		WriteName(name);
		WriteByte(in k_Quote);
		WriteDateTime(value);
		WriteBytes(in k_QuoteComma);
	}

	public void PushProduct(string name, TransactionRealCurrency realCurrency, List<TransactionVirtualCurrency> virtualCurrencies, List<TransactionItem> items)
	{
		PushObjectStart(name);
		if (realCurrency != null)
		{
			PushObjectStart("realCurrency");
			realCurrency.Serialize(this);
			PushObjectEnd();
		}
		if (virtualCurrencies.Count > 0)
		{
			PushArrayStart("virtualCurrencies");
			foreach (TransactionVirtualCurrency virtualCurrency in virtualCurrencies)
			{
				PushObjectStart(null);
				PushObjectStart("virtualCurrency");
				virtualCurrency.Serialize(this);
				PushObjectEnd();
				PushObjectEnd();
			}
			PushArrayEnd();
		}
		if (items.Count > 0)
		{
			PushArrayStart("items");
			foreach (TransactionItem item in items)
			{
				PushObjectStart(null);
				PushObjectStart("item");
				item.Serialize(this);
				PushObjectEnd();
				PushObjectEnd();
			}
			PushArrayEnd();
		}
		PushObjectEnd();
	}

	public void PushObject(string name, object value)
	{
		if (value == null)
		{
			return;
		}
		Type type = value.GetType();
		if (type == typeof(string))
		{
			PushString(name, (string)value);
		}
		else if (type == typeof(int))
		{
			PushInt(name, (int)value);
		}
		else if (type == typeof(long))
		{
			PushInt64(name, (long)value);
		}
		else if (type == typeof(float))
		{
			PushFloat(name, (float)value);
		}
		else if (type == typeof(double))
		{
			PushDouble(name, (double)value);
		}
		else if (type == typeof(bool))
		{
			PushBool(name, (bool)value);
		}
		else if (type == typeof(DateTime))
		{
			PushTimestamp(name, (DateTime)value);
		}
		else if (value is Enum obj)
		{
			PushString(name, obj.ToString());
		}
		else if (value is IDictionary<string, object> dictionary)
		{
			PushObjectStart(name);
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				PushObject(item.Key, item.Value);
			}
			PushObjectEnd();
		}
		else if (value is IList<object> { Count: >0 } list)
		{
			PushArrayStart(name);
			for (int i = 0; i < list.Count; i++)
			{
				PushObject(null, list[i]);
			}
			PushArrayEnd();
		}
	}

	public byte[] Serialize()
	{
		if (m_EventSummaries.Count > 0)
		{
			long position = m_Buffer.Position;
			int endIndex = m_EventSummaries[0].EndIndex;
			int i;
			for (i = 0; i < m_EventSummaries.Count && (long)m_EventSummaries[i].EndIndex < 4194304L; i++)
			{
				endIndex = m_EventSummaries[i].EndIndex;
			}
			if (EventsClearing != null)
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
				for (int j = 0; j < i; j++)
				{
					hashSet.Add(m_EventSummaries[j].Id);
				}
				EventsClearing(hashSet);
			}
			byte[] array = new byte[k_PayloadHeader.Length + endIndex + 1];
			k_PayloadHeader.CopyTo(array, 0);
			m_Buffer.Position = 0L;
			m_Buffer.Read(array, k_PayloadHeader.Length, endIndex);
			byte[] bytes = Encoding.UTF8.GetBytes("]}");
			array[k_PayloadHeader.Length + endIndex - 1] = bytes[0];
			array[k_PayloadHeader.Length + endIndex] = bytes[1];
			m_Buffer.Position = position;
			return array;
		}
		return null;
	}

	public void ClearBuffer()
	{
		m_Buffer.SetLength(0L);
		m_Buffer.Position = 0L;
		m_EventSummaries.Clear();
	}

	public void ClearBuffer(long upTo)
	{
		if (m_EventSummaries.Count <= 0)
		{
			return;
		}
		MemoryStream buffer = m_Buffer;
		m_Buffer = m_SpareBuffer;
		m_SpareBuffer = buffer;
		int num = 0;
		int num2 = (int)upTo;
		for (int i = 0; i < m_EventSummaries.Count; i++)
		{
			EventSummary value = m_EventSummaries[i];
			value.StartIndex -= num2;
			value.EndIndex -= num2;
			m_EventSummaries[i] = value;
			if (m_EventSummaries[i].EndIndex <= 0)
			{
				num = i;
			}
		}
		if (EventsCleared != null)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			for (int j = 0; j <= num; j++)
			{
				hashSet.Add(m_EventSummaries[j].Id);
			}
			EventsCleared(hashSet);
		}
		m_EventSummaries.RemoveRange(0, num + 1);
		m_Buffer.SetLength(0L);
		m_Buffer.Position = 0L;
		m_SpareBuffer.Position = upTo;
		for (long num3 = upTo; num3 < m_SpareBuffer.Length; num3++)
		{
			byte value2 = (byte)m_SpareBuffer.ReadByte();
			m_Buffer.WriteByte(value2);
		}
		m_SpareBuffer.SetLength(0L);
		m_SpareBuffer.Position = 0L;
	}

	public void FlushToDisk()
	{
		m_DiskCache.Write(m_EventSummaries, m_Buffer);
	}

	public void ClearDiskCache()
	{
		m_DiskCache.Clear();
	}

	public void LoadFromDisk()
	{
		if (!m_DiskCache.Read(m_EventSummaries, m_Buffer))
		{
			ClearBuffer();
		}
	}

	internal static string SerializeDateTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture);
	}
}
