using CGJ2025.SceneCell;
using CGJ2025.System.Grid;
using CGJ2025.System.Interact;
using RuGameFramework;
using RuGameFramework.Core;
using RuGameFramework.Input;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CGJ2025.Scene
{
	public class GameScene : MonoBehaviour
	{
		public GridSystem gridSystem;
		public MouseManager mouseManager;
		public TimerManager timerManager;
		public InteractSystem interactSystem;
		public Transform map;
		public Tilemap tilemap;
		public Grid grid;

		void Start ()
		{
			Initialize();
		}
		

		private void Initialize()
		{
			if (timerManager == null)
			{
				timerManager = App.Instance.TimerManager;
			}

			if (mouseManager == null)
			{
				mouseManager = App.Instance.MouseManager;
			}


			gridSystem = gameObject.GetComponent<GridSystem>();
			if (gridSystem == null)
			{
				gridSystem = gameObject.AddComponent<GridSystem>();
			}

			var cellList = tilemap.GetComponentsInChildren<Cell>();
			gridSystem.Initialize(5, 5, grid, tilemap, cellList);


			interactSystem = gameObject.GetComponent<InteractSystem>();
			if (interactSystem == null)
			{
				interactSystem = gameObject.AddComponent<InteractSystem>();
			}
			interactSystem.Initialize(mouseManager, timerManager);
		}

		public void Update ()
		{
			if(App.IsComplete)return;
			interactSystem.DragUpdate();

			var cell = GridSystem.GetCell(mouseManager.WorldPosition + (Vector3)(Vector2.down * new Vector2(0, 4)));
			if(cell == null || cell.cellData.cellType == CellType.NotInteract)
			{
				App.Instance.HideCountTip();
			}else
			{
				App.Instance.ShowCountTip(cell);
			}
		}
	}

}
