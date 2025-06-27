using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.AnimeStateMachine
{
	public struct AnimeStateTransition
	{
		private string _fromState;
		public string FromState => _fromState;

		private string _toState;
		public string ToState => _toState;

		private Func<bool> _transitionAction;
		public Func<bool> TransitionCondition => _transitionAction;

		// 需要播放完成后退出
		private bool _canComplateExit;
		public bool CanComplateExit => _canComplateExit;

		public AnimeStateTransition (string from, string to, Func<bool> action = null, bool canExit = false)
		{
			_fromState = from;
			_toState = to;
			_canComplateExit = canExit;
			_transitionAction = action;
		}
	}
}

