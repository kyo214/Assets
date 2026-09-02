using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
	public TMP_Text TextPlayerName;

	public GameObject DialogueObject;

	public GameObject BotDialogueObject;

	public TMP_Text TextDialogue;

	public GameObject ProgressBarObject;

	public GameObject NormalBarObject;

	public GameObject HealBarObject;

	public Image RedBarBG;

	public DOTweenAnimation TweenTargetAnimation;

	public RectTransform PointerStitch;

	public List<RectTransform> listTargetStitch = new List<RectTransform>();

	public Image ProgressBarRadial;

	public Animator iconBarAnimator;

	public Transform BarProgressTransform;

	public GameObject ChargeMeleeProgressObject;

	public Image ChargeMeleeProgressRadial;

	public GameObject ProgressBarTutorialObject;

	public TMP_Text TextTutorialText;

	public Transform BarProgressTutorialTransform;

	public GameObject IconHostObject;

	public RectTransform rectTransform;

	public TMP_Text TextProgressMashButton;

	public GameObject Divide;

	public TMP_Text TextHealingValue;

	public TMP_Text TextHealingValue2;

	public TMP_Text TextHealingReviveValue;

	public GameObject SolvingPuzzleUI;
}
