using CGJ2025.SceneCell;
using CGJ2025.System.Grid;
using CGJ2025.System.Interact;
using RuGameFramework.Input;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;


namespace CGJ2025.Scene
{
	public class GameScene : MonoBehaviour
	{
		public GridSystem gridSystem;
		public MouseManager mouseManager;
		public InteractSystem interactSystem;

		void Start ()
		{
			Initialize();
		}

		private void Initialize ()
		{
			gridSystem = gameObject.GetComponent<GridSystem>();
			if (gridSystem == null)
			{
				gridSystem = gameObject.AddComponent<GridSystem>();
			}

			interactSystem = gameObject.GetComponent<InteractSystem>();
			if(interactSystem == null)
			{
				interactSystem = gameObject.AddComponent<InteractSystem>();
			}

			mouseManager = gameObject.GetComponent<MouseManager>();
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
