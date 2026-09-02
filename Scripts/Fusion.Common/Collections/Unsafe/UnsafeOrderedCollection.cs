#define DEBUG
using System;
using System.Collections.Generic;
using Fusion;

namespace Collections.Unsafe;

internal struct UnsafeOrderedCollection
{
	public struct Entry
	{
		public const int ALIGNMENT = 4;

		public int Left;

		public int Right;

		public int Balance;
	}

	public struct Iterator
	{
		private unsafe fixed int _stack[64];

		private int _depth;

		private int _index;

		public unsafe Entry* Current;

		public unsafe UnsafeOrderedCollection* Collection;

		public unsafe Iterator(UnsafeOrderedCollection* collection)
		{
			Collection = collection;
			Current = null;
			_depth = 0;
			_index = Collection->Root;
		}

		public unsafe bool Next()
		{
			if (Current != null)
			{
				_index = Current->Right;
			}
			if (_index != 0 || _depth > 0)
			{
				while (_index != 0)
				{
					Assert.Check(_depth < 64);
					_stack[_depth++] = _index;
					_index = GetEntry(Collection, _index)->Left;
				}
				_index = _stack[--_depth];
				Current = GetEntry(Collection, _index);
				return true;
			}
			Current = null;
			return false;
		}

		public unsafe void Reset()
		{
			_depth = 0;
			_index = Collection->Root;
		}
	}

	public const int MAX_DEPTH = 64;

	private const string COLLECTION_FULL = "Fixed size ordered collection is full";

	public int Root;

	public int UsedCount;

	public int FreeHead;

	public int FreeCount;

	public int KeyOffset;

	public UnsafeBuffer Entries;

	public unsafe static int Height(UnsafeOrderedCollection* collection)
	{
		return Height(collection, collection->Root);
	}

	public unsafe static int Count(UnsafeOrderedCollection* collection)
	{
		return collection->UsedCount - collection->FreeCount;
	}

	public unsafe static void Remove<T>(UnsafeOrderedCollection* collection, T key) where T : unmanaged, IComparable<T>
	{
		collection->Root = DeletePerform(collection, key);
	}

	public unsafe static void Insert<T>(UnsafeOrderedCollection* collection, T key) where T : unmanaged, IComparable<T>
	{
		if (collection->FreeCount == 0 && collection->UsedCount == collection->Entries.Length)
		{
			if (collection->Entries.Dynamic != 1)
			{
				throw new InvalidOperationException("Fixed size ordered collection is full");
			}
			Expand(collection);
		}
		collection->Root = InsertPerform(collection, key, update: false);
	}

	public unsafe static Entry* Find<T>(UnsafeOrderedCollection* collection, T key) where T : unmanaged, IComparable<T>
	{
		int num = collection->Root;
		while (num != 0)
		{
			Entry* entry = GetEntry(collection, num);
			T key2 = GetKey<T>(collection, num);
			int num2 = key.CompareTo(key2);
			if (num2 < 0)
			{
				num = entry->Left;
				continue;
			}
			if (num2 > 0)
			{
				num = entry->Right;
				continue;
			}
			return entry;
		}
		return null;
	}

	public unsafe static Entry* GetEntry(UnsafeOrderedCollection* collection, int index)
	{
		if (index <= 0)
		{
			return null;
		}
		return (Entry*)UnsafeBuffer.Element(collection->Entries.Ptr, index - 1, collection->Entries.Stride);
	}

	public unsafe static T GetKey<T>(UnsafeOrderedCollection* collection, int index) where T : unmanaged
	{
		Assert.Check(index > 0);
		Entry* entry = GetEntry(collection, index);
		return *(T*)((byte*)entry + collection->KeyOffset);
	}

	private unsafe static int Height(UnsafeOrderedCollection* collection, int index)
	{
		if (index == 0)
		{
			return 0;
		}
		Entry* entry = GetEntry(collection, index);
		return 1 + Math.Max(Height(collection, entry->Left), Height(collection, entry->Right));
	}

	private unsafe static void SetKey<T>(UnsafeOrderedCollection* collection, int index, T value) where T : unmanaged
	{
		Entry* entry = GetEntry(collection, index);
		*(T*)((byte*)entry + collection->KeyOffset) = value;
	}

	private unsafe static void Expand(UnsafeOrderedCollection* collection)
	{
		Assert.Check(collection->Entries.Dynamic == 1);
		Assert.Check(collection->FreeCount == 0);
		Assert.Check(collection->FreeHead == 0);
		int length = collection->Entries.Length * 2;
		UnsafeBuffer unsafeBuffer = default;
		UnsafeBuffer.InitDynamic(&unsafeBuffer, length, collection->Entries.Stride);
		UnsafeBuffer.Copy(collection->Entries, 0, unsafeBuffer, 0, collection->Entries.Length);
		UnsafeBuffer.Free(&collection->Entries);
		collection->Entries = unsafeBuffer;
	}

