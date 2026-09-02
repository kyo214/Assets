using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dissonance.Extensions;
using UnityEngine;

namespace Dissonance;

public abstract class BaseProximityTrigger<THandle> : BaseCommsTrigger
{
	protected abstract class Grid
	{
		private readonly StringBuilder _nameBuilder = new StringBuilder();

		private readonly List<(Vector3Int, THandle)> _handles = new List<(Vector3Int, THandle)>();

		private readonly HashSet<Vector3Int> _keys = new HashSet<Vector3Int>();

		private IDissonancePlayer _player;

		private const int CacheSize = 128;

		private readonly Dictionary<Vector3Int, string> _roomNameCache = new Dictionary<Vector3Int, string>(128);

		public BaseProximityTrigger<THandle> Parent { get; }

		public int ChannelCount => _handles.Count;

		protected Grid(BaseProximityTrigger<THandle> parent)
		{
			Parent = parent;
		}

		public void Update(IDissonancePlayer player)
		{
			_player = player;
			Bounds bounds = new Bounds(player.Position, new Vector3(Parent.Range, Parent.Range, Parent.Range) * 2f);
			Vector3Int minPosition = CellPos(bounds.min);
			Vector3Int maxPosition = CellPos(bounds.max);
			BoundsInt bounds2 = default;
			bounds2.SetMinMax(minPosition, maxPosition);
			for (int num = _handles.Count - 1; num >= 0; num--)
			{
				(Vector3Int, THandle) tuple = _handles[num];
				Vector3Int item = tuple.Item1;
				THandle item2 = tuple.Item2;
				Vector3Int vector3Int = item;
				THandle handle = item2;
				if (!InBound(vector3Int, bounds2))
				{
					CloseHandle(handle);
					_handles.RemoveAt(num);
					_keys.Remove(vector3Int);
				}
			}
			for (int i = minPosition.x; i <= maxPosition.x; i++)
			{
				for (int j = minPosition.y; j <= maxPosition.y; j++)
				{
					for (int k = minPosition.z; k <= maxPosition.z; k++)
					{
						Vector3Int vector3Int2 = new Vector3Int(i, j, k);
						if (Parent.AllowJoin(vector3Int2) && _keys.Add(vector3Int2))
						{
							THandle item3 = CreateHandle(vector3Int2, GenerateName(vector3Int2));
							_handles.Add((vector3Int2, item3));
						}
					}
				}
			}
		}

		private static bool InBound(Vector3Int point, BoundsInt bounds)
		{
			if (point.x >= bounds.xMin && point.x <= bounds.xMax && point.y >= bounds.yMin && point.y <= bounds.yMax && point.z >= bounds.zMin)
			{
				return point.z <= bounds.zMax;
			}
			return false;
		}

		public void CloseAll()
		{
			foreach (var handle in _handles)
			{
				CloseHandle(handle.Item2);
			}
			_handles.Clear();
			_keys.Clear();
		}

		protected abstract THandle CreateHandle(Vector3Int id, string name);

		protected abstract void CloseHandle(THandle handle);

		private Vector3Int CellPos(Vector3 pos)
		{
			return pos.Quantise(Parent.Size);
		}

		private string GenerateName(Vector3Int pos)
		{
			if (!_roomNameCache.TryGetValue(pos, out var value))
			{
				if (_roomNameCache.Count >= 128)
				{
					_roomNameCache.Clear();
				}
				_nameBuilder.Clear();
				_nameBuilder.EnsureCapacity(Parent.RoomName.Length + 50);
				_nameBuilder.Append(Parent.RoomName);
				_nameBuilder.Append(" {X:");
				_nameBuilder.Append(pos.x);
				_nameBuilder.Append(",Y:");
				_nameBuilder.Append(pos.y);
				_nameBuilder.Append(",Z:");
				_nameBuilder.Append(pos.z);
				_nameBuilder.Append(",R:");
				_nameBuilder.Append(Parent.Range);
				_nameBuilder.Append("}");
				value = _nameBuilder.ToString();
				_roomNameCache[pos] = value;
			}
			return value;
		}

		public void DrawGizmos()
		{
			if (_player == null)
			{
				return;
			}
			Color color = new Color(0.3f, 0.3f, 0.95f);
			Color fill = new Color(color.r, color.g, color.b, 0.05f);
			Gizmos.color = color;
			Gizmos.DrawWireSphere(_player.Position, Parent.Range);
			foreach (Vector3Int key in _keys)
			{
				Vector3 vector = (Vector3)key * Parent.Size;
				Vector3 max = vector + new Vector3(Parent.Size, Parent.Size, Parent.Size);
				DrawCube(vector, max, color, fill);
			}
		}

