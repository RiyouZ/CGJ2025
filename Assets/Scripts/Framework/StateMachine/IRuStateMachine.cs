using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.StateMachine
{
	public interface IRuStateMachine<T>
	{
		public IRuState<T> AddTransition (T from, T to, Func<bool> cond = null);
		public AnoState<T> RegisterState (T t);
		public IRuState<T> RegisterState (T t, IRuState<T> state);
		public IRuState<T> RegisterState (IRuState<T> stateIns);
		public void UnRegisterState (T stateIndex);
		public void UnRegisterState (IRuState<T> stateIns);
		public void SetDefault (T stateIndex, float timeSimple);
		public void UpdateMachine ();
		public void Reset ();
		public void Dispose ();
	}



}

