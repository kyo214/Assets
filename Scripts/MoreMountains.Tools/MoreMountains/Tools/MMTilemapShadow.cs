using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools;

[ExecuteAlways]
[AddComponentMenu("More Mountains/Tools/Tilemaps/MMTilemapShadow")]
[RequireComponent(typeof(Tilemap))]
public class MMTilemapShadow : MonoBehaviour
{
	public Tilemap ReferenceTilemap;

	[MMInspectorButton("UpdateShadows")]
	public bool UpdateShadowButton;

	protected Tilemap _tilemap;

	public virtual void UpdateShadows()
	{
		if (!(ReferenceTilemap == null))
		{
			_tilemap = base.gameObject.GetComponent<Tilemap>();
			Copy(ReferenceTilemap, _tilemap);
		}
	}

	public static void Copy(Tilemap source, Tilemap destination)
	{
		source.RefreshAllTiles();
		destination.RefreshAllTiles();
		List<Vector3Int> list = new List<Vector3Int>();
		foreach (Vector3Int item in source.cellBounds.allPositionsWithin)
		{
			Vector3Int vector3Int = new Vector3Int(item.x, item.y, item.z);
			if (source.HasTile(vector3Int))
			{
				list.Add(vector3Int);
			}
		}
		Vector3Int[] array = new Vector3Int[list.Count];
		TileBase[] array2 = new TileBase[list.Count];
		int num = 0;
		foreach (Vector3Int item2 in list)
		{
			array2[num] = source.GetTile(array[num] = item2);
			num++;
		}
		destination.ClearAllTiles();
		destination.RefreshAllTiles();
		destination.size = source.size;
		destination.origin = source.origin;
		destination.ResizeBounds();
		destination.SetTiles(array, array2);
	}
}
