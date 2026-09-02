using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGModdingMetaProtection
{
	internal bool addDisabled;

	internal bool deleteDisabled;

	internal bool editDisabled;

	public Dictionary<BGId, BGModdingRepoProtection.FieldSettingEnum> fields = new Dictionary<BGId, BGModdingRepoProtection.FieldSettingEnum>();

	public Dictionary<BGId, bool> rowsEdit = new Dictionary<BGId, bool>();

	public Dictionary<BGId, bool> rowsDelete = new Dictionary<BGId, bool>();

	public Dictionary<BGId, Dictionary<BGId, bool>> cells = new Dictionary<BGId, Dictionary<BGId, bool>>();

	public bool AddDisabled
	{
		get
		{
			return addDisabled;
		}
		set
		{
			if (addDisabled != value)
			{
				addDisabled = value;
				FireEvent();
			}
		}
	}

	public bool DeleteDisabled
	{
		get
		{
			return deleteDisabled;
		}
		set
		{
			if (deleteDisabled != value)
			{
				deleteDisabled = value;
				FireEvent();
			}
		}
	}

	public bool EditDisabled
	{
		get
		{
			return editDisabled;
		}
		set
		{
			if (editDisabled != value)
			{
				editDisabled = value;
				FireEvent();
			}
		}
	}

	internal Dictionary<BGId, BGModdingRepoProtection.FieldSettingEnum> Fields => fields;

	internal Dictionary<BGId, bool> RowsEdit => rowsEdit;

	internal Dictionary<BGId, bool> RowsDelete => rowsDelete;

	internal Dictionary<BGId, Dictionary<BGId, bool>> Cells => cells;

	public bool ProtectedAny
	{
		get
		{
			if (!AddDisabled && !DeleteDisabled)
			{
				return EditDisabled;
			}
			return true;
		}
	}

	public event Action Changed;

	public BGModdingMetaProtection Clone()
	{
		BGModdingMetaProtection bGModdingMetaProtection = new BGModdingMetaProtection
		{
			addDisabled = addDisabled,
			deleteDisabled = deleteDisabled,
			editDisabled = editDisabled,
			fields = new Dictionary<BGId, BGModdingRepoProtection.FieldSettingEnum>(Fields),
			rowsEdit = new Dictionary<BGId, bool>(RowsEdit),
			rowsDelete = new Dictionary<BGId, bool>(RowsDelete),
			cells = new Dictionary<BGId, Dictionary<BGId, bool>>()
		};
		foreach (KeyValuePair<BGId, Dictionary<BGId, bool>> cell in cells)
		{
			bGModdingMetaProtection.cells[cell.Key] = new Dictionary<BGId, bool>(cell.Value);
		}
		return bGModdingMetaProtection;
	}

	private void FireEvent()
	{
		Changed?.Invoke();
	}

	public bool HasFieldEdit(BGId fieldId)
	{
		return fields.ContainsKey(fieldId);
	}

	public void SetFieldEdit(BGId fieldId, BGModdingRepoProtection.FieldSettingEnum setting)
	{
		if (setting == BGModdingRepoProtection.FieldSettingEnum.Inherited)
		{
			if (!fields.TryGetValue(fieldId, out var value))
			{
				return;
			}
			if (value == setting)
			{
				fields.Remove(fieldId);
				return;
			}
			fields.Remove(fieldId);
		}
		else
		{
			if (fields.TryGetValue(fieldId, out var value2) && value2 == setting)
			{
				return;
			}
			fields[fieldId] = setting;
		}
		FireEvent();
	}

	public BGModdingRepoProtection.FieldSettingEnum GetFieldEdit(BGId fieldId)
	{
		if (!fields.TryGetValue(fieldId, out var value))
		{
			return BGModdingRepoProtection.FieldSettingEnum.Inherited;
		}
		return value;
	}

	public bool RemoveRowsEdit(BGId entityId)
	{
		bool flag = rowsEdit.Remove(entityId);
		if (flag)
		{
			FireEvent();
		}
		return flag;
	}

	public bool RemoveRowsDelete(BGId entityId)
	{
		bool flag = rowsDelete.Remove(entityId);
		if (flag)
		{
			FireEvent();
		}
		return flag;
	}

	public bool RemoveCellField(BGId fieldId)
	{
		bool flag = cells.Remove(fieldId);
		if (flag)
		{
			FireEvent();
		}
		return flag;
	}

	public bool? Get(BGId fieldId, BGId entityId)
	{
		if (!cells.TryGetValue(fieldId, out var value))
		{
			return null;
		}
		if (!value.TryGetValue(entityId, out var value2))
		{
			return null;
		}
		return value2;
	}

	public bool? GetRowDelete(BGId entityId)
	{
		if (!rowsDelete.TryGetValue(entityId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool? GetRowEdit(BGId entityId)
	{
		if (!rowsEdit.TryGetValue(entityId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool AddRowEditDisabled(BGId entityId, bool disabled)
	{
		if (rowsEdit.TryGetValue(entityId, out var value) && value == disabled)
		{
			return false;
		}
		rowsEdit[entityId] = disabled;
		FireEvent();
		return true;
	}

	public bool AddRowDeleteDisabled(BGId entityId, bool disabled)
	{
		if (rowsDelete.TryGetValue(entityId, out var value) && value == disabled)
		{
			return false;
		}
		rowsDelete[entityId] = disabled;
		FireEvent();
		return true;
	}

	public bool RemoveRowEdit(BGId entityId)
	{
		if (!rowsEdit.Remove(entityId))
		{
			return false;
		}
		FireEvent();
		return true;
	}

	public bool RemoveRowDelete(BGId entityId)
	{
		if (!rowsDelete.Remove(entityId))
		{
			return false;
		}
		FireEvent();
		return true;
	}
}
