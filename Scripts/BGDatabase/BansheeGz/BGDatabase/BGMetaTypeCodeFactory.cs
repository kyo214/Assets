using System;

namespace BansheeGz.BGDatabase;

public class BGMetaTypeCodeFactory : BGMetaTypeCodeFactory.BGMetaTypeCodeFactoryI
{
	public interface BGMetaTypeCodeFactoryI
	{
		BGMetaEntity Create(BGRepo repo, ushort typeCode, BGId id, string name);
	}

	public static readonly BGMetaTypeCodeFactory Instance = new BGMetaTypeCodeFactory();

	private BGMetaTypeCodeFactory()
	{
	}

	public BGMetaEntity Create(BGRepo repo, ushort typeCode, BGId id, string name, ArraySegment<byte> config, bool system, string addon, bool nameUnique, bool singleton, bool nameEmpty)
	{
		BGMetaEntity bGMetaEntity = Create(repo, typeCode, id, name);
		if (bGMetaEntity == null)
		{
			throw new Exception($"Can not create a meta: unsupported meta type code={typeCode}!");
		}
		bGMetaEntity.System = system;
		bGMetaEntity.UniqueName = nameUnique;
		bGMetaEntity.Singleton = singleton;
		bGMetaEntity.EmptyName = nameEmpty;
		bGMetaEntity.Addon = addon;
		bGMetaEntity.ConfigFromBytes(config);
		return bGMetaEntity;
	}

	public BGMetaEntity Create(BGRepo repo, ushort typeCode, BGId id, string name)
	{
		return typeCode switch
		{
			1 => new BGMetaRow(repo, id, name), 
			2 => new BGMetaNested(repo, id, name), 
			_ => BGLocalizationUglyHacks.LocalizationMetaFactory?.Create(repo, typeCode, id, name), 
		};
	}
}
