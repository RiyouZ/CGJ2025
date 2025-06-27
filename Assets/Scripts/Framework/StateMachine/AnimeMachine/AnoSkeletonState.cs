using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

namespace RuGameFramework.AnimeStateMachine
{
	public class AnoSkeletonState : SkeletonState
	{
		private Action<ISkeletonState, TrackEntry> _onComplate;
		private Action<ISkeletonState, TrackEntry> _onStart;
		private Action<ISkeletonState, TrackEntry> _onEnd;

		public AnoSkeletonState (SkeletonStateMachine machine)
		{
			_machine = machine;
		}

		// ����ת��Ϊ�ն�����ת��ʱ��
		public AnoSkeletonState SetEmptyMixDuration (float mixDuration)
		{
			_mixDuration = mixDuration;
			return this;
		}

		public AnoSkeletonState AddAnoTransition (string toStateIndex, Func<bool> cond, bool canExit = false)
		{
			return AddTransition(toStateIndex, cond, canExit) as AnoSkeletonState;
		}

		public AnoSkeletonState AddAnoAnimationEvent (string eventName, Action<TrackEntry, Spine.Event> action)
		{
			return AddAnimationEvent(eventName, action) as AnoSkeletonState;
		}

		public AnoSkeletonState OnAnimationComplate (Action<ISkeletonState, TrackEntry> action)
		{
			_onComplate = action;
			return this;
		}

		public AnoSkeletonState OnAnimationStart (Action<ISkeletonState, TrackEntry> action)
		{
			_onStart = action;
			return this;
		}

		public AnoSkeletonState OnAnimationEnd (Action<ISkeletonState, TrackEntry> action)
		{
			_onEnd = action;
			return this;
		}

		public override void OnComplate (TrackEntry track)
		{
			_onComplate?.Invoke(this, track);
		}

		public override void OnStart (TrackEntry track)
		{
			_onStart?.Invoke(this, track);
		}

		public override void OnEnd (TrackEntry track)
		{
			_onEnd?.Invoke(this, track);
		}

	}
}

