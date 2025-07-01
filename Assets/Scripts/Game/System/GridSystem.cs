using CGJ2025.Character;
using CGJ2025.SceneCell;
using RuGameFramework;
using RuGameFramework.Assets;
using RuGameFramework.Core;
using RuGameFramework.Event;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CGJ2025.System.Grid
{
    public class GridSystem : MonoBehaviour
	{
		public const string EVENT_GRASS_CREADED = "GrassCreated";
		public const string EVENT_ADD_GEN_GRASS = "AddGenrateGrass";

		public readonly static Vector2Int HeadStone = new Vector2Int(2, 2);
		public UnityEngine.Grid grid;
		public Tile sampleTile;

		public static Tilemap Tilemap;
		private static Vector3Int OriginPos;

		private static float Width;
		private static float Height;

		public static float GenerateTime = 8f;
		public static float doubleGenrate = 0.00f;

		private static int row;
		private static int col;


		public static SceneCell.Cell[ , ] groundObj;

		public bool[ , ] grassExsit;
		private readonly static int[ , ] RangeDir = new int[,]
		{
			{-1, -1, 0, 1, 1, 1, 0, -1},
			{0, 1, 1, 1, 0, -1, -1, -1}
		};


		public AudioSource audioSource;


		private static List<int> grassCellList;

		private void Reset()
		{
			GenerateTime = 8;
			doubleGenrate = 0.00f;
			lastTime = 5;
			for (int x = 0; x < row; x++)
			{
				for (int y = 0; y < col; y++)
				{
					grassExsit[x, y] = false;
				}
			}
		}

		public void Initialize (int row, int col, UnityEngine.Grid grid, Tilemap tilemap, SceneCell.Cell[] cellList)
		{
			GridSystem.row = row;
			GridSystem.col = col;
			this.grid = grid;
			GridSystem.Tilemap = tilemap;
			groundObj = new SceneCell.Cell[row, col];
			grassCellList = new List<int>(row * col);
			grassExsit = new bool[row, col]; 

			

			foreach (SceneCell.Cell cell in cellList)
			{
				cell.Initialize(WorldToCell(cell.transform.position));
				var index = cell.GridIndex;
				// Debug.Log($"[Initialize] {index}");
				groundObj[index.x, index.y] = cell;
				grassExsit[index.x, index.y] = false;
			}

			OriginPos = Tilemap.cellBounds.min;

			Width = Tilemap.cellBounds.size.x;
			Height = Tilemap.cellBounds.size.y;

			groundObj[0, 0].cellData.cellType = CellType.Grass;
			grassExsit[0, 0] = true;
			groundObj[HeadStone.x, HeadStone.y].cellData.cellType = CellType.NotInteract;

			grassCellList.Add(Index(0, 0));
			groundObj[0, 0].CreateGrass();

			audioSource = GetComponent<AudioSource>();
			lastTime = 7f;
			InitializeEvent();
		}

		private void InitializeEvent()
		{
			EventManager.AddListener(EVENT_GRASS_CREADED, OnGrassCreate);

			EventManager.AddListener(EVENT_ADD_GEN_GRASS, OnGenrateGrassAdd);
		}

		void OnDestroy()
		{
			EventManager.RemoveListener(EVENT_GRASS_CREADED, OnGrassCreate);
			EventManager.RemoveListener(EVENT_ADD_GEN_GRASS, OnGenrateGrassAdd);
		}

		private void OnGenrateGrassAdd(IGameEventArgs eventArgs)
		{
			var args = eventArgs as GridEventArgs;
			var index = args.genIndex;
			grassCellList.Add(Index(index.x, index.y));
		}

		Timer timerGen;
		Timer timerComplete;
		private void OnGrassCreate(IGameEventArgs eventArgs)
		{
			var args = eventArgs as GridEventArgs;
			var cell = args.grassCell;
			if(cell == null)
			{
				return;
			}

			if(!HeadStoneGrassAllExist(cell.GridIndex))
			{	
				return;
			};

			var headstoneCell = groundObj[HeadStone.x, HeadStone.y];

			headstoneCell.CreateGrass();
			StartCoroutine(AsyncComplete());
		}

		private IEnumerator AsyncComplete()
		{
			yield return new WaitForSeconds(2);
			App.Instance.Complete();
			Reset();
		}

		private bool HeadStoneGrassAllExist(Vector2Int index)
		{
			if(grassExsit[index.x, index.y])
			{
				return false;
			}

			grassExsit[index.x, index.y] = true;
			// for(int i = 0; i < 8; i++)
			// {
			// 	int dx = HeadStone.x + RangeDir[0, i];
			// 	int dy = HeadStone.y + RangeDir[1, i];

			// 	if(dx < 0 || dx >= row ||dy < 0 || dy >= col)
			// 	{
			// 		continue;
			// 	}

			// 	if(!grassExsit[dx, dy]) return false;;
			// }

			//	All Grass Exist but headstone
			for (int i = 0; i < row; i++)
			{
				for (int j = 0; j < col; j++)
				{
					if (i == HeadStone.x && j == HeadStone.y) continue;
					if (!grassExsit[i, j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int Index(int x, int y)
		{
			return y * col + x;
		}

		public Vector2Int Index(int index)
		{
			return new Vector2Int(index % col, index / col);
		}

		private float lastTime = 7f;
		void Update()
		{
			lastTime += Time.deltaTime;
			if(lastTime > GenerateTime && !App.IsComplete)
			{
				float doubleRandom = Random.Range(0, 1f);
				if(doubleRandom < doubleGenrate)
				{
					GenrateCharacter();
					GenrateCharacter();
				}
				else
				{
					GenrateCharacter();
				}

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
			if (cell.character != null)
			{
				return;
			}

			cell.Shake(()=>
			{
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
			});

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

		public static Vector2Int TileIndexToGroundIndex (Vector3Int pos)
		{
			var index = new Vector2Int(pos.x - OriginPos.x, pos.y - OriginPos.y);
			if (index.x < 0 || index.x >= row || index.y < 0 || index.y >= col)
				return new Vector2Int(-1, -1);
			
			return index;
		}
		public static Vector3Int GroundIndexToTileIndex (Vector3Int pos)
		{
			if (pos.x < 0 || pos.x >= Width || pos.y < 0 || pos.y >= Height)
			{
				return new Vector3Int(-1, -1);
			}

			return new Vector3Int(OriginPos.x + pos.x, OriginPos.y + pos.y, 0);
		}

		public static Vector2Int WorldToCell (Vector3 worldPos)
		{
			var tilePos = Tilemap.WorldToCell(worldPos);
			return TileIndexToGroundIndex(tilePos);
		}

		public static Vector3 CellToWorld (Vector3Int cellPos)
		{
			var tileIndex = GroundIndexToTileIndex(cellPos);
			return Tilemap.CellToWorld(tileIndex);
		}

		public static Cell GetCell(Vector3 pos)
		{
			Vector2Int cellIndex = WorldToCell(pos);
			if(cellIndex == new Vector2Int(-1, -1))
			{
				return null;
			}

			var cell = groundObj[cellIndex.x, cellIndex.y];
			return cell;
		}




#if UNITY_EDITOR
		[Button]
		public void SetOrigin ()
		{
			Tilemap.transform.position = -Tilemap.CellToWorld(new Vector3Int(Tilemap.cellBounds.min.x, Tilemap.cellBounds.min.y, 0));
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
