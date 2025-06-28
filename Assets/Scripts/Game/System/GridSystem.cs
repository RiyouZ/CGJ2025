using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace CGJ2025.System.Grid
{
    public class GridSystem : MonoBehaviour
	{
		public UnityEngine.Grid grid;
		public Tilemap tilemap;
		public Tile sampleTile;

		public SceneCell.Cell[ , ] groundObj;
		public CellData[ , ] groundData;

		private Vector3Int originPos;

		private float _width;
		private float _height;

		public void Initialize (int row, int col, List<SceneCell.Cell> cellList)
		{
			groundData = new CellData[row, col];
			groundObj = new SceneCell.Cell[ row, col];

			foreach (SceneCell.Cell cell in cellList)
			{
				var index = cell.gridIndex;
				groundObj[index.x, index.y] = cell;
			}

			originPos = tilemap.cellBounds.min;

			_width = tilemap.cellBounds.size.x;
			_height = tilemap.cellBounds.size.y;
		}

		public Vector3 CalcultorCellSize ()
		{
			if (sampleTile.sprite == null)
			{
				return Vector3.zero;
			}

			Vector2 size = sampleTile.sprite.bounds.size;
			grid.cellSize = new Vector3(size.x, size.y, 1f);
			return grid.cellSize;
		}

		public Vector2Int TileIndexToGroundIndex (Vector3Int pos)
		{
			return new Vector2Int(pos.x - originPos.x, pos.y - originPos.y);
		}

		public Vector3Int GroundIndexToTileIndex (Vector3Int pos)
		{
			if (pos.x < 0 || pos.x >= _width || pos.y < 0 || pos.y >= _height)
			{
				return Vector3Int.zero;
			}

			return new Vector3Int(originPos.x + pos.x, originPos.y + pos.y, 0);
		}

		public Vector2Int WorldToCell (Vector3 worldPos)
		{
			var tilePos = tilemap.WorldToCell(worldPos);
			return TileIndexToGroundIndex(tilePos);
		}

		public Vector3 CellToWorld (Vector3Int cellPos)
		{
			var tileIndex = GroundIndexToTileIndex(cellPos);
			return tilemap.CellToWorld(tileIndex);
		}


#if UNITY_EDITOR
		[Button]
		public void SetOrigin ()
		{
			tilemap.transform.position = -tilemap.CellToWorld(new Vector3Int(tilemap.cellBounds.min.x, tilemap.cellBounds.min.y, 0));
		}

		[Button]
		public void SettingTilemap ()
		{
			var size = CalcultorCellSize();
			Debug.Log($"[Grid.Tilemap] Size {size}");
		}

#endif

	}

}
