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
    public class Rabbit : Character.Character<Rabbit.State>
    {
        public enum State
        {
            Hide,
            Find,
            Catch,
            Escape,
            DropSuccess,
            DropFail
        }

        public State state;
		public float escapeTime;
		public float genEffectPercent;

		[SerializeField] private bool _canCatch = false;
		[SerializeField] private bool _canDrop;
		[SerializeField] private bool _isEscape;
		[SerializeField] private bool _isWrong;

		public AudioClip audioFind;
		public AudioClip audioFind2;
		public AudioClip audioCatch;
		public AudioClip audioCatch2;
		public AudioClip audioDropSccess;
		public AudioClip audioDropSccess2;
		public AudioClip audioDropSccess3;
		public AudioClip audioDropFail;

		public AudioClip audioCatchSuccess;

		private Timer _timer;

		[SerializeField] private MouseManager _mouseManager;

        public override bool IsGrassRepel 
		{
			get;
			set;
		}

        protected override void OnStart()
        {
            if(_mouseManager == null)
			{
				_mouseManager = App.Instance.MouseManager;
			}
        }

        protected override void InitializeFSM()
        {
            fSM.RegisterState(State.Hide)
				.OnEnter((fsmState)=>
				{	
					PlayGrassAudio(audioGen);
				}).AddTransition(State.Find, () => 
				{
					return state == State.Find;
				});

			fSM.RegisterState(State.Find)
				.AddTransition(State.Catch, ()=>
				{
					return state == State.Catch;
				})
				.AddTransition(State.Escape, ()=>
				{
					return _isEscape;
				});
				
			fSM.RegisterState(State.Catch)
				.OnEnter((fsmState)=>
				{
					_isEscape = false;
					_isWrong = false;
					skeletonMchine.PlayTrackAnimation("Snatch_in");
				}).AddTransition(State.DropSuccess, ()=>
				{
					return dropCell != null;
				}).AddTransition(State.DropFail, ()=>
				{
					return dropCell == null && _isWrong;
				});
			
			fSM.RegisterState(State.DropFail)
				.AddTransition(State.Catch, ()=>
				{
					return state == State.Catch && _canCatch;
				});


			fSM.RegisterState(State.Escape)
				.OnEnter((fsmState)=>
				{
					state = State.Escape;
				});
			
			fSM.SetDefault(State.Hide, 0.016f);
        }
        
        protected override void InitializeSkeleton()
		{
			skeletonMchine.RegisterState(SkeletonLayer.Base, "In", true)
			.AddAnoTransition("Find", ()=>
			{
				return state == State.Find;
			});

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Find", false)
			.OnAnimationStart((_, _) => {
				var random = UnityEngine.Random.Range(0, 2);
				if (random == 0) PlayAudio(audioFind); 
				else PlayAudio(audioFind2);
				_canCatch = true;
			})
			.OnAnimationComplate((_, _) => OnCharacterEscape())
			.AddAnoTransition("Snatch_in", ()=>
			{
				return state == State.Catch && !_isEscape;
			})
			.AddAnimationEvent("Cantselect", (_, _) => _canCatch = false);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch_in")
			.OnAnimationStart((_, _) =>
			{
				Cursor.visible = false;
				var random = UnityEngine.Random.Range(0, 2);
				if (random == 0) PlayAudio(audioCatch); 
				else PlayAudio(audioCatch2);
			})		
			.AddAnoTransition("Snatch", () => true, true)
			.AddAnoTransition("Drop", () => state == State.DropSuccess)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch", true)
			.OnAnimationStart((_, _) => {
				Cursor.visible = false;
			})
			.OnAnimationEnd((_, _) => {
				Cursor.visible = true;
			})			
			.AddAnoTransition("Drop", () => state == State.DropSuccess)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Drop")
			.OnAnimationStart((_, _) => 
			{
				Cursor.visible = true;
				IsDestroy = true;
				var random = UnityEngine.Random.Range(0, 3);
				if (random == 0) PlayAudio(audioDropSccess);
				if (random == 1) PlayAudio(audioDropSccess2);
				else PlayAudio(audioDropSccess3);

				gameSortLayer.SortLayerName = LAYER_INTERACTABLE;
			})
			.OnAnimationEnd((_, _) => 
			{
				OnCharacterDroped();
			})
			.OnAnimationComplate((_, _) =>
			{
				OnCharacterDroped();
			});

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong_in")
			.OnAnimationStart((_, _) =>
			{
				PlayAudio(audioDropFail);
				Cursor.visible = false;
				_isWrong = true;
				_canCatch = true;
			})
			.AddAnoTransition("Wrong", () => true);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong", true)
			.OnAnimationStart((_, _) => {
				Cursor.visible = false;
				gameSortLayer.SortLayerName = LAYER_HOVER;
			})
			.OnAnimationEnd((_, _) => {
				Cursor.visible = true;
			});

			skeletonMchine.SetDefault("In");
			skeletonMchine.StartMachine();
		}

		private void OnCharacterEscape()
		{
			if(GenCell != null)
			{
				GenCell.OnCharacterRemove();
			}
			Destroy(this.gameObject);
		}

        protected override void DropedEffect()
        {
            GridSystem.GenerateTime = Mathf.Max(1, GridSystem.GenerateTime - GridSystem.GenerateTime * genEffectPercent);
        }

		// 掉落事件
        protected override void OnCharacterDroped()
		{
			base.OnCharacterDroped();
			Destroy(this.gameObject);
		}

		protected override void OnUpdate()
		{
			if(_isWrong)
			{
				FollowDragPoint(_mouseManager.WorldPosition);
			}
		}

		public override void OnDragBegin(InteractContext context)
		{
			base.OnDragBegin(context);

			state = State.Catch;
			onshotAudio.PlayOneShot(audioCatchSuccess);

			var random = UnityEngine.Random.Range(0, 2);
			if (random == 0) PlayGrassAudio(audioRepel); 
			else PlayGrassAudio(audioRepel);
			gameSortLayer.SortLayerName = LAYER_HOVER;
		}

		public override void OnDragEnd(InteractContext context)
		{
			base.OnDragEnd(context);

			if(dropCell != null && dropCell.GridIndex != GridSystem.HeadStone)
			{
				DropSuccess(dropCell);
			}
			else
			{
				DropFail();
			}
		}

		protected override bool DragCondition(InteractContext context) 
		{
			return _canCatch == true && IsGrassRepel && state != State.DropSuccess;
		}

		public void DropSuccess(Cell dropCell)
		{
			state = State.DropSuccess;
		}

		public void DropFail()
		{
			state = State.DropFail;
		}

        public override void OnCellMouseDown(Cell cell)
        {
			state = State.Find;
			_isEscape = false;
			_timer = timerManager.SetTimeout((deltaTime)=>
			{
				if(state == State.Find)
				{
					Escape();
				}

				_timer.Release();
			}, escapeTime);
        }
        
		public void Escape()
		{
			_isEscape = true;
		}
        
    }

}

