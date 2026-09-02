using System.Collections;
using UnityEngine;

public interface IPuzzle
{
	void SetInteractableObject(ItemInteractable intObject);

	ItemInteractable GetInteractableObject();

	void SetPassword(string pass);

	void InitAnswer();

	void Show();

	void Hide();

	void Navigate(Vector2 direction);

	void Action1Press();

	void Action1Release();

	IEnumerator PuzzleUnlocked();
}
