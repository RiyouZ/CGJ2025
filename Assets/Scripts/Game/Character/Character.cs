using System.Collections;
using System.Collections.Generic;
using CGJ2025.System.Interact;
using RuGameFramework.AnimeStateMachine;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace CGJ2025.Character
{
	public class Character : MonoBehaviour, IInteractable
	{
		private const string DRAG_POINT = "Dragpoint";
		public ScriptableCharacter characterData;
		public SkeletonAnimation skeletonAnimation;
		public SkeletonStateMachine skeletonMchine;
		public Renderer spineRenderer;

		public BoxCollider2D collider;
		
		private float[] _vertexBuffer = new float[8];
		public Bone dragBone;
		private Vector2 _dragOffset;

		private void Start() {
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

			
		}

		void LateUpdate() {
			UpdateCollider(skeletonAnimation);
		}

		private void InitializeSkeleton()
		{
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch");
			skeletonMchine.SetDefault("Snatch");
			skeletonMchine.StartMachine();

		}

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

		public void OnDragBegin(InteractContext context)
		{
			Vector2 boneWorldPos = skeletonAnimation.transform.TransformPoint(new Vector2(dragBone.WorldX, dragBone.WorldY));
			_dragOffset = transform.position - (Vector3)(context.mousePosition - boneWorldPos);
			Debug.Log("OnDragBegin");
		}

		public void OnDragEnd(InteractContext context)
		{
			Debug.Log("OnDragEnd");
		}

		public void OnDragUpdate(InteractContext context)
		{
			Vector2 boneWorldPos = skeletonAnimation.transform.TransformPoint(new Vector2(dragBone.WorldX, dragBone.WorldY));
			Vector3 offset = context.mousePosition - boneWorldPos;

			transform.position += offset;
		}

		
	}


}
