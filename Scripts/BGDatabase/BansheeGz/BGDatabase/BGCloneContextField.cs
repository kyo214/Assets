using System;

namespace BansheeGz.BGDatabase;

public class BGCloneContextField
{
	public readonly BGMetaEntity meta;

	public readonly bool copyValues;

	public Action<BGField> OnAfterFieldCreated;

	public BGCloneContextField(BGMetaEntity meta, bool copyValues)
	{
		this.meta = meta;
		this.copyValues = copyValues;
	}
}
