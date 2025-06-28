using System.Collections;
using System.Collections.Generic;
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
	public abstract class Character<T> : MonoBehaviour, IInteractable
	{
		public const string LAYER_INTERACTABLE = "Interactable";
		public const string LAYER_HOVER = "Hover";

		private const string DRAG_POINT = "Dragpoint";

		public TimerManager timerManager;

		public ScriptableCharacter characterData;
		public SkeletonAnimation skeletonAnimation;
		public SkeletonStateMachine skeletonMchine;
		public Renderer spineRenderer;

		public RuFSM<T> fSM;

		public BoxCollider2D collider;
		
		private float[] _vertexBuffer = new float[8];
		public Bone dragBone;
		
		public GameSortLayerCom gameSortLayer;

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
		}

		void Update() {
			OnUpdate();
			skeletonMchine.UpdateMachine();
			fSM.UpdateMachine();
		}

		protected virtual void OnUpdate(){}

		void LateUpdate() {
			UpdateCollider(skeletonAnimation);
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
		public virtual void OnCellMouseDown() {}

		public virtual void OnDragBegin(InteractContext context)
		{
			// if(!characterData.dragCondition.CanDrag(context))
			// {
			// 	return;
			// }
			if(!DragCondition(context))
			{
				return;
			}
			
			gameSortLayer.SortLayerName = LAYER_HOVER;
		}

		protected virtual bool DragCondition(InteractContext context) {return true;}

		public virtual void OnDragEnd(InteractContext context)
		{
			gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
			Debug.Log("OnDragEnd");
		}

		public virtual void OnDragUpdate(InteractContext context)
		{
			if(!DragCondition(context))
			{
				return;
			}
			
			FollowDragPoint(context.mousePosition);
			
		}

		public void FollowDragPoint(Vector2 mousePosition)
		{
			Vector2 boneWorldPos = skeletonAnimation.transform.TransformPoint(new Vector2(dragBone.WorldX, dragBone.WorldY));
			Vector3 offset = mousePosition - boneWorldPos;

			transform.position += offset;
		}
    }


}
