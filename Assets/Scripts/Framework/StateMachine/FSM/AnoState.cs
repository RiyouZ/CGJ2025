using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.StateMachine
{
	public class AnoState<T> : RuState<T>
	{
		private Action<IRuState<T>> _onEnter;
		private Action<IRuState<T>, float> _onUpdate;
		private Action<IRuState<T>> _onExit;

		public AnoState (IRuStateMachine<T> fsm)
		{
			_fsm = fsm;
		}

		public AnoState<T> OnEnter (Action<IRuState<T>> onEnter)
		{
			_onEnter = onEnter;
			return this;
		}

		public AnoState<T> OnUpdate (Action<IRuState<T>, float> onUpdate)
		{
			_onUpdate = onUpdate;
			return this;
		}

		public AnoState<T> OnExit (Action<IRuState<T>> onExit)
		{
			_onExit = onExit;
			return this;
		}

		public override void Enter ()
		{
			_onEnter?.Invoke(this);
		}

		public override void Update (float deltaTime)
		{
			_onUpdate?.Invoke(this, deltaTime);
		}

		public override void Exit ()
		{
			_onExit?.Invoke(this);
		}
	}


}
