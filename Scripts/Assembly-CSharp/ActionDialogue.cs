using System;
using _Modules.DialogueSystem;

[Serializable]
public class ActionDialogue
{
	public ActionDialogueType ActionType;

	public bool HideDialogueBox;

	public CharDialogueEnum CharName;

	public string TermTextDialogue;

	public int PlayerDirection;

	public float Delay;

	public bool IsDelayTrigger;

	public bool CantSkip;

	public DialogueCustomActionBase CustomAction;
}
