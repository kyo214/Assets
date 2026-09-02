#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fusion.Profiling;

public class FusionSampler
{
	public class Frame
	{
		public int FrameID = -1;

		public List<Sample> Samples = new List<Sample>();

		public void Reset(int newFrameId)
		{
			Samples.Clear();
		}
	}

	public const string CLIENT_UPDATE = "Client Update";

	public const string SERVER_UPDATE = "Server Update";

	public const string RPC_SEND = "RPC Send";

	public const string CLIENT_TICK = "Client Tick";

	public const string SERVER_TICK = "Server Tick";

	public const string INPUT_TICK_CONSUMED = "Input Tick Consumed";

	public const string ACCUMULATED_TICKS = "Accumulated Ticks";

	public const string CONSUME_INPUT = "Consume Input";

	public const string RTT_TO_SERVER = "RTT To Servver";

	public const string OBJ_COUNT = "Object Count";

	public const string SOCKET_SEND = "Socket Send";

	private string _name;

	private Frame[] _frames;

	private int _currentFrameIndex;

	private int _maxFrames;

	private int _frameHead;

	private int _frameCount;

	private Timer _timer;

	public string Name => _name;

	internal int Head => _frameHead;

	public int FrameCount => _frameCount;

	public List<Sample> this[int i]
	{
		get
		{
			int num = (_frameHead + i) % _maxFrames;
			return _frames[num].Samples;
		}
	}

	internal FusionSampler(string name, Timer timer, int currentFrame, int maxFrames = -1)
	{
		_name = name;
		_timer = timer;
		_maxFrames = maxFrames;
		_frameHead = 0;
		_frames = new Frame[(maxFrames < 1) ? 100 : maxFrames];
		for (int i = 0; i <= currentFrame; i++)
		{
			IncrementFrame(i);
		}
	}

	[Conditional("DEBUG")]
	public void IncrementFrame(int newFrameId)
	{
		if (_maxFrames > 0)
		{
			_currentFrameIndex = newFrameId % _maxFrames;
			Frame frame = _frames[_currentFrameIndex];
			if (frame != null)
			{
				_frames[_currentFrameIndex].Reset(newFrameId);
				_frameHead = (_currentFrameIndex + 1) % _maxFrames;
			}
			else
			{
				_frames[_currentFrameIndex] = new Frame
				{
					FrameID = newFrameId
				};
				_frameCount++;
			}
		}
		else
		{
			_currentFrameIndex = newFrameId;
			if (newFrameId >= _frames.Length)
			{
				Array.Resize(ref _frames, _frames.Length * 2);
			}
			_frames[_currentFrameIndex] = new Frame
			{
				FrameID = newFrameId
			};
			_frameCount++;
		}
		Add("New Frame", -1, newFrameId);
	}

	[Conditional("DEBUG")]
	public void Add(Sample sample)
	{
		Frame frame = _frames[_currentFrameIndex];
		frame.Samples.Add(sample);
	}

	[Conditional("DEBUG")]
	public void Add(string name, int tick, double value, SampleSegmentFlag segmentFlag = SampleSegmentFlag.None, int stage = 0)
	{
		Add(new Sample
		{
			Name = name,
			Time = new SampleTime(_timer.ElapsedInSeconds, tick, stage),
			Data = new SampleData(value, segmentFlag)
		});
	}
}
