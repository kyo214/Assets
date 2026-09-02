namespace System.Security.AccessControl;

[Flags]
public enum RegistryRights
{
	QueryValues = 1,
	SetValue = 2,
	CreateSubKey = 4,
	EnumerateSubKeys = 8,
	Notify = 0x10,
	CreateLink = 0x20,
	Delete = 0x10000,
	ReadPermissions = 0x20000,
	WriteKey = SetValue | CreateSubKey | ReadPermissions,
	ReadKey = QueryValues | EnumerateSubKeys | Notify | ReadPermissions,
	ExecuteKey = ReadKey,
	ChangePermissions = 0x40000,
	TakeOwnership = 0x80000,
	FullControl = ReadKey | SetValue | CreateSubKey | CreateLink | Delete | ChangePermissions | TakeOwnership
}
