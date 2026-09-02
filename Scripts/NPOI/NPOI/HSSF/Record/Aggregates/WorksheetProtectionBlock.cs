using NPOI.HSSF.Model;
using NPOI.POIFS.Crypt;
using NPOI.Util;

namespace NPOI.HSSF.Record.Aggregates;

public class WorksheetProtectionBlock : RecordAggregate
{
	private ProtectRecord _protectRecord;

	private ObjectProtectRecord _objectProtectRecord;

	private ScenarioProtectRecord _scenarioProtectRecord;

	private PasswordRecord _passwordRecord;

	private ProtectRecord Protect
	{
		get
		{
			if (_protectRecord == null)
			{
				_protectRecord = new ProtectRecord(isProtected: false);
			}
			return _protectRecord;
		}
	}

	public PasswordRecord Password
	{
		get
		{
			if (_passwordRecord == null)
			{
				_passwordRecord = CreatePassword();
			}
			return _passwordRecord;
		}
	}

	public bool IsSheetProtected
	{
		get
		{
			if (_protectRecord != null)
			{
				return _protectRecord.Protect;
			}
			return false;
		}
	}

	public bool IsObjectProtected
	{
		get
		{
			if (_objectProtectRecord != null)
			{
				return _objectProtectRecord.Protect;
			}
			return false;
		}
	}

	public bool IsScenarioProtected
	{
		get
		{
			if (_scenarioProtectRecord != null)
			{
				return _scenarioProtectRecord.Protect;
			}
			return false;
		}
	}

	public int PasswordHash
	{
		get
		{
			if (_passwordRecord == null)
			{
				return 0;
			}
			return _passwordRecord.Password;
		}
	}

	public static bool IsComponentRecord(int sid)
	{
		if ((uint)(sid - 18) <= 1u || sid == 99 || sid == 221)
		{
			return true;
		}
		return false;
	}

	private bool ReadARecord(RecordStream rs)
	{
		switch (rs.PeekNextSid())
		{
		case 18:
			CheckNotPresent(_protectRecord);
			_protectRecord = rs.GetNext() as ProtectRecord;
			break;
		case 99:
			CheckNotPresent(_objectProtectRecord);
			_objectProtectRecord = rs.GetNext() as ObjectProtectRecord;
			break;
		case 221:
			CheckNotPresent(_scenarioProtectRecord);
			_scenarioProtectRecord = rs.GetNext() as ScenarioProtectRecord;
			break;
		case 19:
			CheckNotPresent(_passwordRecord);
			_passwordRecord = rs.GetNext() as PasswordRecord;
			break;
		default:
			return false;
		}
		return true;
	}

	private void CheckNotPresent(Record rec)
	{
		if (rec != null)
		{
			throw new RecordFormatException("Duplicate WorksheetProtectionBlock record (sid=0x" + StringUtil.ToHexString(rec.Sid) + ")");
		}
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		VisitIfPresent(_protectRecord, rv);
		VisitIfPresent(_objectProtectRecord, rv);
		VisitIfPresent(_scenarioProtectRecord, rv);
		VisitIfPresent(_passwordRecord, rv);
	}

	private static void VisitIfPresent(Record r, RecordVisitor rv)
	{
		if (r != null)
		{
			rv.VisitRecord(r);
		}
	}

	public PasswordRecord GetPasswordRecord()
	{
		return _passwordRecord;
	}

	public ScenarioProtectRecord GetHCenter()
	{
		return _scenarioProtectRecord;
	}

	public void AddRecords(RecordStream rs)
	{
		while (ReadARecord(rs))
		{
		}
	}

	public void ProtectSheet(string password, bool shouldProtectObjects, bool shouldProtectScenarios)
	{
		if (password == null)
		{
			_passwordRecord = null;
			_protectRecord = null;
			_objectProtectRecord = null;
			_scenarioProtectRecord = null;
			return;
		}
		ProtectRecord protect = Protect;
		PasswordRecord password2 = Password;
		protect.Protect = true;
		password2.Password = (short)CryptoFunctions.CreateXorVerifier1(password);
		if ((_objectProtectRecord == null) & shouldProtectObjects)
		{
			ObjectProtectRecord objectProtectRecord = CreateObjectProtect();
			objectProtectRecord.Protect = true;
			_objectProtectRecord = objectProtectRecord;
		}
		if ((_scenarioProtectRecord == null) & shouldProtectScenarios)
		{
			ScenarioProtectRecord scenarioProtectRecord = CreateScenarioProtect();
			scenarioProtectRecord.Protect = true;
			_scenarioProtectRecord = scenarioProtectRecord;
		}
	}

	private static ObjectProtectRecord CreateObjectProtect()
	{
		return new ObjectProtectRecord
		{
			Protect = false
		};
	}

	private static ScenarioProtectRecord CreateScenarioProtect()
	{
		return new ScenarioProtectRecord
		{
			Protect = false
		};
	}

	private static PasswordRecord CreatePassword()
	{
		return new PasswordRecord(0);
	}
}
