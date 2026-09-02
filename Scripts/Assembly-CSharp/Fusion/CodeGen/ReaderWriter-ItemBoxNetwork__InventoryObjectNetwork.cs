using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ReaderWriter_0040ItemBoxNetwork__InventoryObjectNetwork : IElementReaderWriter<ItemBoxNetwork.InventoryObjectNetwork>
{
	public static IElementReaderWriter<ItemBoxNetwork.InventoryObjectNetwork> Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ItemBoxNetwork.InventoryObjectNetwork Read(byte* data, int index)
	{
		return *(ItemBoxNetwork.InventoryObjectNetwork*)(data + index * 20);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref ItemBoxNetwork.InventoryObjectNetwork ReadRef(byte* data, int index)
	{
		return ref *(ItemBoxNetwork.InventoryObjectNetwork*)(data + index * 20);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Write(byte* data, int index, ItemBoxNetwork.InventoryObjectNetwork val)
	{
		*(ItemBoxNetwork.InventoryObjectNetwork*)(data + index * 20) = val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetElementWordCount()
	{
		return 5;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IElementReaderWriter<ItemBoxNetwork.InventoryObjectNetwork> GetInstance()
	{
		if (Instance == null)
		{
			Instance = default(ReaderWriter_0040ItemBoxNetwork__InventoryObjectNetwork);
		}
		return Instance;
	}
}
