using System;
using UnityEngine;

namespace Fusion;

[Serializable]
public class HeapConfiguration
{
	[InlineHelp]
	public PageSizes PageShift = PageSizes._16Kb;

	[InlineHelp]
	[Range(16f, 4096f)]
	[MultiPropertyDrawersFix]
	public int PageCount = 128;

	[InlineHelp]
	[HideInInspector]
	public int GlobalsSize;

	internal Allocator.Config ToAllocatorConfig()
	{
		return new Allocator.Config(PageShift, PageCount, GlobalsSize);
	}

	public HeapConfiguration Init(int globalsSize)
	{
		HeapConfiguration heapConfiguration = (HeapConfiguration)MemberwiseClone();
		heapConfiguration.GlobalsSize = globalsSize;
		return heapConfiguration;
	}

	public override string ToString()
	{
		return $"[HeapConfiguration: {PageShift}/{PageCount}/{GlobalsSize}]";
	}
}
