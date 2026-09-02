#define DEBUG
using System;

namespace Fusion;

internal class SimulationInputCollection
{
	private int _count;

	private SimulationInput[] _byIndex;

	private SimulationInput[] _byPlayer;

	public int Count => _count;

	public SimulationInputCollection(int playerCount)
	{
		_byIndex = new SimulationInput[playerCount];
		_byPlayer = new SimulationInput[playerCount];
	}

	public SimulationInput GetByIndex(int index)
	{
		if (index >= 0 && index < _count)
		{
			return _byIndex[index];
		}
		return null;
	}

	public SimulationInput GetByPlayer(int player)
	{
		if (player >= 0 && player < _byPlayer.Length)
		{
			return _byPlayer[player];
		}
		return null;
	}

	public void Clear()
	{
		_count = 0;
		Array.Clear(_byIndex, 0, _byIndex.Length);
		Array.Clear(_byPlayer, 0, _byPlayer.Length);
	}

	public void AddInput(SimulationInput input)
	{
		int num = _count++;
		Assert.Check(_byIndex[num] == null);
		Assert.Check(_byPlayer[(int)input.Player] == null);
		_byIndex[num] = input;
		_byPlayer[(int)input.Player] = input;
	}
}
