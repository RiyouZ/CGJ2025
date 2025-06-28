using CGJ2025.System.Grass;
using CGJ2025.System.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGJ2025.SceneCell
{
	public class Cell : MonoBehaviour
	{
		public Vector2Int gridIndex;
		public GrassSystem grassSystem;
		public List<GameObject> plantList;

		public void Start ()
		{
			gridIndex = GameObject.Find("Scene").GetComponent<GridSystem>().WorldToCell(this.transform.position);

			grassSystem = GetComponentInChildren<GrassSystem>();
			if(grassSystem == null)
			{
				grassSystem = gameObject.AddComponent<GrassSystem>();
			}
		}

		
	}

}
