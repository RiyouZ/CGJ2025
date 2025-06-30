using System.Collections;
using System.Collections.Generic;
using CGJ2025.SceneCell;
using CGJ2025.System.Grid;
using CGJ2025.System.Interact;
using RuGameFramework;
using RuGameFramework.AnimeStateMachine;
using RuGameFramework.Core;
using RuGameFramework.SortingLayer;
using RuGameFramework.StateMachine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace CGJ2025.Character
{
	public abstract class Character<T> : MonoBehaviour, IInteractable, ICharacter
	{
		public const string LAYER_INTERACTABLE = "Grass";
		public const string LAYER_HOVER = "Hover";

		private const string DRAG_POINT = "Dragpoint";

		public TimerManager timerManager;

		public ScriptableCharacter characterData;
		public SkeletonAnimation skeletonAnimation;
		public SkeletonStateMachine skeletonMchine;
		public Renderer spineRenderer;

		public AudioSource onshotAudio;
		public AudioSource audioSource;
		public AudioSource grassAudio;

		public AudioClip audioRepel;
		public AudioClip audioRepel1;
		public AudioClip audioGen;

		public RuFSM<T> fSM;

		public BoxCollider2D collider;
		
		private float[] _vertexBuffer = new float[8];
		public Bone dragBone;
		
		public GameSortLayerCom gameSortLayer;

		protected Cell dropCell;

		private int _skinIndex;
	
		public GameObject CharacteObject => this.gameObject;
		public GameObject InteractObject => this.gameObject;

		public virtual bool IsGrassRepel
		{
			set; get;
		}

        public Cell GenCell 
		{
			get; set; 
		}

        private void Start() 
		{
			if(timerManager == null)
			{
				timerManager = App.Instance.TimerManager;
			}

			if(skeletonAnimation == null)
			{
				skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
				skeletonAnimation.skeletonDataAsset = characterData.skeletonData;
				skeletonAnimation.Initialize(true);

				_skinIndex = UnityEngine.Random.Range(0, characterData.skinInfoList.Count);
				var skin = characterData.skinInfoList[_skinIndex].skinName;

				skeletonAnimation.skeleton.SetSkin(skin);
				skeletonAnimation.skeleton.SetSlotsToSetupPose();
				skeletonAnimation.AnimationState.Apply(skeletonAnimation.skeleton);
				spineRenderer = skeletonAnimation.GetComponent<Renderer>();
			}

			if(dragBone == null)
			{
				dragBone = skeletonAnimation.skeleton.FindBone(DRAG_POINT);
			}

			if(collider == null)
			{
				collider = GetComponent<BoxCollider2D>();
			}

			if(skeletonMchine == null)
			{
				skeletonMchine = new SkeletonStateMachine(skeletonAnimation);
				InitializeSkeleton();
			}

			if(fSM == null)
			{
				fSM = new RuFSM<T>(6);
				InitializeFSM();
			}

			if(gameSortLayer == null)
			{
				gameSortLayer = GetComponentInChildren<GameSortLayerCom>();
				gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
			}

			if(audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
			}
			OnStart();
		}

		public void PlayAudio(AudioClip clip)
		{
			audioSource.clip = clip;
			audioSource.Play();
		}

		public void PlayOneShot(AudioClip clip)
		{
			onshotAudio.clip = clip;
			onshotAudio.Play();
		}

		public void PlayGrassAudio(AudioClip clip)
		{
			grassAudio.clip = clip;
			grassAudio.Play();
		}

		protected virtual void OnStart() {}

		void Update()
		{
			OnUpdate();
			skeletonMchine.UpdateMachine();
			fSM.UpdateMachine();
		}

		protected virtual void OnUpdate(){}

		void LateUpdate() {
			UpdateCollider(skeletonAnimation);
		}

		void OnDestroy() {
			App.Instance.HideEffectTip();	
		}

		protected abstract void InitializeSkeleton();
		protected abstract void InitializeFSM();

		private void UpdateCollider(SkeletonAnimation skeleton)
		{
			// 获取 Spine 渲染边界（世界空间）
			Bounds worldBounds = spineRenderer.bounds;
			Vector2 sizeWorld = new Vector2(worldBounds.size.x, worldBounds.size.y);
			Vector2 centerWorld = worldBounds.center;

			// 将世界中心转换为当前父物体的本地空间
			Vector2 centerLocal = transform.InverseTransformPoint(centerWorld);

			collider.size = sizeWorld;
			collider.offset = centerLocal;
		}

		// 给Grid调用
		public virtual void OnCellMouseDown(Cell cell) {}

		public virtual void OnDragBegin(InteractContext context)
		{
			if(!DragCondition(context))
			{
				return;
			}

			if(GenCell != null)
			{
				GenCell.OnCharacterRemove();
				GenCell = null;
			}
		}

		protected virtual bool DragCondition(InteractContext context) {return true;}

		public virtual void OnDragEnd(InteractContext context)
		{
			gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
		}

		public virtual void OnDragUpdate(InteractContext context)
		{
			if (!DragCondition(context))
			{
				return;
			}

			FollowDragPoint(context.mousePosition);
			
			RefreshCell();
			dropCell = GridSystem.GetCell(this.transform.position + (Vector3)(Vector2.down * new Vector2(0, 4)));
			if(dropCell != null && dropCell.cellData.cellType != CellType.NotInteract)
			{
				dropCell.OnCharacterInCell(this);
			}

			if(dropCell != null && dropCell.cellData.cellType == CellType.Grass)
			{
				var skinInfo = characterData.skinInfoList[_skinIndex];
				App.Instance.ShowEffectTip(context.mousePosition, characterData.elvenName, skinInfo.tipSprite, characterData.effectDescript);
			}
		}

		protected virtual void OnCharacterDroped()
		{
			if(dropCell == null)
			{
				return;
			}

			dropCell.RefreshCell();
			dropCell.OnCharacterDroped(this, DropedEffect);
		}

		protected abstract void DropedEffect();

		public void RefreshCell()
		{
			if(dropCell == null)
			{
				App.Instance.HideEffectTip();
				return;
			}

			if(dropCell.cellData.cellType == CellType.NotInteract || dropCell.cellData.cellType == CellType.Empty)
			{
				App.Instance.HideEffectTip();
			}

			dropCell.RefreshCell();
			dropCell = null;
		}

		public void FollowDragPoint(Vector2 mousePosition)
		{
			if(skeletonAnimation == null)
			{
				return;
			}
			Vector2 boneWorldPos = skeletonAnimation.transform.TransformPoint(new Vector2(dragBone.WorldX, dragBone.WorldY));
			Vector3 offset = mousePosition - boneWorldPos;

			transform.position += offset;
		}
    }


}
