using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRepoBinaryV1 : BGRepo.RepoReaderI, BGRepo.RepoWriterI
{
	internal class FieldBuilder
	{
		private byte[] fieldValues = new byte[64];

		private byte[] indexesValues = new byte[64];

		private readonly BGBinaryWriter builder;

		private int count;

		private int cursor;

		private int fieldSize;

		private int indexesSize;

		public FieldBuilder(BGBinaryWriter builder)
		{
			this.builder = builder;
		}

		public void Add(int entityIndex, byte[] fieldValue)
		{
			int num = indexesSize;
			indexesSize += 8;
			if (indexesValues.Length < indexesSize)
			{
				int num2 = indexesValues.Length * 2;
				if (num2 < indexesSize)
				{
					num2 = indexesSize;
				}
				byte[] dst = new byte[num2];
				Buffer.BlockCopy(indexesValues, 0, dst, 0, num);
				indexesValues = dst;
			}
			int num3 = fieldValue.Length;
			int num4 = fieldSize;
			fieldSize += num3;
			if (fieldValues.Length < fieldSize)
			{
				int num5 = fieldValues.Length * 2;
				if (num5 < fieldSize)
				{
					num5 = fieldSize;
				}
				byte[] dst2 = new byte[num5];
				Buffer.BlockCopy(fieldValues, 0, dst2, 0, num4);
				fieldValues = dst2;
			}
			count++;
			byte[] array = BGFieldInt.ValueToBytes(entityIndex);
			indexesValues[num] = array[0];
			indexesValues[num + 1] = array[1];
			indexesValues[num + 2] = array[2];
			indexesValues[num + 3] = array[3];
			int value = cursor + num3;
			byte[] array2 = BGFieldInt.ValueToBytes(value);
			indexesValues[num + 4] = array2[0];
			indexesValues[num + 5] = array2[1];
			indexesValues[num + 6] = array2[2];
			indexesValues[num + 7] = array2[3];
			cursor = value;
			if (num3 < 16)
			{
				for (int i = 0; i < num3; i++)
				{
					fieldValues[num4 + i] = fieldValue[i];
				}
			}
			else
			{
				Buffer.BlockCopy(fieldValue, 0, fieldValues, num4, num3);
			}
		}

		public void Finish()
		{
			builder.AddInt(4 + indexesSize + fieldSize);
			builder.AddInt(count);
			if (indexesSize > 0)
			{
				byte[] array = new byte[indexesSize];
				Buffer.BlockCopy(indexesValues, 0, array, 0, indexesSize);
				builder.AddBytesRaw(array);
			}
			if (fieldSize > 0)
			{
				byte[] array2 = new byte[fieldSize];
				Buffer.BlockCopy(fieldValues, 0, array2, 0, fieldSize);
				builder.AddBytesRaw(array2);
			}
		}
	}

	private const int MyVersion = 1;

	public BGRepo Read(byte[] dataBytes)
	{
		BGBinaryReader binder = new BGBinaryReader(dataBytes);
		int binaryFormatVersion = binder.ReadInt();
		BGRepo repo = new BGRepo
		{
			BinaryFormatVersion = binaryFormatVersion
		};
		binder.ReadArray(() =>
		{
			string type = binder.ReadString();
			string config = binder.ReadString();
			repo.Addons.Add(BGAddon.Create(type, config));
		});
		ReadMetas(binder, repo);
		return repo;
	}

	private static void ReadMetas(BGBinaryReader binder, BGRepo repo)
	{
		binder.ReadArray(() =>
		{
			BGId id = binder.ReadId();
			string name = binder.ReadString();
			string type = binder.ReadString();
			string config = binder.ReadString();
			bool system = binder.ReadBool();
			string addon = binder.ReadString();
			bool uniqueName = binder.ReadBool();
			bool singleton = binder.ReadBool();
			bool emptyName = binder.ReadBool();
			BGMetaEntity meta = BGMetaEntity.Create(repo, type, id, name, config, system, addon, uniqueName, singleton, emptyName);
			ArraySegment<byte> arraySegment = binder.ReadByteArray();
			int num = ((arraySegment.Count != 0) ? (arraySegment.Count / 16) : 0);
			if (num > 0)
			{
				meta.EntitiesCapacity = num;
				byte[] array = arraySegment.Array;
				int offset = arraySegment.Offset;
				int num2 = offset + num * 16;
				for (int i = offset; i < num2; i += 16)
				{
					meta.NewEntity(new BGId(array, i));
				}
			}
			binder.ReadArray(() =>
			{
				BGId id2 = binder.ReadId();
				string name2 = binder.ReadString();
				string type2 = binder.ReadString();
				string config2 = binder.ReadString();
				bool system2 = binder.ReadBool();
				string addon2 = binder.ReadString();
				string defaultValue = binder.ReadString();
				bool required = binder.ReadBool();
				BGField bGField = BGField.Create(meta, type2, id2, name2, config2, system2, addon2, defaultValue, required);
				int countEntities = meta.CountEntities;
				ArraySegment<byte> arraySegment2 = binder.ReadByteArray();
				if (arraySegment2.Count > 0)
				{
					byte[] array2 = arraySegment2.Array;
					int offset2 = arraySegment2.Offset;
					int constantSize = bGField.ConstantSize;
					if (constantSize > 0)
					{
						for (int j = 0; j < countEntities; j++)
						{
							BGEntity bGEntity = meta[j];
							bGField.FromBytes(bGEntity.Index, new ArraySegment<byte>(array2, offset2 + j * constantSize, constantSize));
						}
					}
					else
					{
						int num3 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array2, offset2, 4));
						if (num3 != 0)
						{
							int num4 = offset2 + 4;
							int num5 = num4 + num3 * 8;
							int num6 = 0;
							if (bGField is BGFieldEntityName bGFieldEntityName)
							{
								for (int k = 0; k < num3; k++)
								{
									int num7 = num4 + k * 8;
									int index = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array2, num7, 4));
									BGEntity bGEntity2 = meta[index];
									int num8 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array2, num7 + 4, 4));
									try
									{
										bGFieldEntityName.SetEntityValue(bGEntity2.Index, BGFieldStringA.ValueFromBytes(new ArraySegment<byte>(array2, num5 + num6, num8 - num6)));
									}
									catch (Exception exception)
									{
										Debug.Log("Can not read field value. Field " + bGFieldEntityName.FullName);
										Debug.LogException(exception);
									}
									num6 = num8;
								}
							}
							else
							{
								for (int l = 0; l < num3; l++)
								{
									int num9 = num4 + l * 8;
									int index2 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array2, num9, 4));
									BGEntity bGEntity3 = meta[index2];
									int num10 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array2, num9 + 4, 4));
									try
									{
										bGField.FromBytes(bGEntity3.Index, new ArraySegment<byte>(array2, num5 + num6, num10 - num6));
									}
									catch (Exception exception2)
									{
										Debug.Log("Can not read field value. Field " + bGField.FullName);
										Debug.LogException(exception2);
									}
									num6 = num10;
								}
							}
						}
					}
				}
			});
		});
	}

	public byte[] Write(BGRepo repo)
	{
		BGBinaryWriter builder = new BGBinaryWriter();
		builder.AddInt(1);
		builder.AddArray(() =>
		{
			repo.Addons.ForEachAddon((BGAddon addon) =>
			{
				builder.AddString(addon.GetType().FullName);
				builder.AddString(addon.ConfigToString());
			});
		}, repo.Addons.Count);
		builder.AddArray(() =>
		{
			repo.ForEachMeta((BGMetaEntity meta) =>
			{
				builder.AddId(meta.Id);
				builder.AddString(meta.Name);
				builder.AddString(meta.GetType().FullName);
				builder.AddString(meta.ConfigToString());
				builder.AddBool(meta.System);
				builder.AddString(meta.Addon);
				builder.AddBool(meta.UniqueName);
				builder.AddBool(meta.Singleton);
				builder.AddBool(meta.EmptyName);
				int entityCount = meta.CountEntities;
				byte[] entityIds = new byte[entityCount * 16];
				int i = 0;
				meta.ForEachEntity((BGEntity entity) =>
				{
					entity.Id.ToByteArray(entityIds, i << 4);
					i++;
				});
				builder.AddByteArray(entityIds);
				builder.AddArray(() =>
				{
					meta.ForEachField((BGField field) =>
					{
						builder.AddId(field.Id);
						builder.AddString(field.Name);
						builder.AddString(field.GetType().FullName);
						builder.AddString(field.ConfigToString());
						builder.AddBool(field.System);
						builder.AddString(field.Addon);
						builder.AddString(field.DefaultValue);
						builder.AddBool(field.Required);
						int constantSize = field.ConstantSize;
						if (constantSize > 0)
						{
							byte[] fieldValues = new byte[entityCount * constantSize];
							int cursor = 0;
							if (constantSize < 16)
							{
								meta.ForEachEntity((BGEntity entity) =>
								{
									byte[] array = field.ToBytes(entity.Index);
									for (int j = 0; j < constantSize; j++)
									{
										fieldValues[cursor++] = array[j];
									}
								});
							}
							else
							{
								meta.ForEachEntity((BGEntity entity) =>
								{
									Buffer.BlockCopy(field.ToBytes(entity.Index), 0, fieldValues, cursor, constantSize);
									cursor += constantSize;
								});
							}
							builder.AddByteArray(fieldValues);
						}
						else
						{
							FieldBuilder fieldBuilder = new FieldBuilder(builder);
							if (!field.EmptyContent)
							{
								field.ForEachValue((int index) =>
								{
									byte[] array;
									try
									{
										array = field.ToBytes(index);
									}
									catch (Exception)
									{
										array = null;
									}
									if (array != null && array.Length != 0)
									{
										fieldBuilder.Add(index, array);
									}
								});
							}
							fieldBuilder.Finish();
						}
					});
				}, meta.CountFields);
			});
		}, repo.CountMeta);
		return builder.ToArray();
	}
}
