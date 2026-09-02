using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Tilemaps/MMTilemapBoolean")]
public class MMTilemapBoolean : MonoBehaviour
{
	public Tilemap TilemapToClean;

	[MMInspectorButton("BooleanClean")]
	public bool BooleanCleanButton;

	protected Tilemap _tilemap;

	protected virtual void BooleanClean()
	{
		if (TilemapToClean == null)
		{
			return;
		}
		_tilemap = base.gameObject.GetComponent<Tilemap>();
		foreach (Vector3Int item in _tilemap.cellBounds.allPositionsWithin)
		{
			Vector3Int position = new Vector3Int(item.x, item.y, item.z);
			if (_tilemap.HasTile(position) && TilemapToClean.HasTile(position))
			{
				TilemapToClean.SetTile(position, null);
			}
		}
		_tilemap.RefreshAllTiles();
	}
}
