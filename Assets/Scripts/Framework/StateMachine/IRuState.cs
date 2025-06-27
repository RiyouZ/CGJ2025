using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.StateMachine
{
	public interface IRuState<T>
	{
		public T Index
		{
			set; get;
		}

		public List<StateTransition<T>> StateTransitions
		{
			set; get;
		}

		// 添加条件
		public IRuState<T> AddTransition (IRuState<T> toState, Func<bool> cond);

		public IRuState<T> AddTransition (T toStateIndex, Func<bool> cond);

		// 进入状态
		public void Enter ();

		// 更新状态
		public void Update (float deltaTime);

		// 退出状态
		public void Exit ();
	}
}
