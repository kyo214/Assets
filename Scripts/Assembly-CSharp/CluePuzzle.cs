using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CluePuzzle : MonoBehaviour
{
	public TextMeshProUGUI[] _textObject;

	public Image[] _imageObject;

	public List<ValueClue> ArrValue = new List<ValueClue>();

	[SerializeField]
	private int _idxPage;

	[Header("-- PAGING -- ")]
	[SerializeField]
	private InputActionReference _moveButtonPress;

	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private List<GameObject> ListPage = new List<GameObject>();

	[SerializeField]
	private bool isPressing;

	private void Start()
	{
		_idxPage = 0;
	}

	private void OnEnable()
	{
		if (ListPage.Count > 0)
		{
			_moveButtonPress.action.started += OnMoveButtonPress;
		}
	}

	private void OnMoveButtonPress(InputAction.CallbackContext obj)
	{
		if (!_canvas || !_canvas.enabled || ListPage.Count <= 0)
		{
			return;
		}
		Vector2 vector = obj.ReadValue<Vector2>();
		if (!obj.started)
		{
			return;
		}
		if (vector.x < 0f)
		{
			_idxPage--;
		}
		else if (vector.x > 0f)
		{
			_idxPage++;
		}
		if (_idxPage < 0)
		{
			_idxPage = 0;
		}
		else if (_idxPage >= ListPage.Count)
		{
			_idxPage = ListPage.Count - 1;
		}
		foreach (GameObject item in ListPage)
		{
			item.SetActive(value: false);
		}
		ListPage[_idxPage].SetActive(value: true);
	}

	private void OnDisable()
	{
		if (ListPage.Count > 0)
		{
			_moveButtonPress.action.started -= OnMoveButtonPress;
		}
	}

	public void Show()
	{
		if (!(NetworkGameManager.Instance.ownPlayer.itemCollision != null) || !NetworkGameManager.Instance.ownPlayer.itemCollision.TryGetComponent<ItemInteractable>(out var component))
		{
			return;
		}
		for (int i = 0; i < _textObject.Length; i++)
		{
			if (component.ClueID != -1 && component.ClueID < ArrValue.Count)
			{
				_textObject[i].text = ArrValue[component.ClueID].ValueText[i];
			}
		}
	}
}
