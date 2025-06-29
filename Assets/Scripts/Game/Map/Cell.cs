using CGJ2025.System.Grass;
using CGJ2025.System.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuGameFramework;
using RuGameFramework.Input;

namespace CGJ2025.SceneCell
{
	public class Cell : MonoBehaviour
	{
		public Vector2Int gridIndex;
		public GrassSystem grassSystem;
		public List<GameObject> plantList;
		public MouseManager mouseManager;

		private Collider2D m_collider2D;


		public void Start()
		{
			gridIndex = GameObject.Find("Scene").GetComponent<GridSystem>().WorldToCell(this.transform.position);

			grassSystem = GetComponentInChildren<GrassSystem>();
			
			if (App.Instance != null)
				mouseManager = App.Instance.MouseManager;
				
            if (mouseManager == null)
			{
				mouseManager = FindObjectOfType<MouseManager>();
			}


			if (grassSystem == null)
			{
				grassSystem = gameObject.AddComponent<GrassSystem>();
			}
			
			grassSystem._mouseManager = mouseManager;
			m_collider2D = GetComponent<Collider2D>();
		}

		void Update()
		{
			if (mouseManager._mouseData.LeftState == RuGameFramework.Input.MouseButtonState.Down &&
			mouseManager._mouseData.collider == m_collider2D)
			{
				grassSystem.TryStartRepel();
			}
			else
			{
				grassSystem.TryEndRepel();
			}
           
        }
    }

}
