using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.StateMachine
{
	public class RuFSM<T> : IRuStateMachine<T>
	{
		private Dictionary<T, IRuState<T>> _stateDic;

		private IRuState<T> _defaultState;
		private IRuState<T> _currentState;
		private IRuState<T> _preState;

		private bool _isRunning;
		private float _updateSimple;

		private bool _isInit;

		public RuFSM (int capacity)
		{
			_stateDic = new Dictionary<T, IRuState<T>>(capacity);
			_isRunning = false;
		}

		private IRuState<T> GetState (T stateIndex)
		{
			if (!_stateDic.TryGetValue(stateIndex, out IRuState<T> state))
			{
				return null;
			}
			return state;
		}

		public IRuState<T> AddTransition (T from, T to, Func<bool> cond = null)
		{
			// From???????
			if (!_stateDic.TryGetValue(from, out IRuState<T> fromState))
			{
				return null;
			}

			fromState.StateTransitions.Add(new StateTransition<T>(from, to, cond));

			return fromState;
		}

		public void Dispose ()
		{
			_stateDic.Clear();
			_stateDic = null;
		}

		// ?????????
		public AnoState<T> RegisterState (T stateIndex)
		{
			if (_stateDic.TryGetValue(stateIndex, out IRuState<T> state))
			{
				if (state == null)
				{
					_stateDic[stateIndex] = new AnoState<T>(this);
					_stateDic[stateIndex].Index = stateIndex;
				}

				return _stateDic[stateIndex] as AnoState<T>;
			}

			_stateDic.Add(stateIndex, new AnoState<T>(this));
			_stateDic[stateIndex].Index = stateIndex;

			return _stateDic[stateIndex] as AnoState<T>;
		}

		// ????????????
		public IRuState<T> RegisterState (T stateIndex, IRuState<T> stateIns)
		{
			stateIns.Index = stateIndex;
			if (_stateDic.TryGetValue(stateIndex, out IRuState<T> state))
			{
				if (state == null)
				{
					_stateDic[stateIndex] = stateIns;
				}

				return _stateDic[stateIndex];
			}
			_stateDic.Add(stateIndex, stateIns);

			return _stateDic[stateIndex];
		}

		// ???????? ???????????????
		public IRuState<T> RegisterState (IRuState<T> stateIns)
		{
			T index = stateIns.Index;
			if (index == null)
			{
#if UNITY_EDITOR
				Debug.LogError($"?? {index} ??");
#endif
				return null;
			}

			if (_stateDic.TryGetValue(index, out IRuState<T> state))
			{
				if (state == null)
				{
					_stateDic[index] = stateIns;
				}

				return _stateDic[index];
			}
			_stateDic.Add(index, stateIns);

			return _stateDic[index];
		}

		public void Reset ()
		{
			if (_isRunning)
			{
				_isRunning = false;
			}

			Start(_defaultState);
		}

		public void UnRegisterState (T stateIndex)
		{
			if (!_stateDic.TryGetValue(stateIndex, out IRuState<T> state))
			{
				return ;
			}
			_stateDic.Remove(stateIndex);
		}

		public void UnRegisterState (IRuState<T> stateIns)
		{
			if (!_stateDic.TryGetValue(stateIns.Index, out IRuState<T> state))
			{
				return;
			}
			_stateDic.Remove(stateIns.Index);
		}

		// ???��???
		public void SetDefault (T stateIndex, float timeSimple)
		{
			_isInit = true;
			_updateSimple = timeSimple;
			if (!_stateDic.TryGetValue(stateIndex, out IRuState<T> state))
			{
				return;
			}
			_defaultState = state;
		}

		// ????????
		public void UpdateMachine ()
		{
			if(!_isInit) return;

			if (_isRunning)
			{
				Update(_updateSimple);
				return;
			}

			Start(_defaultState);
		}

		private void Start (IRuState<T> defaultState)
		{
			if (_isRunning)
			{
				Update(_updateSimple);
				return;
			}

			_isRunning = true;
			_defaultState = defaultState;
			_currentState = defaultState;
			_currentState.Enter();
		}

		private void Update (float deltaTime)
		{
			if(_currentState.StateTransitions == null)
			{
				return;
			}

			foreach (var cond in _currentState.StateTransitions)
			{
				var canTrans = cond.TranstionCondition?.Invoke();

				if (canTrans == null || canTrans == true)
				{
					ChangeState(GetState(cond.ToState));
					return;
				}
			}

			_currentState.Update(deltaTime);
		}

		private void ChangeState (IRuState<T> state)
		{
			if (state == null)
			{
				return;
			}

			_preState = _currentState;
			_currentState.Exit();
			_currentState = state;
			_currentState.Enter();
		}
	}

}
