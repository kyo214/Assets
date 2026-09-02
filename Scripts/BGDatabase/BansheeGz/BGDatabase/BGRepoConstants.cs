namespace BansheeGz.BGDatabase;

public class BGRepoConstants
{
	public class MetaConstants
	{
		public const string SheetName = "_meta";

		public const string FieldId = "_id";

		public const string FieldName = "_name";

		public const string FieldType = "_type";

		public const string FieldSystem = "_system";

		public const string FieldAddon = "_addon";

		public const string FieldUniqueName = "_uniqueName";

		public const string FieldEmptyName = "_emptyName";

		public const string FieldSingleton = "_singleton";

		public const string FieldComment = "_comment";

		public const string FieldConfig = "_config";

		public static string[] Fields => new string[9] { "_id", "_name", "_type", "_system", "_addon", "_uniqueName", "_emptyName", "_singleton", "_config" };
	}

	public class FieldConstants
	{
		public const string SheetName = "_field";

		public const string FieldId = "_id";

		public const string FieldName = "_name";

		public const string FieldMetaId = "_metaId";

		public const string FieldType = "_type";

		public const string FieldSystem = "_system";

		public const string FieldAddon = "_addon";

		public const string FieldDefaultValue = "_defaultValue";

		public const string FieldRequired = "_required";

		public const string FieldConfig = "_config";

		public static string[] Fields => new string[9] { "_id", "_name", "_type", "_system", "_addon", "_metaId", "_config", "_defaultValue", "_required" };
	}

	public class AddonConstants
	{
		public const string SheetName = "_addon";

		public const string FieldType = "_type";

		public const string FieldConfig = "_config";

		public static string[] Fields => new string[2] { "_type", "_config" };
	}
}
