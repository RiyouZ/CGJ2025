using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.StateMachine
{
	public struct StateTransition<T>
	{
		private T _fromState;
		public T FromState => _fromState;

		private T _toState;
		public T ToState => _toState;

		private Func<bool> _transitionAction;
		public Func<bool> TranstionCondition => _transitionAction;

		public StateTransition (T from, T to, Func<bool> action = null)
		{
			_fromState = from;
			_toState = to;

			_transitionAction = action;
		}

	}
}