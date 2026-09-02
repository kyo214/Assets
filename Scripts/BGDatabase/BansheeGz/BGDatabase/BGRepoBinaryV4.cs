using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRepoBinaryV4 : BGRepo.RepoReaderI, BGRepo.RepoWriterI
{
	private const int MyVersion = 4;

	private static readonly BGId UniqueId = new BGId(5208396773359933591uL, 14401663668042484142uL);

	public BGRepo Read(byte[] dataBytes)
	{
		BGBinaryReader binder = new BGBinaryReader(dataBytes);
		int binaryFormatVersion = binder.ReadInt();
		BGRepo repo = new BGRepo
		{
			BinaryFormatVersion = binaryFormatVersion
		};
		if (dataBytes.Length < 21)
		{
			return repo;
		}
		if (binder.ReadId() != UniqueId)
		{
			throw new Exception("Provided binary array is not a valid BGDatabase content!");
		}
		binder.ReadArray(() =>
		{
			repo.Addons.Add(BGAddon.FromBinary(binder));
		});
		BGAddonSettings bGAddonSettings = repo.Addons.Get<BGAddonSettings>();
		bool multithreaded = false;
		if (bGAddonSettings != null)
		{
			multithreaded = bGAddonSettings.MultiThreadedLoading;
			if (bGAddonSettings.ZippedContent)
			{
				using DeflateStream input = new DeflateStream(new MemoryStream(dataBytes, binder.Cursor, dataBytes.Length - binder.Cursor), CompressionMode.Decompress);
				using MemoryStream memoryStream = new MemoryStream();
				CopyTo(input, memoryStream);
				binder = new BGBinaryReader(memoryStream.ToArray());
			}
		}
		ReadMetas(binder, repo, multithreaded);
		return repo;
	}

	private static void ReadMetas(BGBinaryReader binder, BGRepo repo, bool multithreaded)
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			multithreaded = false;
		}
		BGMultiThreadedLoader multiThreadedLoader = null;
		if (multithreaded)
		{
			multiThreadedLoader = new BGMultiThreadedLoader();
		}
		binder.ReadArray(() =>
		{
			BGMetaEntity meta = BGMetaEntity.FromBinary(binder, repo);
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
				BGField field = BGField.FromBinary(binder, meta);
				int entitiesCount = meta.CountEntities;
				ArraySegment<byte> fieldValues = binder.ReadByteArray();
				if (fieldValues.Count > 0)
				{
					if (multithreaded)
					{
						multiThreadedLoader.AddAction(() =>
						{
							ReadFieldValues(fieldValues, field, entitiesCount, multiThreadedLoader.AddException);
						}, !field.SupportMultiThreadedLoading);
					}
					else
					{
						ReadFieldValues(fieldValues, field, entitiesCount, (Exception e) =>
						{
							Debug.Log("Can not read field value. Field " + field.FullName);
							Debug.LogException(e);
						});
					}
				}
			});
		});
		multiThreadedLoader?.Load();
	}

	private static void ReadFieldValues(ArraySegment<byte> fieldValues, BGField field, int entitiesCount, Action<Exception> onError)
	{
		BGMetaEntity meta = field.Meta;
		byte[] array = fieldValues.Array;
		int offset = fieldValues.Offset;
		int constantSize = field.ConstantSize;
		if (constantSize > 0)
		{
			for (int i = 0; i < entitiesCount; i++)
			{
				BGEntity bGEntity = meta[i];
				field.FromBytes(bGEntity.Index, new ArraySegment<byte>(array, offset + i * constantSize, constantSize));
			}
			return;
		}
		int num = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, offset, 4));
		if (num == 0)
		{
			return;
		}
		int num2 = offset + 4;
		int num3 = num2 + num * 8;
		int num4 = 0;
		if (field is BGFieldEntityName bGFieldEntityName)
		{
			for (int j = 0; j < num; j++)
			{
				int num5 = num2 + j * 8;
				int index = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, num5, 4));
				BGEntity bGEntity2 = meta[index];
				int num6 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, num5 + 4, 4));
				try
				{
					bGFieldEntityName.SetEntityValue(bGEntity2.Index, BGFieldStringA.ValueFromBytes(new ArraySegment<byte>(array, num3 + num4, num6 - num4)));
				}
				catch (Exception obj)
				{
					onError(obj);
				}
				num4 = num6;
			}
			return;
		}
		for (int k = 0; k < num; k++)
		{
			int num7 = num2 + k * 8;
			int index2 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, num7, 4));
			BGEntity bGEntity3 = meta[index2];
			int num8 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(array, num7 + 4, 4));
			try
			{
				field.FromBytes(bGEntity3.Index, new ArraySegment<byte>(array, num3 + num4, num8 - num4));
			}
			catch (Exception obj2)
			{
				onError(obj2);
			}
			num4 = num8;
		}
	}

	public static void CopyTo(Stream input, Stream output)
	{
		byte[] array = new byte[65536];
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, count);
		}
	}

	public byte[] Write(BGRepo repo)
	{
		BGBinaryWriter builder = new BGBinaryWriter();
		builder.AddInt(4);
		builder.AddId(UniqueId);
		builder.AddArray(() =>
		{
			repo.Addons.ForEachAddon((BGAddon addon) =>
			{
				BGAddon.ToBinary(builder, addon);
			});
		}, repo.Addons.Count);
		bool flag = repo.Addons.Get<BGAddonSettings>()?.ZippedContent ?? false;
		int count = builder.Count;
		builder.AddArray(() =>
		{
			repo.ForEachMeta((BGMetaEntity meta) =>
			{
				BGMetaEntity.ToBinary(builder, meta);
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
						BGField.ToBinary(builder, field);
						int constantSize = field.ConstantSize;
						if (constantSize > 0)
						{
							byte[] fieldValues = new byte[entityCount * constantSize];
							int cursor = 0;
							if (constantSize < 16)
							{
								meta.ForEachEntity((BGEntity entity) =>
								{
									byte[] array4 = field.ToBytes(entity.Index);
									for (int j = 0; j < constantSize; j++)
									{
										fieldValues[cursor++] = array4[j];
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
							BGRepoBinaryV1.FieldBuilder fieldBuilder = new BGRepoBinaryV1.FieldBuilder(builder);
							if (!field.EmptyContent)
							{
								field.ForEachValue((int index) =>
								{
									byte[] array4;
									try
									{
										array4 = field.ToBytes(index);
									}
									catch (Exception)
									{
										array4 = null;
									}
									if (array4 != null && array4.Length != 0)
									{
										fieldBuilder.Add(index, array4);
									}
								});
							}
							fieldBuilder.Finish();
						}
					});
				}, meta.CountFields);
			});
		}, repo.CountMeta);
		byte[] array = builder.ToArray();
		if (flag)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
			{
				deflateStream.Write(array, count, array.Length - count);
			}
			byte[] array2 = memoryStream.ToArray();
			byte[] array3 = new byte[count + array2.Length];
			Buffer.BlockCopy(array, 0, array3, 0, count);
			Buffer.BlockCopy(array2, 0, array3, count, array2.Length);
			array = array3;
		}
		return array;
	}
}
