using System;

namespace BansheeGz.BGDatabase;

public class BGCloneContextMeta
{
	public readonly BGRepo repo;

	public readonly Predicate<BGId> metaFilter;

	public readonly Predicate<BGField> fieldFilter;

	public readonly bool copyValues;

	public readonly Predicate<BGEntity> entityFilter;

	public Action<BGField> OnAfterFieldCreated;

	public BGCloneContextMeta(BGRepo repo, Predicate<BGId> metaFilter, Predicate<BGField> fieldFilter, bool copyValues, Predicate<BGEntity> entityFilter)
	{
		this.repo = repo;
		this.metaFilter = metaFilter;
		this.fieldFilter = fieldFilter;
		this.copyValues = copyValues;
		this.entityFilter = entityFilter;
	}
}
