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
			gridSystem.grid = GameObject.FindObjectOfType<Grid>();
			gridSystem.tilemap = FindObjectOfType<Tilemap>();

			var cellList = map.GetComponentsInChildren<Cell>();
			gridSystem.Initialize(5, 5, cellList);

			interactSystem = gameObject.GetComponent<InteractSystem>();
			if (interactSystem == null)
			{
				interactSystem = gameObject.AddComponent<InteractSystem>();
			}
			interactSystem.Initialize(mouseManager, timerManager);
		}

		public void Update ()
		{
			MouseSelectUpdate();
			interactSystem.DragUpdate();
		}

		private void MouseSelectUpdate ()
		{
			var selectObject = mouseManager.CurrentObject;
			if (selectObject == null)
			{
				return;
			}
		}

	}

}
