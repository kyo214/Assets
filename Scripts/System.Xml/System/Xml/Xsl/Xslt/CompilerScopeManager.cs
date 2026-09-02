using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt;

internal sealed class CompilerScopeManager<V>
{
	public enum ScopeFlags
	{
		BackwardCompatibility = 1,
		ForwardCompatibility = 2,
		CanHaveApplyImports = 4,
		NsDecl = 16,
		NsExcl = 32,
		Variable = 64,
		CompatibilityFlags = 3,
		InheritedFlags = 7,
		ExclusiveFlags = 112
	}

	public struct ScopeRecord
	{
		public int scopeCount;

		public ScopeFlags flags;

		public string ncName;

		public string nsUri;

		public V value;

		public bool IsVariable => (flags & ScopeFlags.Variable) != 0;

		public bool IsNamespace => (flags & ScopeFlags.NsDecl) != 0;
	}

	internal struct NamespaceEnumerator
	{
		private CompilerScopeManager<V> scope;

		private int lastRecord;

		private int currentRecord;

		public ScopeRecord Current => scope.records[currentRecord];

		public NamespaceEnumerator(CompilerScopeManager<V> scope)
		{
			this.scope = scope;
			lastRecord = scope.lastRecord;
			currentRecord = lastRecord + 1;
		}

		public void Reset()
		{
			currentRecord = lastRecord + 1;
		}

