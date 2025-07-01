using CGJ2025.System.Grass;
using CGJ2025.System.Grid;
using System.Collections.Generic;
using UnityEngine;
using RuGameFramework;
using RuGameFramework.Input;
using CGJ2025.Character;
using System;
using RuGameFramework.Event;
using CGJ2025.System;

namespace CGJ2025.SceneCell
{
	public class Cell : MonoBehaviour
	{
		private Vector2Int _gridIndex;
		public Vector2Int GridIndex
		{
			get
			{
				return _gridIndex;
			}
		}

		public GrassSystem grassSystem;
		public List<GameObject> plantList;
		public MouseManager mouseManager;

		public SpriteRenderer sprite;

		public CellData cellData = new CellData();
		public ICharacter character;

		private Collider2D m_collider2D;

		public Action OnRepel;

		public int rabbitCnt;
		public int flowerCnt;

		public void Awake()
		{

			
		}

		public void Initialize(Vector2Int gridIndex)
		{
			this._gridIndex = gridIndex;

			this.gameObject.name = $"{this.gameObject.name}_{GridIndex}";

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

			if(sprite == null)
			{
				sprite = GetComponent<SpriteRenderer>();
			}
			
			grassSystem._mouseManager = mouseManager;

			m_collider2D = GetComponent<Collider2D>();
			grassSystem.OnStartRepel += OnGrassRepel;
			grassSystem.OnEndRepel += OnGrassRepelEnd;
		}

		void OnDestroy()
		{
			grassSystem.OnStartRepel -= OnGrassRepel;
			grassSystem.OnEndRepel -= OnGrassRepelEnd;
		}

		public void OnGrassRepel()
		{
			if(character != null)
			{
				character.OnCellMouseDown(this);
			}
		}

		public void OnGrassRepelEnd()
		{
			if(character != null)
			{
				character.IsGrassRepel = true;
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

		public void OnCharacterInCell(ICharacter character)
		{
			if(cellData.cellType != CellType.NotInteract && sprite.enabled == false)
			{
				sprite.enabled = true;
			}
		}

		public void OnCharacterDroped(ICharacter character, Action onDroped)
		{
			if(cellData.cellType == CellType.Empty)
			{
				CreateGrass();
				return;
			}
			if(cellData.cellType != CellType.NotInteract)
			{
				if(character is Rabbit)
				{
					rabbitCnt++;
				}
				else
				{
					flowerCnt++;
				}
			}
			
			onDroped?.Invoke();
		}

		public void RefreshCell()
		{
			if(cellData.cellType != CellType.NotInteract && sprite.enabled == true)
			{
				sprite.enabled = false;
			}
		}

		public void CreateGrass()
		{
			cellData.cellType = CellType.Grass;
			grassSystem.GenerateGrass();
			var args = new GridEventArgs()
			{
				grassCell = this,
				genIndex = GridIndex
			};

			EventManager.InvokeEvent(GridSystem.EVENT_GRASS_CREADED, args);

			EventManager.InvokeEvent(GridSystem.EVENT_ADD_GEN_GRASS, args);
		}

		public void AddCharacter(GameObject character, Action callback = null)
		{
			this.character = character.GetComponent<ICharacter>();
			this.character.GenCell = this;
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

		public void OnCharacterRemove()
		{
			character = null;
			EventManager.InvokeEvent(GridSystem.EVENT_ADD_GEN_GRASS, new GridEventArgs()
			{
				genIndex = GridIndex
			});
		}

		public void Shake(Action onShakeEnd)
		{
			if(cellData.cellType != CellType.Grass)
			{
				return;
			}

			int random = UnityEngine.Random.Range(1, 3);

			grassSystem.StartShaking(random, onShakeEnd);
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
