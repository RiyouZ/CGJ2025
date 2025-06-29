using System.Collections;
using System.Collections.Generic;
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
		
		private Timer catchTimer;

		[SerializeField] private MouseManager _mouseManager;

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
			.AddAnoTransition("Idle", ()=> true, true);

			// 待机
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Idle", true)
			.AddAnoTransition("Find", () => state == State.Catching);

			// 拾取开始
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Find", false)
			.AddAnoTransition("Snatch_in", () => state == State.CatchSuccess)
			.AddAnoTransition("Find_cancel", () => state == State.CatchFail);

			// 拾取失败
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Find_cancel")
			.OnAnimationStart((_, _) => gameSortLayer.SortLayerName = LAYER_INTERACTABLE)
			.AddAnoTransition("Idle", () => true, true);

			// 拾取成功 挣扎
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch_in")
			.AddAnoTransition("Snatch", () => true, true)
			.AddAnoTransition("Drop", () => state == State.DropSuccess, true)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			// 挣扎中
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch", true)
			.AddAnoTransition("Drop", () => state == State.DropSuccess)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			// 成功释放
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Drop")
			.OnAnimationComplate((_, _) => Destroy(this.gameObject));

			// 释放失败
			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong_in")
			.AddAnoTransition("Wrong", () => true, true);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong", true)
			.AddAnoTransition("Drop", () => dropCell != null && _isMouseHold)
			.AddAnoTransition("Snatch_in", () => state == State.CatchSuccess);
			
			skeletonMchine.SetDefault("In");
			skeletonMchine.StartMachine();
		}

		private bool _isMouseHold = false;
		public override void OnDragBegin(InteractContext context)
		{
			_isMouseHold = true;
			if(state == State.DropFail)
			{
				state = State.CatchSuccess;
				gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
				return;
			}

			state = State.Catching;
			gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
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
			}

			if(state == State.CatchSuccess)
			{
				FollowDragPoint(context.mousePosition);
			}
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
				else
				{
					return;
				}
			}

			if(state == State.CatchSuccess)
			{
				state = dropCell == null ? State.DropFail : State.DropSuccess; 
			}
		}

        protected override void InitializeFSM()
		{

		}
    }


}
