using System;
using System.Collections.Generic;

[Serializable]
public class LeaderboardDetails
{
	public List<ulong> ID = new List<ulong>();

	public List<int> ScrP = new List<int>();

	public List<string> Prks = new List<string>();

	public byte TotP;

	public byte Life;

	public byte D;

	public short K;

	public byte KE;

	public byte Dif;

	public byte Rev;

	public byte Pzl;

	public short Time;

	public List<bool> FP = new List<bool>();
}
