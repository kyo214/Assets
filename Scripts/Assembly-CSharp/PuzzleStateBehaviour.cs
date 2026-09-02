using UnityEngine;

public class PuzzleStateBehaviour : StateMachineBehaviour
{
	private IPuzzle _puzzle;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (_puzzle == null)
		{
			_puzzle = animator.GetComponent<PuzzleAnimatorRef>().Puzzle;
		}
		_puzzle.Navigate(Vector2.zero);
	}
}
