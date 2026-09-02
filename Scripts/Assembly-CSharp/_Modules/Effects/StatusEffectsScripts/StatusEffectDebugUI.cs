using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Modules.Effects.StatusEffectsScripts;

public class StatusEffectDebugUI : MonoBehaviour
{
	private Dictionary<string, TMP_Text> _textDebugDict = new Dictionary<string, TMP_Text>();

	[SerializeField]
	private TMP_Text _textDebugPrefab;

	[SerializeField]
	private Transform _textParentTransform;

	public Dictionary<string, TMP_Text> TextDebugDict => _textDebugDict;

	public TMP_Text CreateTextDebug(string text, string key = "")
	{
		TMP_Text tMP_Text = Object.Instantiate(_textDebugPrefab, _textParentTransform);
		tMP_Text.text = text;
		if (!string.IsNullOrWhiteSpace(key))
		{
			_textDebugDict.Add(key, tMP_Text);
		}
		return tMP_Text;
	}
}
