using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct CachedAssetBundle(string name, Hash128 hash)
{
	private string m_Name = name;

	private Hash128 m_Hash = hash;

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public Hash128 hash
	{
		get
		{
			return m_Hash;
		}
		set
		{
			m_Hash = value;
		}
	}
}
