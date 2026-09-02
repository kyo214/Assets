namespace BansheeGz.BGDatabase;

[Validator(Target = typeof(BGMetaNested))]
public class BGMetaEntityValidatorNested : BGMetaEntityValidator
{
	public override void Start(params BGValidationLog[] logs)
	{
		base.Start(logs);
		BGMetaNested bGMetaNested = (BGMetaNested)meta;
		BGFieldRelationSingle ownerRelation = bGMetaNested.OwnerRelation;
		if (ownerRelation == null)
		{
			BGValidator.Add(logs, "Nested Meta $ is broken- no owner relation", meta.Name);
		}
	}

	public override void Validate(BGEntity entity, params BGValidationLog[] logs)
	{
		base.Validate(entity, logs);
		BGMetaNested bGMetaNested = (BGMetaNested)meta;
		BGFieldRelationSingle ownerRelation = bGMetaNested.OwnerRelation;
		if (ownerRelation != null && ownerRelation[entity.Index] == null)
		{
			BGValidator.Add(logs, "Nested Entity [$] is missing owner relation", entity.FullName);
		}
	}
}
