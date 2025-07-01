using System.Collections;
using System.Collections.Generic;
using CGJ2025.SceneCell;
using CGJ2025.System.Grid;
using CGJ2025.System.Interact;
using RuGameFramework;
using RuGameFramework.AnimeStateMachine;
using RuGameFramework.Core;
using RuGameFramework.Input;
using UnityEngine;


namespace CGJ2025.Character
{
	public class Flower : Character.Character<Flower.State>
	{
		public enum State
		{
			Start,
			Catch,
			Catching,
			CatchSuccess,
			CatchFail,
			DropSuccess,
			DropFail
		}

		public State state = State.Start;

		public float needCatchTime;
		public float increaseDoubleGenValue;

		[SerializeField] private MouseManager _mouseManager;

		public AudioClip audioFind;
		public AudioClip audioFind2;
		public AudioClip audioCatch;
		
		public AudioClip audioCatch2;
		public AudioClip audioDropSccess;
		public AudioClip audioDropSccess2;
		public AudioClip audioDropFail;

		public AudioClip audioCatchSuccess;
		public AudioClip audioCatchCharacter;


		public override bool IsGrassRepel 
		{
			get; set;
		}

		protected override void OnStart()
        {
            if(_mouseManager == null)
			{
				_mouseManager = App.Instance.MouseManager;
			}
        }

		protected override void OnUpdate()
		{
			if(state == State.DropFail)
			{
				FollowDragPoint(_mouseManager.WorldPosition);
			}
		}

		protected override void InitializeSkeleton()
		{
			skeletonMchine.RegisterState(SkeletonLayer.Base, "In", false)
			.OnAnimationStart((_, _) => 
			{
				state = State.Start;
				PlayGrassAudio(audioGen);
			})
			.AddAnoTransition("Find", () => state == State.Catching)
			.AddAnoTransition("Idle", ()=> true, true);

			// 待机
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Idle", true)
			.AddAnoTransition("Find", () => state == State.Catching);

			// 拾取开始
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Find", false)
			.OnAnimationStart((_, _) => 
			{
				var random = UnityEngine.Random.Range(0, 2);
				if (random == 0) PlayAudio(audioFind); 
				else PlayAudio(audioFind2);
			})
			.AddAnoTransition("Snatch_in", () => state == State.CatchSuccess)
			.AddAnoTransition("Find_cancel", () => state == State.CatchFail);

			// 拾取失败
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Find_cancel")
			.OnAnimationStart((_, _) => gameSortLayer.SortLayerName = LAYER_INTERACTABLE)
			.AddAnoTransition("Idle", () => true, true);

			// 拾取成功 挣扎
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch_in")
			.OnAnimationStart((_, _) => {
				var random = UnityEngine.Random.Range(0, 2);
				if (random == 0) PlayAudio(audioCatch); 
				else PlayAudio(audioCatch2);

				Cursor.visible = false;
			})
			.AddAnoTransition("Snatch", () => true, true)
			.AddAnoTransition("Drop", () => state == State.DropSuccess, true)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			// 挣扎中
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch", true)
			.OnAnimationStart((_, _) => {
				Cursor.visible = false;
			})
			.AddAnoTransition("Drop", () => state == State.DropSuccess)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			// 成功释放
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Drop")
			.OnAnimationStart((_, _) => 
			{
				Cursor.visible = true;

				var random = UnityEngine.Random.Range(0, 2);
				if (random == 0) PlayAudio(audioDropSccess); 
				else PlayAudio(audioDropSccess2);


				gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
			})
			.OnAnimationEnd((_, _) => 
			{
				Cursor.visible = true;
				OnCharacterDroped();
			})
			.OnAnimationComplate((_, _) => 
			{

				Cursor.visible = true;
				OnCharacterDroped();
			});

			// 释放失败
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong_in")
			.OnAnimationStart((_, _) => {
				PlayAudio(audioDropFail);
				Cursor.visible = false;
			})
			.AddAnoTransition("Wrong", () => true, true);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong", true)
			.OnAnimationStart((_, _) => {
				Cursor.visible = false;
				gameSortLayer.SortLayerName = LAYER_HOVER;
			})
			.AddAnoTransition("Drop", () => dropCell != null && _isMouseHold)
			.AddAnoTransition("Snatch_in", () => state == State.CatchSuccess);
			
			skeletonMchine.SetDefault("In");
			skeletonMchine.StartMachine();
		}

		protected override void DropedEffect()
		{
			GridSystem.doubleGenrate = Mathf.Max(0.8f, GridSystem.doubleGenrate + increaseDoubleGenValue);
		}

		protected override void OnCharacterDroped()
		{
			base.OnCharacterDroped();
			
			Destroy(this.gameObject);
		}

		private bool _isMouseHold = false;
		public override void OnDragBegin(InteractContext context)
		{
			_isMouseHold = true;
			if(state == State.DropFail)
			{
				Cursor.visible = false;
				state = State.CatchSuccess;
				gameSortLayer.SortLayerName = LAYER_HOVER;
				return;
			}

			if(!IsGrassRepel)
			{
				return;
			}

			state = State.Catching;
			PlayOneShot(audioCatchCharacter);
			gameSortLayer.SortLayerName = LAYER_HOVER;
		}

		public Cell currentCell;
		 public override void OnCellMouseDown(Cell cell)
        {

		}

		protected override bool DragCondition(InteractContext context) 
		{
			return state == State.CatchSuccess;
		}

		public override void OnDragUpdate(InteractContext context)
		{
			if(context.dragTime >= needCatchTime && state == State.Catching)
			{
				state = State.CatchSuccess;
				if(GenCell != null)
				{
					GenCell.OnCharacterRemove();
					GenCell = null;
				}
			}

			base.OnDragUpdate(context);
		}

		public override void OnDragEnd(InteractContext context)
		{
			_isMouseHold = false;
			if(state == State.Catching)
			{
				if(context.dragTime < needCatchTime)
				{
					state = State.CatchFail;
					return;
				}
				return;
			}

			PlayOneShot(audioCatchSuccess);
			if(state == State.CatchSuccess)
			{
				state = dropCell == null || dropCell.GridIndex == GridSystem.HeadStone ? State.DropFail : State.DropSuccess; 
			}
			gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
		}

        protected override void InitializeFSM()
		{

		}
    }


}
