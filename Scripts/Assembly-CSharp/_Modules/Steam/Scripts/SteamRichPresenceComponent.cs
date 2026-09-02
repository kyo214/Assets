using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Modules.Steam.Scripts;

public class SteamRichPresenceComponent : MonoBehaviour
{
	private enum RichPresenceType
	{
		Text = 0,
		TextCustom = 1,
		TextWithValue = 2,
		Clear = 3
	}

	[SerializeField]
	private bool _runOnStart = true;

	[SerializeField]
	private RichPresenceType _type;

	[SerializeField]
	private string _id;

	[SerializeField]
	private string _text;

	[SerializeField]
	private List<SteamRichPresence.RichPresenceValueVariable> _valueTextCustom;

	private void Start()
	{
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			if (_runOnStart)
			{
				UpdateRichPresence();
			}
		}).Forget();
	}

	public void UpdateRichPresence()
	{
		UpdateRichPresence(_text);
	}

	public void UpdateRichPresence(string valueText)
	{
		_text = valueText;
		switch (_type)
		{
		case RichPresenceType.Text:
			SteamRichPresence.SetRichPresence(_text);
			break;
		case RichPresenceType.TextCustom:
			SteamRichPresence.SetRichPresenceCustom(_id);
			break;
		case RichPresenceType.TextWithValue:
			SteamRichPresence.SetRichPresenceVariableValue(_valueTextCustom, _id);
			break;
		default:
			SteamRichPresence.ClearRichPresence();
			break;
		}
	}

	public void SetRichPresenceID(string idText)
	{
		_id = idText;
	}

	public void SetValueVariable(string id, string valueText)
	{
		foreach (SteamRichPresence.RichPresenceValueVariable item in _valueTextCustom)
		{
			if (item.id == id)
			{
				item.value = valueText;
				break;
			}
		}
	}
}
