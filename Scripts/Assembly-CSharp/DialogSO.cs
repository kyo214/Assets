using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "WMO/ScriptableObjects/Dialogue/Dialogue SO", order = 1)]
public class DialogSO : ScriptableObject
{
	[SerializeField]
	private List<ActionDialogue> _listActionDialogue = new List<ActionDialogue>();

	public List<ActionDialogue> ListActionDialogue => _listActionDialogue;
}
