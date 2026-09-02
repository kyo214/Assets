using System;

namespace BansheeGz.BGDatabase;

public class BGRepoDelta
{
	private class DeltaBinary
	{
		public void Save(BGRepoDelta delta, BGBinaryWriter builder)
		{
			builder.AddInt(1);
			builder.AddId(UniqueId);
			delta.added.ToBinary(builder);
			delta.updated.ToBinary(builder);
			delta.deleted.ToBinary(builder);
		}

		public void Load(BGRepoDelta delta, BGBinaryReader reader)
		{
			if (reader.Length < 4)
			{
				return;
			}
			int num = (delta.BinaryFormatVersion = reader.ReadInt());
			if (reader.Length >= 20)
			{
				if (reader.ReadId() != UniqueId)
				{
					throw new Exception("Provided binary array is not a valid BGDatabase delta content!");
				}
				if (num != 1)
				{
					throw new BGException("Can not read repo delta from binary array: unsupported version $", num);
				}
				delta.added.FromBinary(reader);
				delta.updated.FromBinary(reader);
				delta.deleted.FromBinary(reader);
			}
		}
	}

	private readonly BGRepoDeltaAdded added = new BGRepoDeltaAdded();

	private readonly BGRepoDeltaUpdated updated = new BGRepoDeltaUpdated();

	private readonly BGRepoDeltaDeleted deleted = new BGRepoDeltaDeleted();

	public const int LastVersion = 1;

	private static readonly BGId UniqueId = new BGId(5702804340146962847L, 3523676889787481529L);

	public int BinaryFormatVersion { get; set; }

	public BGRepoDelta()
	{
	}

	public BGRepoDelta(byte[] data)
	{
		Load(data);
	}

	public static BGRepoDelta Create(BGRepo baseRepo, BGRepo targetRepo)
	{
		BGRepoDelta bGRepoDelta = new BGRepoDelta();
		bGRepoDelta.added.Match(baseRepo, targetRepo);
		bGRepoDelta.updated.Match(baseRepo, targetRepo);
		bGRepoDelta.deleted.Match(baseRepo, targetRepo);
		return bGRepoDelta;
	}

	public void ApplyTo(BGRepo repo, BGModdingRepoProtection repoProtection)
	{
		added.ApplyTo(repo, repoProtection);
		updated.ApplyTo(repo, repoProtection);
		deleted.ApplyTo(repo, repoProtection);
		repo.Events.FireAnyChange();
	}

	public byte[] Save()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter();
		Save(bGBinaryWriter);
		return bGBinaryWriter.ToArray();
	}

	public void Save(BGBinaryWriter builder)
	{
		new DeltaBinary().Save(this, builder);
	}

	public static BGRepoDelta LoadStatic(byte[] data)
	{
		BGRepoDelta bGRepoDelta = new BGRepoDelta();
		bGRepoDelta.Load(data);
		return bGRepoDelta;
	}

	public void Load(byte[] data)
	{
		Load(new BGBinaryReader(data));
	}

	public void Load(BGBinaryReader reader)
	{
		new DeltaBinary().Load(this, reader);
	}
}
