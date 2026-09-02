using System;
using System.Collections.Generic;

namespace NPOI.Util;

public class IdentifierManager
{
	internal class Segment
	{
		public long start;

		public long end;

		public Segment(long start, long end)
		{
			this.start = start;
			this.end = end;
		}

		public override string ToString()
		{
			return "[" + start + "; " + end + "]";
		}
	}

	public static long MAX_ID = 9223372036854775806L;

	public static long MIN_ID = 0L;

	private long upperbound;

	private long lowerbound;

	private List<Segment> segments;

	public IdentifierManager(long lowerbound, long upperbound)
	{
		if (lowerbound > upperbound)
		{
			throw new ArgumentException("lowerbound must not be greater than upperbound, had " + lowerbound + " and " + upperbound);
		}
		if (lowerbound < MIN_ID)
		{
			throw new ArgumentException("lowerbound must be greater than or equal to " + MIN_ID);
		}
		if (upperbound > MAX_ID)
		{
			throw new ArgumentException("upperbound must be less than or equal to " + MAX_ID + " but had " + upperbound);
		}
		this.lowerbound = lowerbound;
		this.upperbound = upperbound;
		segments = new List<Segment>();
		segments.Add(new Segment(lowerbound, upperbound));
	}

	public long Reserve(long id)
	{
		if (id < lowerbound || id > upperbound)
		{
			throw new ArgumentException("Value for parameter 'id' was out of bounds");
		}
		VerifyIdentifiersLeft();
		if (id == upperbound)
		{
			int index = segments.Count - 1;
			Segment segment = segments[index];
			if (segment.end == upperbound)
			{
				segment.end = upperbound - 1;
				if (segment.start > segment.end)
				{
					segments.RemoveAt(index);
				}
				return id;
			}
			return ReserveNew();
		}
		if (id == lowerbound)
		{
			Segment segment2 = segments[0];
			if (segment2.start == lowerbound)
			{
				segment2.start = lowerbound + 1;
				if (segment2.end < segment2.start)
				{
					segments.RemoveAt(0);
				}
				return id;
			}
			return ReserveNew();
		}
		for (int i = 0; i < segments.Count; i++)
		{
			Segment segment3 = segments[i];
			if (segment3.end < id)
			{
				continue;
			}
			if (segment3.start > id)
			{
				break;
			}
			if (segment3.start == id)
			{
				segment3.start = id + 1;
				if (segment3.end < segment3.start)
				{
					segments.Remove(segment3);
				}
				return id;
			}
			if (segment3.end == id)
			{
				segment3.end = id - 1;
				if (segment3.start > segment3.end)
				{
					segments.Remove(segment3);
				}
				return id;
			}
			segments.Add(new Segment(id + 1, segment3.end));
			segment3.end = id - 1;
			return id;
		}
		return ReserveNew();
	}

	public long ReserveNew()
	{
		VerifyIdentifiersLeft();
		Segment segment = segments[0];
		long start = segment.start;
		segment.start++;
		if (segment.start > segment.end)
		{
			segments.RemoveAt(0);
		}
		return start;
	}

	public bool Release(long id)
	{
		if (id < lowerbound || id > upperbound)
		{
			throw new ArgumentException("Value for parameter 'id' was out of bounds, had " + id + ", but should be within [" + lowerbound + ":" + upperbound + "]");
		}
		if (id == upperbound)
		{
			int index = segments.Count - 1;
			Segment segment = segments[index];
			if (segment.end == upperbound - 1)
			{
				segment.end = upperbound;
				return true;
			}
			if (segment.end == upperbound)
			{
				return false;
			}
			segments.Add(new Segment(upperbound, upperbound));
			return true;
		}
		if (id == lowerbound)
		{
			Segment segment2 = segments[0];
			if (segment2.start == lowerbound + 1)
			{
				segment2.start = lowerbound;
				return true;
			}
			if (segment2.start == lowerbound)
			{
				return false;
			}
			segments.Insert(0, new Segment(lowerbound, lowerbound));
			return true;
		}
		long num = id + 1;
		long num2 = id - 1;
		for (int i = 0; i < segments.Count; i++)
		{
			Segment segment3 = segments[0];
			if (segment3.end < num2)
			{
				continue;
			}
			if (segment3.start > num)
			{
				segments.Insert(i, new Segment(id, id));
				return true;
			}
			if (segment3.start == num)
			{
				segment3.start = id;
				return true;
			}
			if (segment3.end != num2)
			{
				break;
			}
			segment3.end = id;
			if (i + 1 < segments.Count)
			{
				Segment segment4 = segments[i + 1];
				if (segment4.start == segment3.end + 1)
				{
					segment3.end = segment4.end;
					segments.Remove(segment4);
				}
			}
			return true;
		}
		return false;
	}

	public long GetRemainingIdentifiers()
	{
		long num = 0L;
		foreach (Segment segment in segments)
		{
			num -= segment.start;
			num = num + segment.end + 1;
		}
		return num;
	}

	private void VerifyIdentifiersLeft()
	{
		if (segments.Count == 0)
		{
			throw new InvalidOperationException("No identifiers left");
		}
	}
}
