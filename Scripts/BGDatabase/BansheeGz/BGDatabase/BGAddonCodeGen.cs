using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "CodeGen", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerCodeGen")]
public class BGAddonCodeGen : BGAddon
{
	[Serializable]
	private class JsonConfig
	{
		public string GeneratorClass;

		public string SourceFile;

		public string ClassesNamePrefix;

		public string FieldsNamePrefix;

		public string Package;

		public string EntitiesPrefix;

		public string ReferenceClassPostfix;

		public string ReferenceListClassPostfix;

		public string FieldReferenceClassPostfix;

		public bool ReadOnly;
	}

	public const string DefaultExtensionClassesGenerator = "BansheeGz.BGDatabase.Editor.BGExtensionClassesGenerator";

	private string generatorClass;

	private string sourceFile;

	private string classesNamePrefix;

	private string fieldsNamePrefix;

	private string package;

	private string entitiesPrefix;

	private string referenceClassPostfix;

	private string referenceListClassPostfix;

	private string fieldReferenceClassPostfix;

	private bool readOnly;

	public string GeneratorClass
	{
		get
		{
			return generatorClass;
		}
		set
		{
			if (!string.Equals(generatorClass, value))
			{
				generatorClass = value;
				FireChange();
			}
		}
	}

	public string SourceFile
	{
		get
		{
			return sourceFile;
		}
		set
		{
			if (!string.Equals(sourceFile, value))
			{
				sourceFile = value;
				FireChange();
			}
		}
	}

	public string ClassesNamePrefix
	{
		get
		{
			return classesNamePrefix;
		}
		set
		{
			if (!string.Equals(classesNamePrefix, value))
			{
				classesNamePrefix = value;
				FireChange();
			}
		}
	}

	public string FieldsNamePrefix
	{
		get
		{
			return fieldsNamePrefix;
		}
		set
		{
			if (!string.Equals(fieldsNamePrefix, value))
			{
				fieldsNamePrefix = value;
				FireChange();
			}
		}
	}

	public string Package
	{
		get
		{
			return package;
		}
		set
		{
			if (!string.Equals(package, value))
			{
				package = value;
				FireChange();
			}
		}
	}

	public string EntitiesPrefix
	{
		get
		{
			return entitiesPrefix;
		}
		set
		{
			if (!(entitiesPrefix == value))
			{
				entitiesPrefix = value;
				FireChange();
			}
		}
	}

	public string ReferenceClassPostfix
	{
		get
		{
			return referenceClassPostfix;
		}
		set
		{
			if (!string.Equals(referenceClassPostfix, value))
			{
				referenceClassPostfix = value;
				FireChange();
			}
		}
	}

	public string ReferenceListClassPostfix
	{
		get
		{
			return referenceListClassPostfix;
		}
		set
		{
			if (!string.Equals(referenceListClassPostfix, value))
			{
				referenceListClassPostfix = value;
				FireChange();
			}
		}
	}

	public string FieldReferenceClassPostfix
	{
		get
		{
			return fieldReferenceClassPostfix;
		}
		set
		{
			if (!string.Equals(fieldReferenceClassPostfix, value))
			{
				fieldReferenceClassPostfix = value;
				FireChange();
			}
		}
	}

