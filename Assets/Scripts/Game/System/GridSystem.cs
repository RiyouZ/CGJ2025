using CGJ2025.Character;
using RuGameFramework;
using RuGameFramework.Assets;
using RuGameFramework.Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CGJ2025.System.Grid
{
    public class GridSystem : MonoBehaviour
	{
		public UnityEngine.Grid grid;
		public Tilemap tilemap;
		public Tile sampleTile;

		public SceneCell.Cell[ , ] groundObj;

		private Vector3Int originPos;

		private float _width;
		private float _height;

		public static float GenerateTime = 5;
		public static float doubleGenrate = 0.01f;

		private static int row;
		private static int col;

		public AudioSource audioSource;


		[SerializeField] private List<int> grassCellList;

		public void Initialize (int row, int col, SceneCell.Cell[] cellList)
		{
			GridSystem.row = row;
			GridSystem.col = col;
			groundObj = new SceneCell.Cell[row, col];
			grassCellList = new List<int>(row * col);

			foreach (SceneCell.Cell cell in cellList)
			{
				var index = cell.gridIndex;
				groundObj[index.x, index.y] = cell;
				groundObj[index.x, index.y].OnRemoveCharacter = (cell)=>
				{
					grassCellList.Add(Index(cell.gridIndex.x, cell.gridIndex.y));
				};
			}

			originPos = tilemap.cellBounds.min;

			_width = tilemap.cellBounds.size.x;
			_height = tilemap.cellBounds.size.y;
		}

		void Start() 
		{
			groundObj[0, 0].cellData.cellType = CellType.Grass;
			groundObj[2, 2].cellData.cellType = CellType.NotInteract;

			grassCellList.Add(Index(0, 0));
			groundObj[0, 0].CreateGrass();

			audioSource = GetComponent<AudioSource>();
		}

		public static int Index(int x, int y)
		{
			return y * col + x;
		}

		public Vector2Int Index(int index)
		{
			return new Vector2Int(index % col, index / col);
		}

		private float lastTime;
		void Update()
		{
			lastTime += Time.deltaTime;
			if(lastTime > GenerateTime)
			{
				GenrateCharacter();
				lastTime = 0;
			}
		}	

		public void GenrateCharacter()
		{
			if(grassCellList.Count <= 0)
			{
				return;
			}

			int randomCell = UnityEngine.Random.Range(0, grassCellList.Count);
			var idx = Index(grassCellList[randomCell]);
			var cell = groundObj[idx.x, idx.y];

			if(cell.character != null)
			{
				return;
			}

			cell.Shake();

			int randomCharacter = UnityEngine.Random.Range(0, 2);
			if(randomCharacter == 0)
			{	
				cell.AddCharacter(ResManager.Instance.LoadAssets<GameObject>("Character/Rabbit/Character_Rabbit"));
			}
			else
			{
				cell.AddCharacter(ResManager.Instance.LoadAssets<GameObject>("Character/Flower/Character_Flower"));
			}

			cell.StopShake();
			

			grassCellList.Remove(grassCellList[randomCell]);
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
