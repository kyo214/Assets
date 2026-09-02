using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRepoBinaryV6 : BGRepo.RepoReaderI, BGRepo.RepoWriterI
{
	private const int MyVersion = 6;

	private static readonly BGId UniqueId = new BGId(5208396773359933591uL, 14401663668042484142uL);

	private static readonly BGId EncryptionId = new BGId(4770294628005998460uL, 9299804957405062829uL);

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
		if (binder.Cursor + 16 < binder.Length)
		{
			if (binder.ReadId() == EncryptionId)
			{
				string text = binder.ReadString();
				string config = binder.ReadString();
				if (string.IsNullOrEmpty(text))
				{
					throw new BGException("BGDatabase content is encrypted, but encryptor is not set!");
				}
				Type type = BGUtil.GetType(text);
				if (type == null)
				{
					throw new BGException("BGDatabase content is encrypted, can not load encryptor class $!", text);
				}
				if (!(Activator.CreateInstance(type) is BGEncryptor bGEncryptor))
				{
					throw new BGException("BGDatabase content is encrypted, can not create encryptor $- it does not implement BGEncryptor interface!", text);
				}
				ArraySegment<byte> array = bGEncryptor.Decrypt(new ArraySegment<byte>(dataBytes, binder.Cursor, binder.Length - binder.Cursor), config);
				binder = new BGBinaryReader(array);
			}
			else
			{
				binder.ShiftCursor(-16);
			}
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
			binder.ReadArray(() =>
			{
				BGKey bGKey = BGKey.FromBinary(binder, meta);
			});
			binder.ReadArray(() =>
			{
				BGIndex bGIndex = BGIndex.FromBinary(binder, meta);
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
		builder.AddInt(6);
		builder.AddId(UniqueId);
		int count = builder.Count;
		BGAddonSettings bGAddonSettings = repo.Addons.Get<BGAddonSettings>();
		builder.AddArray(() =>
		{
			repo.Addons.ForEachAddon((BGAddon addon) =>
			{
				BGAddon.ToBinary(builder, addon);
			});
		}, repo.Addons.Count);
		bool flag = bGAddonSettings?.ZippedContent ?? false;
		int count2 = builder.Count;
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
									byte[] array7 = field.ToBytes(entity.Index);
									for (int j = 0; j < constantSize; j++)
									{
										fieldValues[cursor++] = array7[j];
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
									byte[] array7;
									try
									{
										array7 = field.ToBytes(index);
									}
									catch (Exception)
									{
										array7 = null;
									}
									if (array7 != null && array7.Length != 0)
									{
										fieldBuilder.Add(index, array7);
									}
								});
							}
							fieldBuilder.Finish();
						}
					});
				}, meta.CountFields);
				builder.AddArray(() =>
				{
					meta.ForEachKey((BGKey key) =>
					{
						BGKey.ToBinary(builder, key);
					});
				}, meta.CountKeys);
				builder.AddArray(() =>
				{
					meta.ForEachIndex((BGIndex index) =>
					{
						BGIndex.ToBinary(builder, index);
					});
				}, meta.CountIndexes);
			});
		}, repo.CountMeta);
		byte[] array = builder.ToArray();
		if (flag)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
			{
				deflateStream.Write(array, count2, array.Length - count2);
			}
			byte[] array2 = memoryStream.ToArray();
			byte[] array3 = new byte[count2 + array2.Length];
			Buffer.BlockCopy(array, 0, array3, 0, count2);
			Buffer.BlockCopy(array2, 0, array3, count2, array2.Length);
			array = array3;
		}
		if (bGAddonSettings != null && bGAddonSettings.Encryptor != null)
		{
			byte[] array4 = new byte[count];
			Buffer.BlockCopy(array, 0, array4, 0, count);
			BGEncryptor encryptor = bGAddonSettings.Encryptor;
			ArraySegment<byte> arraySegment = encryptor.Encrypt(new ArraySegment<byte>(array, array4.Length, array.Length - array4.Length), bGAddonSettings.EncryptorConfig);
			BGBinaryWriter bGBinaryWriter = new BGBinaryWriter();
			bGBinaryWriter.AddId(EncryptionId);
			bGBinaryWriter.AddString(bGAddonSettings.EncryptorType);
			bGBinaryWriter.AddString(bGAddonSettings.EncryptorConfig);
			byte[] array5 = bGBinaryWriter.ToArray();
			byte[] array6 = new byte[array4.Length + array5.Length + arraySegment.Count];
			Buffer.BlockCopy(array4, 0, array6, 0, array4.Length);
			Buffer.BlockCopy(array5, 0, array6, count, array5.Length);
			Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, array6, count + array5.Length, arraySegment.Count);
			array = array6;
		}
		return array;
	}
}
