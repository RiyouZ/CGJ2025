using System.Collections;
using System.Collections.Generic;
using CGJ2025.SceneCell;
using CGJ2025.System.Interact;
using RuGameFramework;
using RuGameFramework.AnimeStateMachine;
using RuGameFramework.Core;
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

		[SerializeField] private bool _canCatch = false;
		[SerializeField] private bool _canDrop;
		[SerializeField] private bool _isEscape;
		[SerializeField] private bool _isWrong;

		private Timer _timer;
		private Cell dropCell;

        protected override void InitializeFSM()
        {
            fSM.RegisterState(State.Hide)
				.OnEnter((fsmState)=>
				{	

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
			
			fSM.SetDefault(State.Hide, App.SampleRate);
        }
        
        protected override void InitializeSkeleton()
		{
			skeletonMchine.RegisterState(SkeletonLayer.Base, "In", true)
			.AddAnoTransition("Find", ()=>
			{
				return state == State.Find;
			});

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Find", false)
			.OnAnimationStart((_, _) => _canCatch = true)
			.AddAnoTransition("Snatch_in", ()=>
			{
				return state == State.Catch && !_isEscape;
			})
			.AddAnimationEvent("Cantselect", (_, _) => _canCatch = false);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch_in")
			.AddAnoTransition("Snatch", () => true, true);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Snatch", true)
			.AddAnoTransition("Drop", () => state == State.DropSuccess)
			.AddAnoTransition("Wrong_in", () => state == State.DropFail);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Drop")
			.OnAnimationEnd((_, _) =>
			{
				Destroy();
			});

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong_in")
			.OnAnimationStart((_, _) =>
			{
				_isWrong = true;
				_canCatch = true;
			})
			.AddAnoTransition("Wrong", () => true);

			skeletonMchine.RegisterState(SkeletonLayer.Base, "Wrong", true);

			skeletonMchine.SetDefault("In");
			skeletonMchine.StartMachine();
		}

		private void Destroy()
		{
			Destroy(this.gameObject);
		}	

		protected override void OnUpdate()
		{
			if(Input.GetMouseButtonDown(1))
			{
				OnCellMouseDown();
			}

			if(_isWrong)
			{
				FollowDragPoint(App.Instance.MouseManager.WorldPosition);
			}
		}

		public override void OnDragBegin(InteractContext context)
		{
			base.OnDragBegin(context);
			if(_canCatch)
			{
				state = State.Catch;
			}
		}

		public override void OnDragEnd(InteractContext context)
		{
			base.OnDragEnd(context);

			if(dropCell != null)
			{
				DropSuccess(dropCell);
			}
			else
			{
				DropFail();
			}

			dropCell = null;
		}

		protected override bool DragCondition(InteractContext context) 
		{
			return _canCatch == true;
		}


		void OnTriggerEnter2D(Collider2D other)
		{
			dropCell = other.GetComponent<Cell>();
		}

		void OnTriggerExit2D(Collider2D other)
		{
			dropCell = null;
		}

		public void DropSuccess(Cell dropCell)
		{
			state = State.DropSuccess;
		}

		public void DropFail()
		{
			state = State.DropFail;
		}


        public override void OnCellMouseDown()
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