		private static void DrawCube(Vector3 min, Vector3 max, Color lines, Color fill)
		{
			Vector3 center = (min + max) / 2f + new Vector3(0f, 0.001f, 0f);
			Vector3 size = max - min;
			Gizmos.color = lines;
			Gizmos.DrawWireCube(center, size);
			Gizmos.color = fill;
			Gizmos.DrawCube(center, size);
		}
	}

	[SerializeField]
	[Range(1f, 100f)]
	[Tooltip("Radius of proximity chat.")]
	private int _range = 10;

	[SerializeField]
	private string _roomName = "GridProximityChat";

	[SerializeField]
	private bool _useTrigger;

	private VoicePlayerState _self;

	private Grid _grid;

	private float Size => _range * 2;

	public int Range
	{
		get
		{
			return _range;
		}
		set
		{
			if (_range != value)
			{
				_range = value;
				CloseChannels();
			}
		}
	}

	public string RoomName
	{
		get
		{
			return _roomName;
		}
		set
		{
			_roomName = value;
			CloseChannels();
		}
	}

	public override bool CanTrigger
	{
		get
		{
			if (!base.Comms || !base.Comms.IsStarted)
			{
				return false;
			}
			if (_roomName == null)
			{
				return false;
			}
			if (_self == null || !_self.IsConnected)
			{
				return false;
			}
			IDissonancePlayer dissonancePlayer = _self?.Tracker;
			if (dissonancePlayer == null || !dissonancePlayer.IsTracking)
			{
				return false;
			}
			if (UseColliderTrigger && !base.IsColliderTriggered)
			{
				return false;
			}
			return true;
		}
	}

	public override bool UseColliderTrigger
	{
		get
		{
			return _useTrigger;
		}
		set
		{
			_useTrigger = value;
		}
	}

	protected int ActiveChannelCount => _grid?.ChannelCount ?? 0;

	protected abstract Grid CreateGrid();

	private void OnValidate()
	{
		CloseChannels();
		List<VoiceProximityBroadcastTrigger> list = (from a in Object.FindObjectsOfType<VoiceProximityBroadcastTrigger>()
			where a.RoomName == RoomName
			select a).ToList();
		List<VoiceProximityReceiptTrigger> list2 = (from a in Object.FindObjectsOfType<VoiceProximityReceiptTrigger>()
			where a.RoomName == RoomName
			select a).ToList();
		foreach (VoiceProximityBroadcastTrigger item in list)
		{
			item.Range = Range;
		}
		foreach (VoiceProximityReceiptTrigger item2 in list2)
		{
			item2.Range = Range;
		}
	}

	protected override void Start()
	{
		_grid = CreateGrid();
		base.Start();
	}

	protected override void OnDisable()
	{
		CloseChannels();
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		CloseChannels();
		base.OnDestroy();
	}

	protected void CloseChannels()
	{
		_grid?.CloseAll();
	}

	internal bool AllowJoin(Vector3Int id)
	{
		return AllowJoin(_self, id);
	}

	protected virtual bool AllowJoin(VoicePlayerState player, Vector3Int id)
	{
		return true;
	}

	protected Bounds GetCellBounds(Vector3Int id)
	{
		Vector3 vector = (Vector3)id * Size;
		Vector3 max = vector + new Vector3(Size, Size, Size);
		Bounds result = default;
		result.SetMinMax(vector, max);
		return result;
	}

	protected override void Update()
	{
		if (!CheckVoiceComm())
		{
			CloseChannels();
			return;
		}
		if (_self == null || _self.Name != base.Comms.LocalPlayerName)
		{
			_self = base.Comms.FindPlayer(base.Comms.LocalPlayerName);
		}
		if (!CanTrigger)
		{
			CloseChannels();
			return;
		}
		if (!base.TokenActivationState)
		{
			CloseChannels();
			return;
		}
		IDissonancePlayer dissonancePlayer = _self?.Tracker;
		if (dissonancePlayer == null || !dissonancePlayer.IsTracking)
		{
			CloseChannels();
			return;
		}
		if (!IsUserActivated())
		{
			CloseChannels();
			return;
		}
		_grid.Update(dissonancePlayer);
		base.Update();
	}

	protected virtual bool IsUserActivated()
	{
		return true;
	}

	public void OnDrawGizmosSelected()
	{
		_grid?.DrawGizmos();
	}
}