	private unsafe static void FreeEntry(UnsafeOrderedCollection* collection, int entryIndex)
	{
		Entry* entry = GetEntry(collection, entryIndex);
		entry->Right = 0;
		entry->Balance = 0;
		entry->Left = collection->FreeHead;
		collection->FreeHead = entryIndex;
		collection->FreeCount++;
	}

	private unsafe static int CreateEntry<T>(UnsafeOrderedCollection* collection, T key) where T : unmanaged
	{
		Entry* entry;
		int result;
		if (collection->FreeHead > 0)
		{
			Assert.Check(collection->FreeCount > 0);
			entry = GetEntry(collection, result = collection->FreeHead);
			collection->FreeHead = entry->Left;
			collection->FreeCount--;
			entry->Left = 0;
		}
		else
		{
			Assert.Check(collection->UsedCount < collection->Entries.Length);
			entry = GetEntry(collection, result = ++collection->UsedCount);
		}
		Assert.Check(entry->Left == 0);
		Assert.Check(entry->Right == 0);
		Assert.Check(entry->Balance == 0);
		*(T*)((byte*)entry + collection->KeyOffset) = key;
		return result;
	}

	private unsafe static int InsertPerform<T>(UnsafeOrderedCollection* collection, T insertKey, bool update) where T : unmanaged, IComparable<T>
	{
		int* ptr = stackalloc int[64];
		bool* ptr2 = stackalloc bool[64];
		int num = collection->Root;
		int num2 = 0;
		while (num != 0 && num2 < 63)
		{
			Entry* entry = GetEntry(collection, num);
			T key = GetKey<T>(collection, num);
			int num3 = insertKey.CompareTo(key);
			if (num3 < 0)
			{
				ptr[num2] = num;
				ptr2[num2] = true;
				num = entry->Left;
			}
			else
			{
				if (num3 <= 0)
				{
					if (update)
					{
						SetKey(collection, num, insertKey);
					}
					return collection->Root;
				}
				ptr[num2] = num;
				ptr2[num2] = false;
				num = entry->Right;
			}
			num2++;
		}
		if (num != 0)
		{
			throw new InvalidOperationException("MAX_DEPTH EXCEEDED");
		}
		if (num2 == 0)
		{
			return CreateEntry(collection, insertKey);
		}
		ptr[num2++] = CreateEntry(collection, insertKey);
		int num4 = num2 - 2;
		int num5 = 0;
		while (num4 >= 0)
		{
			Entry* entry2 = GetEntry(collection, ptr[num4]);
			if (ptr2[num4])
			{
				entry2->Left = ptr[num4 + 1];
				entry2->Balance++;
			}
			else
			{
				entry2->Right = ptr[num4 + 1];
				entry2->Balance--;
			}
			if (entry2->Balance == 0)
			{
				break;
			}
			if (entry2->Balance == 2)
			{
				Entry* entry3 = GetEntry(collection, entry2->Left);
				if (entry3->Balance == 1)
				{
					ptr[num4] = RotateRight(collection, ptr[num4], &num5);
				}
				else
				{
					ptr[num4] = RotateLeftRight(collection, ptr[num4]);
				}
				break;
			}
			if (entry2->Balance == -2)
			{
				Entry* entry4 = GetEntry(collection, entry2->Right);
				if (entry4->Balance == -1)
				{
					ptr[num4] = RotateLeft(collection, ptr[num4], &num5);
				}
				else
				{
					ptr[num4] = RotateRightLeft(collection, ptr[num4]);
				}
				break;
			}
			num4--;
		}
		if (--num4 >= 0)
		{
			Entry* entry5 = GetEntry(collection, ptr[num4]);
			if (ptr2[num4])
			{
				entry5->Left = ptr[num4 + 1];
			}
			else
			{
				entry5->Right = ptr[num4 + 1];
			}
		}
		return *ptr;
	}

