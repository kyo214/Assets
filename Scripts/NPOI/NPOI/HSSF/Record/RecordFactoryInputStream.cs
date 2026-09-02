using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.Record.Chart;
using NPOI.HSSF.Record.Crypto;
using NPOI.POIFS.Crypt;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RecordFactoryInputStream
{
	private class StreamEncryptionInfo
	{
		private int _InitialRecordsSize;

		private FilePassRecord _filePassRec;

		private Record _lastRecord;

		private bool _hasBOFRecord;

		public bool HasEncryption => _filePassRec != null;

		public Record LastRecord => _lastRecord;

		public bool HasBOFRecord => _hasBOFRecord;

		public StreamEncryptionInfo(RecordInputStream rs, List<Record> outputRecs)
		{
			rs.NextRecord();
			int num = 4 + rs.Remaining;
			Record record = RecordFactory.CreateSingleRecord(rs);
			outputRecs.Add(record);
			FilePassRecord filePassRec = null;
			if (record is BOFRecord)
			{
				_hasBOFRecord = true;
				if (rs.HasNextRecord)
				{
					rs.NextRecord();
					record = RecordFactory.CreateSingleRecord(rs);
					num += record.RecordSize;
					outputRecs.Add(record);
					if (record is WriteProtectRecord && rs.HasNextRecord)
					{
						rs.NextRecord();
						record = RecordFactory.CreateSingleRecord(rs);
						num += record.RecordSize;
						outputRecs.Add(record);
					}
					if (record is FilePassRecord)
					{
						filePassRec = (FilePassRecord)record;
						outputRecs.RemoveAt(outputRecs.Count - 1);
						record = outputRecs[0];
					}
					else if (record is EOFRecord)
					{
						throw new InvalidOperationException("Nothing between BOF and EOF");
					}
				}
			}
			else
			{
				_hasBOFRecord = false;
			}
			_InitialRecordsSize = num;
			_filePassRec = filePassRec;
			_lastRecord = record;
		}

		public RecordInputStream CreateDecryptingStream(Stream original)
		{
			_ = _filePassRec;
			if (Biff8EncryptionKey.CurrentUserPassword == null)
			{
				_ = Decryptor.DEFAULT_PASSWORD;
			}
			throw new NotImplementedException("Implement it based on poi 4.2 in the future");
		}
	}

	private RecordInputStream _recStream;

	private bool _shouldIncludeContinueRecords;

	private Record[] _unreadRecordBuffer;

	private int _unreadRecordIndex = -1;

	private Record _lastRecord;

	private DrawingRecord _lastDrawingRecord = new DrawingRecord();

	private int _bofDepth;

	private bool _lastRecordWasEOFLevelZero;

	public RecordFactoryInputStream(Stream in1, bool shouldIncludeContinueRecords)
	{
		RecordInputStream recordInputStream = new RecordInputStream(in1);
		List<Record> list = new List<Record>();
		StreamEncryptionInfo streamEncryptionInfo = new StreamEncryptionInfo(recordInputStream, list);
		if (streamEncryptionInfo.HasEncryption)
		{
			recordInputStream = streamEncryptionInfo.CreateDecryptingStream(in1);
		}
		if (list.Count != 0)
		{
			_unreadRecordBuffer = new Record[list.Count];
			_unreadRecordBuffer = list.ToArray();
			_unreadRecordIndex = 0;
		}
		_recStream = recordInputStream;
		_shouldIncludeContinueRecords = shouldIncludeContinueRecords;
		_lastRecord = streamEncryptionInfo.LastRecord;
		_bofDepth = (streamEncryptionInfo.HasBOFRecord ? 1 : 0);
		_lastRecordWasEOFLevelZero = false;
	}

	public Record NextRecord()
	{
		Record nextUnreadRecord = GetNextUnreadRecord();
		if (nextUnreadRecord != null)
		{
			return nextUnreadRecord;
		}
		do
		{
			if (!_recStream.HasNextRecord)
			{
				return null;
			}
			_recStream.NextRecord();
			if (_lastRecordWasEOFLevelZero && _recStream.Sid != 2057)
			{
				return null;
			}
			nextUnreadRecord = ReadNextRecord();
		}
		while (nextUnreadRecord == null);
		return nextUnreadRecord;
	}

	private Record GetNextUnreadRecord()
	{
		if (_unreadRecordBuffer != null)
		{
			int unreadRecordIndex = _unreadRecordIndex;
			if (unreadRecordIndex < _unreadRecordBuffer.Length)
			{
				Record result = _unreadRecordBuffer[unreadRecordIndex];
				_unreadRecordIndex = unreadRecordIndex + 1;
				return result;
			}
			_unreadRecordIndex = -1;
			_unreadRecordBuffer = null;
		}
		return null;
	}

	private Record ReadNextRecord()
	{
		Record record = RecordFactory.CreateSingleRecord(_recStream);
		_lastRecordWasEOFLevelZero = false;
		if (record is BOFRecord)
		{
			_bofDepth++;
			return record;
		}
		if (record is EOFRecord)
		{
			_bofDepth--;
			if (_bofDepth < 1)
			{
				_lastRecordWasEOFLevelZero = true;
			}
			return record;
		}
		if (record is DBCellRecord)
		{
			return null;
		}
		if (record is RKRecord)
		{
			return RecordFactory.ConvertToNumberRecord((RKRecord)record);
		}
		if (record is MulRKRecord)
		{
			Record[] unreadRecordBuffer = RecordFactory.ConvertRKRecords((MulRKRecord)record);
			Record[] array = (_unreadRecordBuffer = unreadRecordBuffer);
			_unreadRecordIndex = 1;
			return array[0];
		}
		if (record.Sid == 235 && _lastRecord is DrawingGroupRecord)
		{
			((DrawingGroupRecord)_lastRecord).Join((AbstractEscherHolderRecord)record);
			return null;
		}
		if (record.Sid == 60)
		{
			ContinueRecord continueRecord = (ContinueRecord)record;
			if (_lastRecord is ObjRecord || _lastRecord is TextObjectRecord)
			{
				_lastDrawingRecord.ProcessContinueRecord(continueRecord.Data);
				if (_shouldIncludeContinueRecords)
				{
					return record;
				}
				return null;
			}
			if (_lastRecord is DrawingGroupRecord)
			{
				((DrawingGroupRecord)_lastRecord).ProcessContinueRecord(continueRecord.Data);
				return null;
			}
			if (_lastRecord is DrawingRecord)
			{
				return continueRecord;
			}
			if (_lastRecord is CrtMlFrtRecord)
			{
				return record;
			}
			if (_lastRecord is UnknownRecord)
			{
				return record;
			}
			if (_lastRecord is EOFRecord)
			{
				return record;
			}
			throw new RecordFormatException("Unhandled Continue Record");
		}
		_lastRecord = record;
		if (record is DrawingRecord)
		{
			_lastDrawingRecord = (DrawingRecord)record;
		}
		return record;
	}
}
