using CGJ2025.System.Grass;
using CGJ2025.System.Grid;
using System.Collections.Generic;
using UnityEngine;
using RuGameFramework;
using RuGameFramework.Input;
using CGJ2025.Character;
using System;

namespace CGJ2025.SceneCell
{
	public class Cell : MonoBehaviour
	{
		public Vector2Int gridIndex;
		public GrassSystem grassSystem;
		public List<GameObject> plantList;
		public MouseManager mouseManager;

		public CellData cellData = new CellData();
		public ICharacter character;

		private Collider2D m_collider2D;

		public Action OnRepel;

		public void Awake()
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

			grassSystem.OnStartRepel += OnGrassRepel;

		}

		public void OnGrassRepel()
		{
			if(character != null)
			{
				character.OnCellMouseDown(this);
			}
		}


		void Update()
		{
			if (mouseManager._mouseData.collider == m_collider2D && mouseManager._mouseData.LeftState != RuGameFramework.Input.MouseButtonState.Down)
            {
				OnRepel?.Invoke();
                grassSystem.TryStartRepel();    
            }
            else
            {
                grassSystem.TryEndRepel();
            }
        }

		public void CreateGrass()
		{
			grassSystem.GenerateGrass();
			OnRemoveCharacter?.Invoke(this);
		}

		public void AddCharacter(GameObject character, Action callback = null)
		{
			this.character = character.GetComponent<ICharacter>();
			float randomRangeX = 3f;
			float randomRangeY = 0.8f;
			Vector3 pos;
			do
			{
				float x = UnityEngine.Random.Range(-randomRangeX, randomRangeX);
				float y = UnityEngine.Random.Range(-randomRangeY, randomRangeY);
				pos = new Vector3(x, y, 0);
			}
			while (Mathf.Abs(pos.x / randomRangeX) + Mathf.Abs(pos.y / randomRangeY) > 1);
			character.transform.position = this.transform.position + pos;
		}

		public Action<Cell> OnRemoveCharacter;

		public void RemoveCharacte()
		{
			OnRemoveCharacter?.Invoke(this);
			character = null;
		}

		public void OnCharacterDroped(Action<Cell> onDroped)
		{
			if(cellData.cellType == CellType.Empty)
			{
				CreateGrass();
				return;
			}
			onDroped?.Invoke(this);
		}

		public void Shake()
		{
			if(cellData.cellType != CellType.Grass)
			{
				return;
			}
			grassSystem.StartShaking();
		}

		public void StopShake()
		{
			if(cellData.cellType != CellType.Grass)
			{
				return;
			}
			grassSystem.StopShaking();
		}
    }

}