		public bool MoveNext()
		{
			while (0 < --currentRecord)
			{
				if (scope.records[currentRecord].IsNamespace && scope.LookupNamespace(scope.records[currentRecord].ncName, lastRecord, currentRecord + 1) == null)
				{
					return true;
				}
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003CGetActiveRecords_003Ed__34 : IEnumerable<ScopeRecord>, IEnumerable, IEnumerator<ScopeRecord>, IDisposable, IEnumerator
	{
		private int _003C_003E1__state;

		private ScopeRecord _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		public CompilerScopeManager<V> _003C_003E4__this;

		private int _003CcurrentRecord_003E5__2;

		ScopeRecord IEnumerator<ScopeRecord>.Current
		{
			[DebuggerHidden]
			get
			{
				return System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002EXml_002EXsl_002EXslt_002ECompilerScopeManager_003CV_003E_002EScopeRecord_003E_002Eget_Current();
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGetActiveRecords_003Ed__34(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			CompilerScopeManager<V> compilerScopeManager = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CcurrentRecord_003E5__2 = compilerScopeManager.lastRecord + 1;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			while (0 < --_003CcurrentRecord_003E5__2)
			{
				if (!compilerScopeManager.records[_003CcurrentRecord_003E5__2].IsNamespace || compilerScopeManager.LookupNamespace(compilerScopeManager.records[_003CcurrentRecord_003E5__2].ncName, compilerScopeManager.lastRecord, _003CcurrentRecord_003E5__2 + 1) == null)
				{
					_003C_003E2__current = compilerScopeManager.records[_003CcurrentRecord_003E5__2];
					_003C_003E1__state = 1;
					return true;
				}
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		private ScopeRecord System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002EXml_002EXsl_002EXslt_002ECompilerScopeManager_003CV_003E_002EScopeRecord_003E_002Eget_Current()
		{
			return _003C_003E2__current;
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<ScopeRecord> IEnumerable<ScopeRecord>.GetEnumerator()
		{
			_003CGetActiveRecords_003Ed__34 result;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				result = this;
			}
			else
			{
				result = new _003CGetActiveRecords_003Ed__34(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			return result;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<ScopeRecord>)this).GetEnumerator();
		}
	}

	private const int LastPredefRecord = 0;

	private ScopeRecord[] records = new ScopeRecord[32];

	private int lastRecord;

	private int lastScopes;

	public bool ForwardCompatibility
	{
		get
		{
			return (records[lastRecord].flags & ScopeFlags.ForwardCompatibility) != 0;
		}
		set
		{
			SetFlag(ScopeFlags.ForwardCompatibility, value);
		}
	}

	public bool BackwardCompatibility
	{
		get
		{
			return (records[lastRecord].flags & ScopeFlags.BackwardCompatibility) != 0;
		}
		set
		{
			SetFlag(ScopeFlags.BackwardCompatibility, value);
		}
	}

	public bool CanHaveApplyImports
	{
		get
		{
			return (records[lastRecord].flags & ScopeFlags.CanHaveApplyImports) != 0;
		}
		set
		{
			SetFlag(ScopeFlags.CanHaveApplyImports, value);
		}
	}

	public CompilerScopeManager()
	{
		records[0].flags = ScopeFlags.NsDecl;
		records[0].ncName = "xml";
		records[0].nsUri = "http://www.w3.org/XML/1998/namespace";
	}

	public CompilerScopeManager(KeywordsTable atoms)
	{
		records[0].flags = ScopeFlags.NsDecl;
		records[0].ncName = atoms.Xml;
		records[0].nsUri = atoms.UriXml;
	}

	public void EnterScope()
	{
		lastScopes++;
	}

	public void ExitScope()
	{
		if (0 < lastScopes)
		{
			lastScopes--;
			return;
		}
		while (records[--lastRecord].scopeCount == 0)
		{
		}
		lastScopes = records[lastRecord].scopeCount;
		lastScopes--;
	}

	[Conditional("DEBUG")]
	public void CheckEmpty()
	{
		ExitScope();
	}

	public bool EnterScope(NsDecl nsDecl)
	{
		lastScopes++;
		bool result = false;
		bool flag = false;
		while (nsDecl != null)
		{
			if (nsDecl.NsUri == null)
			{
				flag = true;
			}
			else if (nsDecl.Prefix == null)
			{
				AddExNamespace(nsDecl.NsUri);
			}
			else
			{
				result = true;
				AddNsDeclaration(nsDecl.Prefix, nsDecl.NsUri);
			}
			nsDecl = nsDecl.Prev;
		}
		if (flag)
		{
			AddExNamespace(null);
		}
		return result;
	}

	private void AddRecord()
	{
		records[lastRecord].scopeCount = lastScopes;
		if (++lastRecord == records.Length)
		{
			ScopeRecord[] destinationArray = new ScopeRecord[lastRecord * 2];
			Array.Copy(records, 0, destinationArray, 0, lastRecord);
			records = destinationArray;
		}
		lastScopes = 0;
	}

	private void AddRecord(ScopeFlags flag, string ncName, string uri, V value)
	{
		ScopeFlags scopeFlags = records[lastRecord].flags;
		if (lastScopes != 0 || (scopeFlags & ScopeFlags.ExclusiveFlags) != 0)
		{
			AddRecord();
			scopeFlags &= ScopeFlags.InheritedFlags;
		}
		records[lastRecord].flags = scopeFlags | flag;
		records[lastRecord].ncName = ncName;
		records[lastRecord].nsUri = uri;
		records[lastRecord].value = value;
	}

	private void SetFlag(ScopeFlags flag, bool value)
	{
		ScopeFlags scopeFlags = records[lastRecord].flags;
		if ((scopeFlags & flag) != 0 == value)
		{
			return;
		}
		if (lastScopes != 0)
		{
			AddRecord();
			scopeFlags &= ScopeFlags.InheritedFlags;
		}
		if (flag == ScopeFlags.CanHaveApplyImports)
		{
			scopeFlags ^= flag;
		}
		else
		{
			scopeFlags &= (ScopeFlags)(-4);
			if (value)
			{
				scopeFlags |= flag;
			}
		}
		records[lastRecord].flags = scopeFlags;
	}

	public void AddVariable(QilName varName, V value)
	{
		AddRecord(ScopeFlags.Variable, varName.LocalName, varName.NamespaceUri, value);
	}

	private string LookupNamespace(string prefix, int from, int to)
	{
		int num = from;
		while (to <= num)
		{
			if ((GetName(ref records[num], out var prefix2, out var nsUri) & ScopeFlags.NsDecl) != 0 && prefix2 == prefix)
			{
				return nsUri;
			}
			num--;
		}
		return null;
	}

	public string LookupNamespace(string prefix)
	{
		return LookupNamespace(prefix, lastRecord, 0);
	}

	private static ScopeFlags GetName(ref ScopeRecord re, out string prefix, out string nsUri)
	{
		prefix = re.ncName;
		nsUri = re.nsUri;
		return re.flags;
	}

	public void AddNsDeclaration(string prefix, string nsUri)
	{
		AddRecord(ScopeFlags.NsDecl, prefix, nsUri, default);
	}

	public void AddExNamespace(string nsUri)
	{
		AddRecord(ScopeFlags.NsExcl, null, nsUri, default);
	}

	public bool IsExNamespace(string nsUri)
	{
		int num = 0;
		int num2 = lastRecord;
		while (0 <= num2)
		{
			ScopeFlags name = GetName(ref records[num2], out var prefix, out var nsUri2);
			if ((name & ScopeFlags.NsExcl) != 0)
			{
				if (nsUri2 == nsUri)
				{
					return true;
				}
				if (nsUri2 == null)
				{
					num = num2;
				}
			}
			else if (num != 0 && (name & ScopeFlags.NsDecl) != 0 && nsUri2 == nsUri)
			{
				bool flag = false;
				for (int i = num2 + 1; i < num; i++)
				{
					GetName(ref records[i], out var prefix2, out var _);
					if ((name & ScopeFlags.NsDecl) != 0 && prefix2 == prefix)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			num2--;
		}
		return false;
	}

	private int SearchVariable(string localName, string uri)
	{
		int num = lastRecord;
		while (0 <= num)
		{
			if ((GetName(ref records[num], out var prefix, out var nsUri) & ScopeFlags.Variable) != 0 && prefix == localName && nsUri == uri)
			{
				return num;
			}
			num--;
		}
		return -1;
	}

	public V LookupVariable(string localName, string uri)
	{
		int num = SearchVariable(localName, uri);
		if (num >= 0)
		{
			return records[num].value;
		}
		return default;
	}

	public bool IsLocalVariable(string localName, string uri)
	{
		int num = SearchVariable(localName, uri);
		while (0 <= --num)
		{
			if (records[num].scopeCount != 0)
			{
				return true;
			}
		}
		return false;
	}

	[IteratorStateMachine(typeof(CompilerScopeManager<>._003CGetActiveRecords_003Ed__34))]
	internal IEnumerable<ScopeRecord> GetActiveRecords()
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CGetActiveRecords_003Ed__34(-2)
		{
			_003C_003E4__this = this
		};
	}

	public NamespaceEnumerator GetEnumerator()
	{
		return new NamespaceEnumerator(this);
	}
}
