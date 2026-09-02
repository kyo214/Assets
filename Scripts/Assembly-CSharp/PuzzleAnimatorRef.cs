using UnityEngine;

public class PuzzleAnimatorRef : MonoBehaviour
{
	public IPuzzle Puzzle;

	public GameObject _puzzleObject;

	private void Awake()
	{
		Puzzle = _puzzleObject.GetComponent(typeof(IPuzzle)) as IPuzzle;
	}
}