	private unsafe static int DeletePerform<T>(UnsafeOrderedCollection* collection, T deleteKey) where T : unmanaged, IComparable<T>
	{
		int* ptr = stackalloc int[64];
		sbyte* ptr2 = stackalloc sbyte[64];
		int num = collection->Root;
		int num2 = 0;
		while (num != 0 && num2 < 63)
		{
			Entry* entry = GetEntry(collection, num);
			T key = GetKey<T>(collection, num);
			int num3 = deleteKey.CompareTo(key);
			if (num3 < 0)
			{
				ptr[num2] = num;
				ptr2[num2] = -1;
				num = entry->Left;
				num2++;
				continue;
			}
			if (num3 > 0)
			{
				ptr[num2] = num;
				ptr2[num2] = 1;
				num = entry->Right;
				num2++;
				continue;
			}
			ptr[num2] = 0;
			ptr2[num2] = 0;
			num2++;
			break;
		}
		if (num == 0)
		{
			return collection->Root;
		}
		int num4 = num2 - 1;
		Entry* entry2 = GetEntry(collection, num);
		int left = entry2->Left;
		int right = entry2->Right;
		int balance = entry2->Balance;
		FreeEntry(collection, num);
		if (left + right == 0 && num2 == 1)
		{
			return 0;
		}
		if (right == 0)
		{
			ptr[num4] = left;
		}
		else if (left == 0)
		{
			ptr[num4] = right;
		}
		else
		{
			bool flag = false;
			int num5 = right;
			Entry* entry3 = GetEntry(collection, num5);
			while (entry3->Left != 0)
			{
				ptr[num2] = num5;
				ptr2[num2] = -1;
				num2++;
				flag = true;
				num5 = entry3->Left;
				entry3 = GetEntry(collection, num5);
			}
			ptr[num4] = num5;
			ptr2[num4] = 1;
			entry3->Left = left;
			entry3->Balance = balance;
			if (flag)
			{
				ptr[num2] = entry3->Right;
				ptr2[num2] = 0;
				num2++;
			}
		}
		int num6 = num2 - 1;
		int num7 = num6;
		int num8 = 0;
		if (ptr[num6] == 0)
		{
			num6--;
		}
		while (num6 >= 0)
		{
			Entry* entry4 = GetEntry(collection, ptr[num6]);
			if (num6 < num7)
			{
				switch (ptr2[num6])
				{
				case -1:
					entry4->Left = ptr[num6 + 1];
					break;
				case 1:
					entry4->Right = ptr[num6 + 1];
					break;
				}
			}
			entry4->Balance += ptr2[num6];
			if (entry4->Balance == 2)
			{
				Entry* entry5 = GetEntry(collection, entry4->Left);
				if (entry5->Balance >= 0)
				{
					ptr[num6] = RotateRight(collection, ptr[num6], &num8);
					if (num8 == -1)
					{
						break;
					}
				}
				else
				{
					ptr[num6] = RotateLeftRight(collection, ptr[num6]);
				}
			}
			else if (entry4->Balance == -2)
			{
				Entry* entry6 = GetEntry(collection, entry4->Right);
				if (entry6->Balance <= 0)
				{
					ptr[num6] = RotateLeft(collection, ptr[num6], &num8);
					if (num8 == 1)
					{
						break;
					}
				}
				else
				{
					ptr[num6] = RotateRightLeft(collection, ptr[num6]);
				}
			}
			else if (entry4->Balance != 0)
			{
				break;
			}
			num6--;
		}
		for (num6--; num6 >= 0; num6--)
		{
			Entry* entry7 = GetEntry(collection, ptr[num6]);
			switch (ptr2[num6])
			{
			case -1:
				entry7->Left = ptr[num6 + 1];
				break;
			case 1:
				entry7->Right = ptr[num6 + 1];
				break;
			}
		}
		return *ptr;
	}

	private unsafe static int RotateLeftRight(UnsafeOrderedCollection* collection, int nodeIndex)
	{
		Entry* entry = GetEntry(collection, nodeIndex);
		int left = entry->Left;
		Entry* entry2 = GetEntry(collection, left);
		int right = entry2->Right;
		Entry* entry3 = GetEntry(collection, right);
		int left2 = entry3->Left;
		int right2 = entry3->Right;
		entry->Left = right2;
		entry2->Right = left2;
		entry3->Left = left;
		entry3->Right = nodeIndex;
		if (entry3->Balance == -1)
		{
			entry->Balance = 0;
			entry2->Balance = 1;
		}
		else if (entry3->Balance == 0)
		{
			entry->Balance = 0;
			entry2->Balance = 0;
		}
		else
		{
			entry->Balance = -1;
			entry2->Balance = 0;
		}
		entry3->Balance = 0;
		return right;
	}

