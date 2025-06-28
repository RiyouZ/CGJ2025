using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RuGameFramework.AnimeStateMachine
{
	public enum SkeletonLayer
	{
		Base = 0,
		Upper = 1,
		Lower = 2,
	}

	public class SkeletonStateMachine
	{
		private SkeletonAnimation _skeleton;

		private TrackEntry _track;
		public TrackEntry Track => _track;

		private Dictionary<SkeletonLayer, TrackEntry> _layerToTrack;

		private ISkeletonState _currentState;
		private ISkeletonState _defaultState;

		private Dictionary<string, ISkeletonState> _stateDic;

		private bool _isRunning;

		public SkeletonStateMachine (SkeletonAnimation skeleton, int capacity = 2)
		{
			_isRunning = false;
			_skeleton = skeleton;
			_stateDic = new Dictionary<string, ISkeletonState>(capacity);
			_layerToTrack = new Dictionary<SkeletonLayer, TrackEntry>(3);
			_layerToTrack.Add(SkeletonLayer.Base, null);
			_layerToTrack.Add(SkeletonLayer.Upper, null);
			_layerToTrack.Add(SkeletonLayer.Lower, null);
		}

		private ISkeletonState GetState (string stateIndex)
		{
			if (!_stateDic.TryGetValue(stateIndex, out ISkeletonState state))
			{
				return null;
			}
			return state;
		}

		public ISkeletonState AddTransition (string from, string to, Func<bool> cond = null, bool canExit = false)
		{
			// From必须注册
			if (!_stateDic.TryGetValue(from, out ISkeletonState fromState))
			{
				return null;
			}

			fromState.StateTransitions.Add(new AnimeStateTransition(from, to, cond, canExit));

			return fromState;
		}

		// 注册匿名状态
		public AnoSkeletonState RegisterState (SkeletonLayer layer, string stateIndex, bool isLoop = false)
		{
			AnoSkeletonState anoState = new AnoSkeletonState(this);
			anoState.AnimeName = stateIndex;
			anoState.IsLoop = isLoop;
			anoState.Layer = layer;

			if (_stateDic.TryGetValue(stateIndex, out ISkeletonState state))
			{
				if (state == null)
				{
					_stateDic[stateIndex] = anoState;
				}

				return state as AnoSkeletonState;
			}

			_stateDic.Add(stateIndex, anoState);

			return _stateDic[stateIndex] as AnoSkeletonState;
		}

		public void UnRegisterState (string stateIndex)
		{
			if (!_stateDic.TryGetValue(stateIndex, out ISkeletonState state))
			{
				return;
			}

			state.Dispose();
			_stateDic.Remove(stateIndex);
		}

		public void SetDefault (string stateIndex)
		{
			if (!_stateDic.TryGetValue(stateIndex, out ISkeletonState state))
			{
				return;
			}

			if (_skeleton == null)
			{
#if UNITY_EDITOR
				Debug.LogError("Skeleton Animation Null");
#endif
				return;
			}

			_defaultState = state;
			_skeleton.AnimationState.Start += OnStart;
			_skeleton.AnimationState.Complete += OnComplate;
			_skeleton.AnimationState.End += OnEnd;
			_skeleton.AnimationState.Event += OnEvent;
		}

		public void BackDefault ()
		{
			if (_defaultState == null)
			{
				return;
			}

			ChangeState(_defaultState);
		}

		public void Dispose ()
		{
			if (_isRunning)
			{
				Stop();
			}

			_skeleton.AnimationState.Start -= OnStart;
			_skeleton.AnimationState.Complete -= OnComplate;
			_skeleton.AnimationState.End -= OnEnd;
			_skeleton.AnimationState.Event -= OnEvent;

			foreach (var state in _stateDic.Values)
			{
				state.Dispose();
			}

			_stateDic.Clear();
			_stateDic = null;
			_skeleton = null;
		}

		private void OnStart (TrackEntry track)
		{
			var state = GetState(track.Animation.Name);
			if (state == null)
			{
				return;
			}

			state.OnStart(track);
		}

		// 不要在该回调上SetAnimation
		private void OnEnd (TrackEntry track)
		{
			var state = GetState(track.Animation.Name);
			if (state == null)
			{
				return;
			}

			state.OnEnd(track);
		}

		private void OnComplate (TrackEntry track)
		{
			var state = GetState(track.Animation.Name);
			if (state == null)
			{
				return;
			}

			// 调用该动画结束事件
			state.OnComplate(track);

			if (state.StateTransitions == null)
			{
				return;
			}

			// 检测条件并转换
			foreach (var cond in state.StateTransitions)
			{
				if (!cond.CanComplateExit)
				{
					continue;
				}
				// 动画结束后转换
				var canTrans = cond.TransitionCondition?.Invoke();
				if (canTrans == null || canTrans == true)
				{
					ChangeState(GetState(cond.ToState));
					return;
				}
			}

		}

		private void OnEvent (TrackEntry track, Spine.Event e)
		{
			if (_currentState == null)
			{
				return;
			}

			_currentState.OnEvent(track, e);

		}

		public void StartMachine ()
		{
			Start(_defaultState);
		}

		private void Start (ISkeletonState defaultState)
		{
			if (defaultState == null)
			{
				return;
			}

			_isRunning = true;

			if (_defaultState != defaultState)
			{
				_defaultState = defaultState;
			}

			_currentState = defaultState;
			_layerToTrack[defaultState.Layer] = _skeleton.AnimationState.SetAnimation((int)defaultState.Layer, defaultState.AnimeName, defaultState.IsLoop);
		}

		// 更新状态机
		public void UpdateMachine ()
		{
			if (!_isRunning)
			{
				return;
			}

			Update();
		}

		private void Update ()
		{
			if (_currentState == null)
			{
				return;
			}

			// 检测条件并转换
			foreach (var cond in _currentState.StateTransitions)
			{
				if (cond.CanComplateExit)
				{
					continue;
				}

				// 直接转换
				var canTrans = cond.TransitionCondition?.Invoke();
				if (canTrans == true)
				{
					ChangeState(GetState(cond.ToState));
					return;
				}
			}
		}

		private void Stop ()
		{
			if (_isRunning)
			{
				_isRunning = false;
			}
		}

		// 重置状态机
		public void ResetMachine ()
		{
			Stop();
			ChangeState(_defaultState);
		}

		private void ChangeState (ISkeletonState state)
		{
			if (state == null)
			{
				return;
			}

			_currentState = state;
			_layerToTrack[state.Layer] = _skeleton.AnimationState.SetAnimation((int)state.Layer, state.AnimeName, state.IsLoop);
			_skeleton.Update(0);
			_skeleton.LateUpdate();
		}

		public void ClearTrackForLayer (SkeletonLayer layer)
		{
			if (!_layerToTrack.TryGetValue(layer, out TrackEntry track))
			{
				return;
			}

			_skeleton.AnimationState.ClearTrack((int)layer);
		}


		// 持有空动画过度
		public void ClearTrackForLayer (SkeletonLayer layer, float mixDuration = 0)
		{
			if (!_layerToTrack.TryGetValue(layer, out TrackEntry track))
			{
				return;
			}

			var emptyTrack = _skeleton.AnimationState.SetEmptyAnimation((int)layer, mixDuration);
			emptyTrack.Complete += (track) =>
			{
				_skeleton.AnimationState.ClearTrack((int)layer);
			};
		}

		// 打断循环的轨道
		public void InterruptTrack (SkeletonLayer layer, float mixDuration = 0, float delay = 0)
		{
			if (!_layerToTrack.TryGetValue(layer, out TrackEntry track))
			{
				return;
			}

			_layerToTrack[layer] = _skeleton.AnimationState.SetEmptyAnimation((int)layer, mixDuration);
			_skeleton.Update(0);
			_skeleton.LateUpdate();
		}

		// 直接在轨道上播放动画 不经过状态
		public void PlayTrackAnimation (SkeletonLayer layer, string animeName, bool isLoop = false)
		{
			// 基层不可以直接播放
			if (layer == SkeletonLayer.Base)
			{
				return;
			}

			_layerToTrack[layer] = _skeleton.AnimationState.SetAnimation((int)layer, animeName, isLoop);
		}

		public void PlayTrackAnimation (string animeName, float mixDuration = 0)
		{
			var state = GetState(animeName);
			if (state == null)
			{
				return;
			}

			// // 基层不可以直接播放
			// if (state.Layer == SkeletonLayer.Base)
			// {
			// 	return;
			// }

			_layerToTrack[state.Layer] = _skeleton.AnimationState.SetAnimation((int)state.Layer, state.AnimeName, state.IsLoop);
			_layerToTrack[state.Layer].MixDuration = mixDuration;
			_skeleton.Update(0);
			_skeleton.LateUpdate();
		}

	}

}