	public bool ReadOnly
	{
		get
		{
			return readOnly;
		}
		set
		{
			if (readOnly != value)
			{
				readOnly = value;
				FireChange();
			}
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			GeneratorClass = generatorClass,
			SourceFile = sourceFile,
			ClassesNamePrefix = classesNamePrefix,
			FieldsNamePrefix = fieldsNamePrefix,
			Package = package,
			EntitiesPrefix = entitiesPrefix,
			ReferenceClassPostfix = referenceClassPostfix,
			ReferenceListClassPostfix = referenceListClassPostfix,
			FieldReferenceClassPostfix = fieldReferenceClassPostfix,
			ReadOnly = readOnly
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		generatorClass = jsonConfig.GeneratorClass;
		sourceFile = jsonConfig.SourceFile;
		classesNamePrefix = jsonConfig.ClassesNamePrefix;
		fieldsNamePrefix = jsonConfig.FieldsNamePrefix;
		package = jsonConfig.Package;
		entitiesPrefix = jsonConfig.EntitiesPrefix;
		referenceClassPostfix = jsonConfig.ReferenceClassPostfix;
		referenceListClassPostfix = jsonConfig.ReferenceListClassPostfix;
		fieldReferenceClassPostfix = jsonConfig.FieldReferenceClassPostfix;
		readOnly = jsonConfig.ReadOnly;
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(512);
		bGBinaryWriter.AddInt(4);
		bGBinaryWriter.AddString(generatorClass);
		bGBinaryWriter.AddString(sourceFile);
		bGBinaryWriter.AddString(classesNamePrefix);
		bGBinaryWriter.AddString(fieldsNamePrefix);
		bGBinaryWriter.AddString(package);
		bGBinaryWriter.AddString(entitiesPrefix);
		bGBinaryWriter.AddString(referenceClassPostfix);
		bGBinaryWriter.AddString(referenceListClassPostfix);
		bGBinaryWriter.AddString(fieldReferenceClassPostfix);
		bGBinaryWriter.AddBool(readOnly);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			generatorClass = bGBinaryReader.ReadString();
			sourceFile = bGBinaryReader.ReadString();
			classesNamePrefix = bGBinaryReader.ReadString();
			fieldsNamePrefix = bGBinaryReader.ReadString();
			package = bGBinaryReader.ReadString();
			entitiesPrefix = bGBinaryReader.ReadString();
			break;
		case 2:
			generatorClass = bGBinaryReader.ReadString();
			sourceFile = bGBinaryReader.ReadString();
			classesNamePrefix = bGBinaryReader.ReadString();
			fieldsNamePrefix = bGBinaryReader.ReadString();
			package = bGBinaryReader.ReadString();
			entitiesPrefix = bGBinaryReader.ReadString();
			referenceClassPostfix = bGBinaryReader.ReadString();
			referenceListClassPostfix = bGBinaryReader.ReadString();
			break;
		case 3:
			generatorClass = bGBinaryReader.ReadString();
			sourceFile = bGBinaryReader.ReadString();
			classesNamePrefix = bGBinaryReader.ReadString();
			fieldsNamePrefix = bGBinaryReader.ReadString();
			package = bGBinaryReader.ReadString();
			entitiesPrefix = bGBinaryReader.ReadString();
			referenceClassPostfix = bGBinaryReader.ReadString();
			referenceListClassPostfix = bGBinaryReader.ReadString();
			fieldReferenceClassPostfix = bGBinaryReader.ReadString();
			break;
		case 4:
			generatorClass = bGBinaryReader.ReadString();
			sourceFile = bGBinaryReader.ReadString();
			classesNamePrefix = bGBinaryReader.ReadString();
			fieldsNamePrefix = bGBinaryReader.ReadString();
			package = bGBinaryReader.ReadString();
			entitiesPrefix = bGBinaryReader.ReadString();
			referenceClassPostfix = bGBinaryReader.ReadString();
			referenceListClassPostfix = bGBinaryReader.ReadString();
			fieldReferenceClassPostfix = bGBinaryReader.ReadString();
			readOnly = bGBinaryReader.ReadBool();
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		return new BGAddonCodeGen
		{
			Repo = repo,
			generatorClass = generatorClass,
			sourceFile = sourceFile,
			classesNamePrefix = classesNamePrefix,
			fieldsNamePrefix = fieldsNamePrefix,
			package = package,
			entitiesPrefix = entitiesPrefix,
			referenceClassPostfix = referenceClassPostfix,
			referenceListClassPostfix = referenceListClassPostfix,
			fieldReferenceClassPostfix = fieldReferenceClassPostfix,
			readOnly = readOnly
		};
	}

	public string GetMetaTypeWithPackage(string metaName)
	{
		return (string.IsNullOrEmpty(package) ? "" : (package + ".")) + GetMetaType(metaName);
	}

	public string GetEntityFactoryTypeWithPackage(string metaName)
	{
		return GetMetaTypeWithPackage(metaName) + "+Factory";
	}

	public string GetFieldName(string fieldName)
	{
		return fieldsNamePrefix + fieldName;
	}

	public string GetMetaType(string metaName)
	{
		return classesNamePrefix + metaName;
	}
}
