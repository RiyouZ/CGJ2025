using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.StateMachine
{
	public class RuState<T> : IRuState<T>
	{
		protected T _index;

		protected IRuStateMachine<T> _fsm;

		public T Index
		{
			get => _index;
			set => _index = value;
		}

		private List<StateTransition<T>> _stateTransitions = new List<StateTransition<T>>();

		public List<StateTransition<T>> StateTransitions
		{
			get => _stateTransitions;
			set => _stateTransitions = value;
		}

		public IRuState<T> AddTransition (IRuState<T> toState, Func<bool> cond)
		{
			_fsm.AddTransition(this.Index, toState.Index, cond);
			return this;
		}

		public virtual IRuState<T> AddTransition (T toStateIndex, Func<bool> cond)
		{
			_fsm.AddTransition(this.Index, toStateIndex, cond);
			return this;
		}

		public virtual void Enter ()
		{
			
		}

		public virtual void Exit ()
		{
			
		}

		public virtual void Update (float deltaTime)
		{
			
		}
	}

}

