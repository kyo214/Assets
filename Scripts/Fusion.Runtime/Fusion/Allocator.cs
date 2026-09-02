#define DEBUG
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion.Sockets;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
public struct Allocator
{
	[StructLayout(LayoutKind.Explicit)]
	private struct Block
	{
		public const int SIZE = 28;

		[FieldOffset(0)]
		public Ptr Prev;

		[FieldOffset(4)]
		public Ptr Next;

		[FieldOffset(8)]
		public int Bucket;

		[FieldOffset(12)]
		public Ptr SegmentsFree;

		[FieldOffset(16)]
		public int SegmentsUsed;

		[FieldOffset(20)]
		public int SegmentsAllocated;

		[FieldOffset(24)]
		public int Index;

		public unsafe int SegmentsFreeCount(Allocator* a)
		{
			int num = 0;
			Ptr ptr = SegmentsFree;
			while ((bool)ptr)
			{
				num++;
				ptr = ((Segment*)a->Ptr(ptr))->Next;
			}
			return num;
		}

		public unsafe bool SegmentsFreeContains(Allocator* a, void* ptr)
		{
			Ptr ptr2 = SegmentsFree;
			while ((bool)ptr2)
			{
				if (ptr2 == a->Ptr(ptr))
				{
					return true;
				}
				ptr2 = ((Segment*)a->Ptr(ptr2))->Next;
			}
			return false;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct BlockList
	{
		public const int SIZE = 8;

		[FieldOffset(0)]
		public Ptr Head;

		[FieldOffset(4)]
		public Ptr Tail;

		public bool IsEmpty => Head.Address == 0;

		public unsafe void AddFirst(Allocator* a, Block* item)
		{
			Assert.Check(item->Next == default(Ptr));
			Assert.Check(item->Prev == default(Ptr));
			Assert.Check(!Contains(a, item));
			Ptr ptr = a->Meta(item);
			item->Next = Head;
			item->Prev = default;
			if ((bool)Head)
			{
				((Block*)a->Meta(Head))->Prev = ptr;
				Head = ptr;
			}
			else
			{
				Head = ptr;
				Tail = ptr;
			}
			Assert.Check(Contains(a, item));
			DebugVerifyListIntegrity(a);
		}

		public unsafe void AddLast(Allocator* a, Block* item)
		{
			Assert.Check(item->Next == default(Ptr));
			Assert.Check(item->Prev == default(Ptr));
			Assert.Check(!Contains(a, item));
			Ptr ptr = a->Meta(item);
			item->Next = default;
			item->Prev = Tail;
			if ((bool)Tail)
			{
				((Block*)a->Meta(Tail))->Next = ptr;
				Tail = ptr;
			}
			else
			{
				Head = ptr;
				Tail = ptr;
			}
			Assert.Check(Contains(a, item));
			DebugVerifyListIntegrity(a);
		}

		public unsafe void MoveFirst(Allocator* a, Block* item)
		{
			Assert.Check(Contains(a, item));
			Ptr ptr = a->Meta(item);
			if (ptr != Head)
			{
				Remove(a, item);
				AddFirst(a, item);
			}
		}

		public unsafe void MoveLast(Allocator* a, Block* item)
		{
			Assert.Check(Contains(a, item));
			Ptr ptr = a->Meta(item);
			if (ptr != Tail)
			{
				Remove(a, item);
				AddLast(a, item);
			}
		}

		public unsafe Block* RemoveHead(Allocator* a)
		{
			Assert.Check(Head != default(Ptr));
			Block* ptr = (Block*)a->Meta(Head);
			Remove(a, ptr);
			return ptr;
		}

		public unsafe void Remove(Allocator* a, Block* item)
		{
			Assert.Check(Contains(a, item));
			if ((bool)item->Prev)
			{
				((Block*)a->Meta(item->Prev))->Next = item->Next;
			}
			if ((bool)item->Next)
			{
				((Block*)a->Meta(item->Next))->Prev = item->Prev;
			}
			Ptr ptr = a->Meta(item);
			if (ptr == Tail)
			{
				Tail = item->Prev;
			}
			if (ptr == Head)
			{
				Head = item->Next;
			}
			item->Prev = default;
			item->Next = default;
			DebugVerifyListIntegrity(a);
			Assert.Check(!Contains(a, item));
		}

		public unsafe bool Contains(Allocator* a, Block* item)
		{
			Ptr ptr = Head;
			while ((bool)ptr)
			{
				Block* ptr2 = (Block*)a->Meta(ptr);
				if (ptr2 == item)
				{
					return true;
				}
				ptr = ptr2->Next;
			}
			return false;
		}

		[Conditional("DEBUG")]
		private unsafe void DebugVerifyListIntegrity(Allocator* a)
		{
			Ptr ptr = Head;
			while ((bool)ptr)
			{
				Block* ptr2 = (Block*)a->Meta(ptr);
				if (ptr == Head)
				{
					Assert.Check(ptr2->Prev == default(Ptr));
				}
				if (ptr == Tail)
				{
					Assert.Check(ptr2->Next == default(Ptr));
				}
				if (ptr != Head && ptr != Tail)
				{
					Assert.Check(ptr2->Prev != default(Ptr));
					Assert.Check(ptr2->Next != default(Ptr));
				}
				ptr = ptr2->Next;
			}
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct Bucket
	{
		public const int SIZE = 16;

		[FieldOffset(0)]
		public int Index;

		[FieldOffset(4)]
		public int SegmentStride;

		[FieldOffset(8)]
		public int SegmentWordCount;

		[FieldOffset(12)]
		public int SegmentCapacity;

		public static Bucket Create(int index, int wordCount, Config config)
		{
			Bucket result = default;
			result.Index = index;
			result.SegmentWordCount = wordCount;
			result.SegmentStride = wordCount * 8;
			result.SegmentCapacity = ((wordCount > 0) ? (config.BlockWordCount / wordCount) : 0);
			return result;
		}
	}

	private static class AllocatorBucketSize
	{
		public static readonly int[] Sizes = new int[57]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 10,
			12, 14, 16, 20, 24, 28, 32, 40, 48, 56,
			64, 80, 96, 112, 128, 160, 192, 224, 256, 320,
			384, 448, 512, 640, 768, 896, 1024, 1280, 1536, 1792,
			2048, 2560, 3072, 3584, 4096, 5120, 6144, 7168, 8192, 10240,
			12288, 14336, 16384, 20480, 24576, 28672, 32768
		};
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Config
	{
		public const int SIZE = 12;

		public const PageSizes DEFAULT_BLOCK_SHIFT = PageSizes._16Kb;

		public const int DEFAULT_BLOCK_COUNT = 128;

		[FieldOffset(0)]
		public int BlockShift;

		[FieldOffset(4)]
		public int BlockCount;

		[FieldOffset(8)]
		public int GlobalsSize;

		public int BlockByteSize => 1 << BlockShift;

		public int BlockWordCount => WordCount(BlockByteSize);

		public int HeapSizeUsable => BlockByteSize * BlockCount;

		public int HeapSizeAllocated => HeapSizeUsable + 8;

		public Config(PageSizes shift, int count, int globalsSize)
		{
			BlockShift = (int)shift;
			BlockCount = Math.Max(1, count);
			GlobalsSize = globalsSize;
		}

		public bool Equals(Config other)
		{
			return BlockShift == other.BlockShift && BlockCount == other.BlockCount;
		}

		public override bool Equals(object obj)
		{
			return obj is Config other && Equals(other);
		}

		public override int GetHashCode()
		{
			return (BlockShift * 397) ^ BlockCount;
		}

		public override string ToString()
		{
			return $"[Allocator.Config: {12}/{BlockShift}/{BlockCount}/{GlobalsSize}/{BlockByteSize}/{BlockWordCount}/{HeapSizeUsable}/{HeapSizeAllocated}]";
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct Segment
	{
		public const int SIZE = 4;

		[FieldOffset(0)]
		public Ptr Next;
	}

	private const int SIZE = 112;

	private const int WORD_SHIFT = 3;

	private const int WORD_BYTE_SIZE = 8;

	public const int HEAP_ALIGNMENT = 8;

	public const int REPLICATE_WORD_SHIFT = 2;

	public const int REPLICATE_WORD_SIZE = 4;

	public const int REPLICATE_WORD_ALIGN = 4;

	public const int BUCKET_COUNT = 57;

	public const byte BUCKET_INVALID = byte.MaxValue;

	private const int PTR_SIZE = 8;

	[FieldOffset(0)]
	private unsafe byte* _root;

	[FieldOffset(8)]
	private unsafe byte* _heap;

	[FieldOffset(16)]
	private unsafe byte* _meta;

	[FieldOffset(24)]
	private unsafe void* _globals;

	[FieldOffset(32)]
	private unsafe void* _checksum;

	[FieldOffset(40)]
	private unsafe void* _replicate;

	[FieldOffset(48)]
	private unsafe Block* _blocks;

	[FieldOffset(56)]
	private unsafe BlockList* _blocksFreeList;

	[FieldOffset(64)]
	private unsafe Bucket* _buckets;

	[FieldOffset(72)]
	private unsafe byte* _bucketsMap;

	[FieldOffset(80)]
	private unsafe BlockList* _bucketsLists;

	[FieldOffset(88)]
	private Config _config;

	[FieldOffset(100)]
	private int _maxBlockIndexUsed;

	[FieldOffset(104)]
	private int _checksumByteLength;

	[FieldOffset(108)]
	private int _replicateByteLength;

	internal Config Configuration => _config;

	internal unsafe void* Globals => _globals;

	internal unsafe void* Checksum => _checksum;

	internal unsafe int* Replicate
	{
		get
		{
			Assert.Check(Native.IsPointerAligned(_replicate, 4));
			return (int*)_replicate;
		}
	}

	internal int ChecksumByteLength => _checksumByteLength;

	internal int ReplicateByteLength => _replicateByteLength;

	internal int ReplicateWordLength
	{
		get
		{
			Assert.Check(ReplicateByteLength % 4 == 0);
			return ReplicateByteLength / 4;
		}
	}

	internal unsafe int ReplicateHeapWordOffset
	{
		get
		{
			long num = _heap - (byte*)_replicate;
			Assert.Check(Native.IsPointerAligned(_heap, 4));
			Assert.Check(Native.IsPointerAligned(_replicate, 4));
			Assert.Check(num >= 0);
			Assert.Check(num % 4 == 0);
			return (int)(num / 4);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Ptr Meta(void* p)
	{
		Assert.Check(IsPointerInMeta(p));
		Ptr result = default;
		result.Address = (int)((byte*)p - _root);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe void* Meta(Ptr ptr)
	{
		byte* ptr2 = _root + ptr.Address;
		Assert.Check(IsPointerInMeta(ptr2));
		return ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int GetReplicateWordOffset(void* ptr)
	{
		return GetReplicateWordOffset(Ptr(ptr));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int GetReplicateWordOffset(Ptr ptr)
	{
		long num = (byte*)Ptr(ptr) - (byte*)_replicate;
		Assert.Check(num >= 0);
		Assert.Check(num % 4 == 0);
		return (int)(num / 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe Ptr Ptr(void* p)
	{
		Assert.Check(IsPointerInHeap(p));
		Ptr result = default;
		result.Address = (int)((byte*)p - _root);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void* Ptr(Ptr ptr)
	{
		byte* ptr2 = _root + ptr.Address;
		Assert.Check(IsPointerInHeap(ptr2), ptr.Address);
		return ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe T* Ptr<T>(Ptr ptr) where T : unmanaged
	{
		byte* ptr2 = _root + ptr.Address;
		Assert.Check(IsPointerInHeap(ptr2), ptr.Address);
		return (T*)ptr2;
	}

	private unsafe bool IsPointerInMeta(void* p)
	{
		return p >= _meta && p < _heap;
	}

	internal unsafe bool IsPointerInHeap(void* p)
	{
		return p >= _heap && p < _heap + _config.HeapSizeUsable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int WordCount(int size)
	{
		Assert.Check(size > 0);
		return size + 7 >> 3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Bucket* GetBucket(int index)
	{
		Assert.Check(index >= 0 && index < 57);
		return _buckets + index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Bucket* GetBucketForBlock(Block* block)
	{
		return GetBucket(block->Bucket);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe BlockList* GetBucketList(int index)
	{
		Assert.Check(index >= 0 && index < 57);
		return _bucketsLists + index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Block* GetBlock(int index)
	{
		Assert.Check(index >= 0 && index < _config.BlockCount);
		return _blocks + index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Block* GetBlock(long index)
	{
		Assert.Check(index >= 0 && index < _config.BlockCount);
		return _blocks + index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe int GetBlockBucket(long index)
	{
		Assert.Check(index >= 0 && index < _config.BlockCount);
		return _blocks[index].Bucket;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe Block* GetBlockForPointer(void* ptr)
	{
		Assert.Check(IsPointerInHeap(ptr));
		Assert.Check((byte*)ptr - _heap >> _config.BlockShift >= 0);
		Assert.Check((byte*)ptr - _heap >> _config.BlockShift < _config.BlockCount);
		return _blocks + ((byte*)ptr - _heap >> _config.BlockShift);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe int GetBlockIndexForPointer(void* ptr)
	{
		Assert.Check(IsPointerInHeap(ptr));
		Assert.Check((byte*)ptr - _heap >> _config.BlockShift >= 0);
		Assert.Check((byte*)ptr - _heap >> _config.BlockShift < _config.BlockCount);
		return (int)((byte*)ptr - _heap >> _config.BlockShift);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe byte* GetBlockMemory(Block* block)
	{
		int num = (int)(((byte*)block - (byte*)_blocks) / sizeof(Block));
		Assert.Check(num >= 0 && num < _config.BlockCount);
		Assert.Check(num == block->Index);
		return _heap + num * _config.BlockByteSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe byte* GetBlockMemory(long blockIndex)
	{
		Assert.Check(blockIndex >= 0 && blockIndex < _config.BlockCount);
		return _heap + blockIndex * _config.BlockByteSize;
	}

	internal unsafe bool TryGetSegmentRoot(void* ptr, out void* root)
	{
		if (IsPointerInHeap(ptr))
		{
			long num = (byte*)ptr - _heap >> _config.BlockShift;
			Block* block = GetBlock(num);
			byte* blockMemory = GetBlockMemory(num);
			long num2 = ((byte*)ptr - blockMemory) / _buckets[block->Bucket].SegmentStride;
			root = blockMemory + num2 * _buckets[block->Bucket].SegmentStride;
			return true;
		}
		root = null;
		return false;
	}

	internal unsafe void* GetSegmentRoot(void* ptr)
	{
		if (TryGetSegmentRoot(ptr, out var root))
		{
			return root;
		}
		Assert.AlwaysFail("TryGetSegmentRoot failed");
		return null;
	}

	internal unsafe static T* AllocArray<T>(Allocator* s, int length) where T : unmanaged
	{
		return (T*)Alloc(s, sizeof(T) * length);
	}

	internal unsafe static T* AllocAndClearArray<T>(Allocator* s, int length) where T : unmanaged
	{
		return (T*)AllocAndClear(s, sizeof(T) * length);
	}

	internal unsafe static T* Alloc<T>(Allocator* s) where T : unmanaged
	{
		return (T*)Alloc(s, sizeof(T));
	}

	internal unsafe static T* AllocAndClear<T>(Allocator* s) where T : unmanaged
	{
		return (T*)AllocAndClear(s, sizeof(T));
	}

	internal unsafe static void* AllocAndClear(Allocator* a, int size)
	{
		void* ptr = Alloc(a, size);
		Native.MemClear(ptr, size);
		return ptr;
	}

	internal unsafe static int GetWordLengthForReplication(Allocator* allocator)
	{
		return GetByteLengthForReplication(allocator) / 4;
	}

	internal unsafe static int GetByteLengthForReplication(Allocator* allocator)
	{
		int maxBlockIndexUsed = allocator->_maxBlockIndexUsed;
		if (maxBlockIndexUsed < allocator->_config.BlockCount - 1)
		{
			byte* blockMemory = allocator->GetBlockMemory(allocator->GetBlock(maxBlockIndexUsed + 1));
			return (int)(blockMemory - (byte*)allocator->_replicate);
		}
		return allocator->ReplicateByteLength;
	}

	internal unsafe static string PrintDebugInfo(Allocator* allocator)
	{
		long num = (byte*)allocator->_replicate - allocator->_root;
		long num2 = allocator->_heap - allocator->_root;
		long num3 = (byte*)allocator->_globals - allocator->_root;
		long num4 = allocator->_meta - allocator->_root;
		long num5 = allocator->_bucketsMap - allocator->_root;
		long num6 = (byte*)allocator->_buckets - allocator->_root;
		long num7 = (byte*)allocator->_bucketsLists - allocator->_root;
		long num8 = (byte*)allocator->_blocksFreeList - allocator->_root;
		return $"replicateOffset:{num}, bucketmapoffset:{num5}, bucketsoffset:{num6}, bucketsList:{num7}, blocksFreeList:{num8} metaoffset:{num4}, globalsOffset:{num3}, heapOffset:{num2}";
	}

	internal unsafe static void DeltaPack(Simulation.IDeltaCompressor compressor, Allocator* current, Allocator* shared, NetBitBuffer* b)
	{
		b->PadToByteBoundary();
		int offsetBits = b->OffsetBits;
		b->WriteInt32(0, 24);
		int words = current->ReplicateByteLength / 4;
		int num = Math.Max(current->_maxBlockIndexUsed, shared->_maxBlockIndexUsed);
		if (num < current->_config.BlockCount - 1)
		{
			byte* blockMemory = current->GetBlockMemory(current->GetBlock(num + 1));
			words = (int)(blockMemory - (byte*)current->_replicate) / 4;
		}
		Assert.Check(current->ReplicateByteLength % 4 == 0);
		int* replicate = current->Replicate;
		int* replicate2 = shared->Replicate;
		compressor.Pack(replicate, replicate2, words, b);
		b->WriteInt32AtOffset(b->OffsetBits - offsetBits, offsetBits, 24);
		b->PadToByteBoundary();
	}

	internal unsafe static void DeltaUnpack(Allocator* target, Allocator* shared, NetBitBuffer* b)
	{
		b->SeekToByteBoundary();
		int* replicate = target->Replicate;
		int num = b->OffsetBits + b->ReadInt32(24);
		Native.MemCpy(replicate, shared->Replicate, shared->ReplicateByteLength);
		int num2 = 0;
		while (b->OffsetBits < num)
		{
			num2 += b->ReadInt32VarLength(3);
			int* ptr = replicate + num2;
			long num3 = Maths.ZigZagDecode(b->ReadInt64VarLength(6));
			*ptr = (int)(*ptr + num3);
		}
		b->SeekToByteBoundary();
	}

	internal bool CanAllocSize(int size)
	{
		return size >= 1 && size < _config.BlockByteSize;
	}

	internal bool CanAllocSizeAssert_Temp(int size)
	{
		if (size < 1)
		{
			Assert.AlwaysFail($"Invalid temp alloc size: {size}");
			return false;
		}
		if (size >= _config.BlockByteSize)
		{
			Assert.AlwaysFail($"Invalid temp alloc size {size} (max: {_config.BlockByteSize})");
			return false;
		}
		return true;
	}

	internal unsafe static void* Alloc(Allocator* a, int size)
	{
		if (size < 1)
		{
			throw new InvalidOperationException($"invalid size {size}");
		}
		if (size >= a->_config.BlockByteSize)
		{
			throw new InvalidOperationException($"invalid size {size} (max alloc: {a->_config.BlockByteSize})");
		}
		Assert.Check(WordCount(size) * 8 >= size);
		byte b = a->_bucketsMap[WordCount(size)];
		Assert.Check(a->GetBucket(b)->Index == a->_bucketsMap[WordCount(size)]);
		Bucket* bucket;
		BlockList* bucketList;
		while (true)
		{
			DebugVerifyBucketIntegrity(a, b);
			bucket = a->GetBucket(b);
			bucketList = a->GetBucketList(b);
			Assert.Check(bucket->SegmentStride >= size);
			Assert.Check(bucket->SegmentWordCount >= WordCount(size));
			if ((bool)bucketList->Head)
			{
				Block* ptr = (Block*)a->Meta(bucketList->Head);
				void* ptr2 = TryAllocateSegmentFromBlock(a, bucket, ptr, size);
				if (ptr2 != null)
				{
					Assert.Check(a->IsPointerInHeap(ptr2));
					Assert.Check(a->GetBlockForPointer(ptr2) == ptr);
					Assert.Check(ptr->SegmentsAllocated > 0);
					return ptr2;
				}
			}
			if (!a->_blocksFreeList->IsEmpty)
			{
				break;
			}
			if (++b < 57)
			{
				continue;
			}
			throw new OutOfMemoryException();
		}
		Block* ptr3 = a->_blocksFreeList->RemoveHead(a);
		Assert.Check(ptr3->SegmentsFree == default(Ptr));
		Assert.Check(ptr3->SegmentsUsed == 0);
		Assert.Check(ptr3->SegmentsAllocated == 0);
		Assert.Check(ptr3->Prev == default(Ptr));
		Assert.Check(ptr3->Next == default(Ptr));
		Assert.Check(ptr3->Bucket == 255);
		ptr3->Bucket = bucket->Index;
		bucketList->AddFirst(a, ptr3);
		void* ptr4 = TryAllocateSegmentFromBlock(a, bucket, ptr3, size);
		if (ptr4 == null)
		{
			throw new Exception();
		}
		Assert.Check(a->GetBlockIndexForPointer(ptr4) == ptr3->Index);
		Assert.Check(a->GetBlockForPointer(ptr4) == ptr3);
		Assert.Check(ptr3->SegmentsAllocated > 0);
		Assert.Check(a->IsPointerInHeap(ptr4));
		return ptr4;
	}

	private unsafe static void* TryAllocateSegmentFromBlock(Allocator* a, Bucket* bucket, Block* block, int size)
	{
		Assert.Check(bucket->Index == block->Bucket);
		Assert.Check(a->GetBucketForBlock(block) == bucket);
		Assert.Check(a->GetBucketList(bucket->Index)->Contains(a, block));
		Assert.Check(block->SegmentsAllocated >= 0);
		Assert.Check(bucket->SegmentStride >= size);
		Assert.Check(block->SegmentsFreeCount(a) + block->SegmentsAllocated == block->SegmentsUsed);
		void* ptr;
		if (block->SegmentsFree.Address > 0)
		{
			Assert.Check(block->SegmentsUsed > 0);
			ptr = a->Ptr(block->SegmentsFree);
			block->SegmentsFree = ((Segment*)ptr)->Next;
		}
		else if (block->SegmentsUsed < bucket->SegmentCapacity)
		{
			ptr = a->GetBlockMemory(block) + block->SegmentsUsed++ * bucket->SegmentStride;
			Assert.Check(block->SegmentsUsed <= bucket->SegmentCapacity);
		}
		else
		{
			ptr = null;
		}
		if (ptr != null)
		{
			Assert.Check(block->SegmentsAllocated < bucket->SegmentCapacity);
			a->_maxBlockIndexUsed = Math.Max(a->_maxBlockIndexUsed, a->GetBlockIndexForPointer(ptr));
			if (++block->SegmentsAllocated == bucket->SegmentCapacity)
			{
				a->GetBucketList(bucket->Index)->MoveLast(a, block);
			}
		}
		Assert.Check(block->SegmentsFreeCount(a) + block->SegmentsAllocated == block->SegmentsUsed);
		DebugVerifyBucketIntegrity(a, bucket->Index);
		return ptr;
	}

	internal unsafe static void Free(Allocator* a, void* ptr)
	{
		if (ptr == null || !a->IsPointerInHeap(ptr))
		{
			return;
		}
		Block* blockForPointer = a->GetBlockForPointer(ptr);
		Assert.Check(!blockForPointer->SegmentsFreeContains(a, ptr));
		Assert.Check(blockForPointer->SegmentsAllocated > 0);
		*(Segment*)ptr = default;
		((Segment*)ptr)->Next = blockForPointer->SegmentsFree;
		blockForPointer->SegmentsFree = a->Ptr(ptr);
		if (--blockForPointer->SegmentsAllocated == 0)
		{
			int bucket = blockForPointer->Bucket;
			BlockList* bucketList = a->GetBucketList(bucket);
			bucketList->Remove(a, blockForPointer);
			blockForPointer->Bucket = 255;
			blockForPointer->SegmentsFree = default;
			blockForPointer->SegmentsUsed = 0;
			blockForPointer->SegmentsAllocated = 0;
			a->_blocksFreeList->AddFirst(a, blockForPointer);
			DebugVerifyBucketIntegrity(a, bucket);
		}
		else
		{
			Assert.Check(blockForPointer->SegmentsFreeContains(a, ptr));
			Bucket* bucketForBlock = a->GetBucketForBlock(blockForPointer);
			if (bucketForBlock->SegmentCapacity == blockForPointer->SegmentsAllocated + 1)
			{
				a->GetBucketList(blockForPointer->Bucket)->MoveFirst(a, blockForPointer);
			}
			DebugVerifyBucketIntegrity(a, bucketForBlock->Index);
		}
	}

	[Conditional("DEBUG")]
	private unsafe static void DebugVerifyBucketIntegrity(Allocator* a, int index)
	{
		Bucket* bucket = a->GetBucket(index);
		BlockList* bucketList = a->GetBucketList(index);
		Ptr ptr = bucketList->Head;
		bool flag = false;
		while ((bool)ptr)
		{
			Block* ptr2 = (Block*)a->Meta(ptr);
			while (true)
			{
				if (flag)
				{
					Assert.Check(ptr2->SegmentsUsed == bucket->SegmentCapacity);
					Assert.Check(ptr2->SegmentsAllocated == bucket->SegmentCapacity);
					Assert.Check(ptr2->SegmentsFreeCount(a) == 0);
					break;
				}
				if (ptr2->SegmentsAllocated == bucket->SegmentCapacity)
				{
					flag = true;
					continue;
				}
				Assert.Check(ptr2->SegmentsFreeCount(a) + ptr2->SegmentsAllocated == ptr2->SegmentsUsed, ptr2->SegmentsFreeCount(a) + ptr2->SegmentsAllocated, ptr2->SegmentsUsed);
				break;
			}
			ptr = ptr2->Next;
		}
	}

	internal unsafe static void Dispose(Allocator* a)
	{
		if (a != null)
		{
			Native.Free(a);
		}
	}

	internal unsafe static void Copy(Allocator* from, Allocator* to, bool onlyUsed = false)
	{
		Assert.Check(from->_config.Equals(to->_config));
		to->_maxBlockIndexUsed = from->_maxBlockIndexUsed;
		int size = (onlyUsed ? GetByteLengthForReplication(from) : from->_replicateByteLength);
		Native.MemCpy(to->_replicate, from->_replicate, size);
	}

	internal unsafe static Allocator* Create(Config config)
	{
		Assert.Check(sizeof(Allocator) == 112);
		Assert.Check(sizeof(Ptr) == 4);
		Assert.Check(sizeof(Bucket) == 16);
		Assert.Check(sizeof(Config) == 12);
		Assert.Check(sizeof(Segment) == 4);
		Assert.Check(sizeof(Block) == 28);
		Assert.Check(sizeof(BlockList) == 8);
		int num = Native.MallocAndClearBlock(sizeof(Allocator), config.BlockWordCount, sizeof(Bucket) * 57, sizeof(BlockList) * 57, sizeof(BlockList), sizeof(Block) * config.BlockCount, config.GlobalsSize, config.HeapSizeAllocated, out var ptr, out var ptr2, out var ptr3, out var ptr4, out var ptr5, out var ptr6, out var ptr7, out var ptr8);
		byte* ptr9 = (byte*)ptr + num;
		Allocator* ptr10 = (Allocator*)ptr;
		ptr10->_config = config;
		ptr10->_root = (byte*)ptr;
		ptr10->_globals = ptr7;
		ptr10->_meta = (byte*)ptr6;
		ptr10->_checksum = ptr2;
		ptr10->_checksumByteLength = (int)(ptr9 - (byte*)ptr2);
		ptr10->_replicate = ptr4;
		ptr10->_replicateByteLength = (int)(ptr9 - (byte*)ptr10->_replicate);
		Assert.Check(Native.IsPointerAligned(ptr10->_replicate, 4));
		Assert.Check(ptr10->_replicateByteLength % 4 == 0);
		ptr10->_buckets = (Bucket*)ptr3;
		ptr10->_bucketsMap = (byte*)ptr2;
		ptr10->_bucketsLists = (BlockList*)ptr4;
		ptr10->_blocks = (Block*)ptr6;
		ptr10->_blocksFreeList = (BlockList*)ptr5;
		ptr10->_heap = (byte*)Native.AlignPointer(ptr8, 8);
		Assert.Check(Native.IsPointerAligned(ptr10->_heap, 8));
		for (int i = 0; i < AllocatorBucketSize.Sizes.Length; i++)
		{
			ptr10->_buckets[i] = Bucket.Create(i, AllocatorBucketSize.Sizes[i], config);
		}
		byte b = 0;
		for (int j = 0; j < config.BlockWordCount; j++)
		{
			if (ptr10->_buckets[(int)b].SegmentWordCount < j)
			{
				b++;
			}
			Assert.Check(ptr10->_buckets[(int)b].SegmentWordCount >= j);
			ptr10->_bucketsMap[j] = b;
		}
		for (int k = 0; k < config.BlockCount; k++)
		{
			Block* ptr11 = ptr10->_blocks + k;
			ptr11->Bucket = 255;
			ptr11->Index = k;
			ptr10->_blocksFreeList->AddLast(ptr10, ptr11);
		}
		return ptr10;
	}
}
