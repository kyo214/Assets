using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRepoBinaryV3 : BGRepo.RepoReaderI, BGRepo.RepoWriterI
{
	private class ActionsListRunner
	{
		private readonly List<Action> actions = new List<Action>();

		private List<Exception> errorsList;

		public bool HasActions => actions.Count > 0;

		public void AddException(Exception e)
		{
			errorsList = errorsList ?? new List<Exception>();
			errorsList.Add(e);
		}

		public void AddAction(Action action)
		{
			actions.Add(action);
		}

		public void Go()
		{
			for (int i = 0; i < actions.Count; i++)
			{
				actions[i]();
			}
		}

		public void PrintExceptions()
		{
			if (errorsList != null)
			{
				for (int i = 0; i < errorsList.Count; i++)
				{
					Debug.LogException(errorsList[i]);
				}
			}
		}
	}

	private const int MyVersion = 3;

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
			ArraySegment<byte> config = binder.ReadByteArray();
			repo.Addons.Add(BGAddon.Create(type, config));
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
		ActionsListRunner[] loaders = null;
		int currentLoader = 0;
		if (multithreaded)
		{
			int num = Mathf.Clamp(Environment.ProcessorCount, 1, 16);
			loaders = new ActionsListRunner[num];
			for (int i = 0; i < num; i++)
			{
				loaders[i] = new ActionsListRunner();
			}
		}
		_003C_003Ec__DisplayClass2_0 CS_0024_003C_003E8__locals1;
		binder.ReadArray(() =>
		{
			BGId id = binder.ReadId();
			string name = binder.ReadString();
			string type = binder.ReadString();
			ArraySegment<byte> config = binder.ReadByteArray();
			bool system = binder.ReadBool();
			string addon = binder.ReadString();
			bool uniqueName = binder.ReadBool();
			bool singleton = binder.ReadBool();
			bool emptyName = binder.ReadBool();
			BGMetaEntity meta = BGMetaEntity.Create(repo, type, id, name, config, system, addon, uniqueName, singleton, emptyName);
			ArraySegment<byte> arraySegment = binder.ReadByteArray();
			int num4 = ((arraySegment.Count != 0) ? (arraySegment.Count / 16) : 0);
			if (num4 > 0)
			{
				meta.EntitiesCapacity = num4;
				byte[] array2 = arraySegment.Array;
				int offset = arraySegment.Offset;
				int num5 = offset + num4 * 16;
				for (int j = offset; j < num5; j += 16)
				{
					meta.NewEntity(new BGId(array2, j));
				}
			}
			binder.ReadArray(() =>
			{
				BGId id2 = binder.ReadId();
				string name2 = binder.ReadString();
				string type2 = binder.ReadString();
				ArraySegment<byte> config2 = binder.ReadByteArray();
				bool system2 = binder.ReadBool();
				string addon2 = binder.ReadString();
				string defaultValue = binder.ReadString();
				bool required = binder.ReadBool();
				string customStringFormatterTypeAsString = binder.ReadString();
				string customEditorTypeAsString = binder.ReadString();
				BGField field = BGField.Create(meta, type2, id2, name2, config2, system2, addon2, defaultValue, required);
				field.CustomStringFormatterTypeAsString = customStringFormatterTypeAsString;
				field.CustomEditorTypeAsString = customEditorTypeAsString;
				int entitiesCount = meta.CountEntities;
				ArraySegment<byte> fieldValues = binder.ReadByteArray();
				if (fieldValues.Count > 0)
				{
					if (multithreaded)
					{
						((ActionsListRunner)(object)CS_0024_003C_003E8__locals1).AddAction((Action)(() =>
						{
							ReadFieldValues(fieldValues, field, entitiesCount, ((ActionsListRunner)(object)CS_0024_003C_003E8__locals1).AddException);
						}));
						int num6 = currentLoader;
						currentLoader = num6 + 1;
						if (currentLoader == loaders.Length)
						{
							currentLoader = 0;
						}
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
		if (!multithreaded)
		{
			return;
		}
		Thread[] array = new Thread[loaders.Length];
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			ActionsListRunner actionsListRunner = loaders[num2];
			if (actionsListRunner.HasActions)
			{
				Thread thread = new Thread(actionsListRunner.Go);
				thread.Start();
				array[num2] = thread;
			}
		}
		for (int num3 = 0; num3 < array.Length; num3++)
		{
			Thread thread2 = array[num3];
			if (thread2 != null)
			{
				thread2.Join();
				loaders[num3].PrintExceptions();
			}
		}
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
		builder.AddInt(3);
		builder.AddArray(() =>
		{
			repo.Addons.ForEachAddon((BGAddon addon) =>
			{
				builder.AddString(addon.GetType().AssemblyQualifiedName);
				builder.AddByteArray(addon.ConfigToBytes());
			});
		}, repo.Addons.Count);
		bool flag = repo.Addons.Get<BGAddonSettings>()?.ZippedContent ?? false;
		int count = builder.Count;
		builder.AddArray(() =>
		{
			repo.ForEachMeta((BGMetaEntity meta) =>
			{
				builder.AddId(meta.Id);
				builder.AddString(meta.Name);
				builder.AddString(meta.GetType().AssemblyQualifiedName);
				builder.AddByteArray(meta.ConfigToBytes());
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
						builder.AddString(field.GetType().AssemblyQualifiedName);
						builder.AddByteArray(field.ConfigToBytes());
						builder.AddBool(field.System);
						builder.AddString(field.Addon);
						builder.AddString(field.DefaultValue);
						builder.AddBool(field.Required);
						builder.AddString(field.CustomStringFormatterTypeAsString);
						builder.AddString(field.CustomEditorTypeAsString);
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