	private unsafe static int RotateRightLeft(UnsafeOrderedCollection* collection, int entryIndex)
	{
		Entry* entry = GetEntry(collection, entryIndex);
		int right = entry->Right;
		Entry* entry2 = GetEntry(collection, right);
		int left = entry2->Left;
		Entry* entry3 = GetEntry(collection, left);
		int left2 = entry3->Left;
		int right2 = entry3->Right;
		entry->Right = left2;
		entry2->Left = right2;
		entry3->Right = right;
		entry3->Left = entryIndex;
		if (entry3->Balance == 1)
		{
			entry->Balance = 0;
			entry2->Balance = -1;
		}
		else if (entry3->Balance == 0)
		{
			entry->Balance = 0;
			entry2->Balance = 0;
		}
		else
		{
			entry->Balance = 1;
			entry2->Balance = 0;
		}
		entry3->Balance = 0;
		return left;
	}

	private unsafe static int RotateRight(UnsafeOrderedCollection* collection, int entryIndex, int* balance)
	{
		Entry* entry = GetEntry(collection, entryIndex);
		int left = entry->Left;
		Entry* entry2 = GetEntry(collection, left);
		entry->Left = entry2->Right;
		entry2->Right = entryIndex;
		*balance = --entry2->Balance;
		entry->Balance = -entry2->Balance;
		return left;
	}

	private unsafe static int RotateLeft(UnsafeOrderedCollection* collection, int entryIndex, int* balance)
	{
		Entry* entry = GetEntry(collection, entryIndex);
		int right = entry->Right;
		Entry* entry2 = GetEntry(collection, right);
		entry->Right = entry2->Left;
		entry2->Left = entryIndex;
		*balance = ++entry2->Balance;
		entry->Balance = -entry2->Balance;
		return right;
	}

	internal unsafe static UnsafeOrderedCollection* Allocate<T>(int capacity) where T : unmanaged, IComparable<T>
	{
		return Allocate(capacity, sizeof(T));
	}

	internal unsafe static UnsafeOrderedCollection* Allocate(int capacity, int valStride)
	{
		int stride = sizeof(Entry);
		int alignment = Native.GetAlignment(valStride);
		int alignment2 = Math.Max(4, alignment);
		valStride = Native.RoundToAlignment(valStride, alignment2);
		stride = Native.RoundToAlignment(stride, alignment2);
		UnsafeOrderedCollection* ptr = Native.MallocAndClear<UnsafeOrderedCollection>();
		UnsafeBuffer.InitDynamic(&ptr->Entries, capacity, stride + valStride);
		ptr->FreeCount = 0;
		ptr->UsedCount = 0;
		ptr->KeyOffset = stride;
		return ptr;
	}

	internal unsafe static void Free(UnsafeOrderedCollection* collection)
	{
		UnsafeBuffer.Free(&collection->Entries);
		Native.Free(collection);
	}

	public unsafe static string PrintTree<T>(UnsafeOrderedCollection* collection, Func<T, string> print) where T : unmanaged
	{
		return PrintEntry(collection, collection->Root, print);
	}

	private unsafe static string PrintEntry<T>(UnsafeOrderedCollection* collection, int index, Func<T, string> print) where T : unmanaged
	{
		Entry* entry = GetEntry(collection, index);
		if (entry == null)
		{
			return "*";
		}
		if (entry->Left == 0 && entry->Right == 0)
		{
			return GetKey<T>(collection, index).ToString();
		}
		string text = print(GetKey<T>(collection, index));
		string text2 = PrintBalance(entry);
		string text3 = PrintEntry(collection, entry->Left, print);
		string text4 = PrintEntry(collection, entry->Right, print);
		if (index == collection->Root)
		{
			return text + text2 + "=" + text3 + "|" + text4;
		}
		return "[" + text + text2 + "=" + text3 + "|" + text4 + "]";
	}

	private unsafe static string PrintBalance(Entry* entry)
	{
		return entry->Balance switch
		{
			-2 => "RR", 
			-1 => "R", 
			0 => "", 
			1 => "L", 
			2 => "LL", 
			_ => throw new InvalidOperationException(entry->Balance.ToString()), 
		};
	}

	public unsafe static void VisitNodes<T>(UnsafeOrderedCollection* collection, Action<T?, T, int, bool> callback) where T : unmanaged
	{
		Stack<(int, int, int, bool)> stack = new Stack<(int, int, int, bool)>();
		stack.Push((0, collection->Root, 0, false));
		while (stack.Count > 0)
		{
			var (num, num2, num3, arg) = stack.Pop();
			if (num2 > 0)
			{
				Entry* entry = GetEntry(collection, num2);
				stack.Push((num2, entry->Left, num3 + 1, true));
				stack.Push((num2, entry->Right, num3 + 1, false));
				if (num > 0)
				{
					callback(GetKey<T>(collection, num), GetKey<T>(collection, num2), num3, arg);
				}
				else
				{
					callback(null, GetKey<T>(collection, num2), num3, arg);
				}
			}
		}
	}
}
