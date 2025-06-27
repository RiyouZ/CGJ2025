using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.AnimeStateMachine
{
	public abstract class SkeletonState : ISkeletonState
	{
		protected string _animeName;

		protected SkeletonStateMachine _machine;

		public string AnimeName
		{
			get => _animeName;
			set => _animeName = value;
		}

		protected bool _isLoop;
		public bool IsLoop
		{
			get => _isLoop;
			set => _isLoop = value;
		}

		protected SkeletonLayer _layer;
		public SkeletonLayer Layer
		{
			set => _layer = value;
			get => _layer;
		}

		protected float _mixDuration;
		public float MixDuration
		{
			set => _mixDuration = value;
			get => _mixDuration;
		}

		private List<AnimeStateTransition> _stateTransitions = new List<AnimeStateTransition>();
		public List<AnimeStateTransition> StateTransitions
		{
			get => _stateTransitions;
			set => _stateTransitions = value;
		}

		protected Dictionary<string, Action<TrackEntry, Spine.Event>> _eventDic = new Dictionary<string, Action<TrackEntry, Spine.Event>>();


		public virtual void OnComplate (TrackEntry track) {}

		public virtual void OnEnd (TrackEntry track) {}

		public virtual void OnEvent (TrackEntry track, Spine.Event e)
		{
			if (_eventDic.TryGetValue(e.Data.Name, out Action<TrackEntry, Spine.Event> action))
			{
				action?.Invoke(track, e);
			}
		}

		public virtual void OnStart (TrackEntry track) {}

		public virtual void Dispose ()
		{
			_eventDic.Clear ();
			_eventDic = null;

			_stateTransitions.Clear();
			_stateTransitions = null;
			
			_machine = null;
		}

		public virtual ISkeletonState AddTransition (string toStateIndex, Func<bool> cond, bool canExit = false)
		{
			_machine.AddTransition(this.AnimeName, toStateIndex, cond, canExit);
			return this;
		}

		public virtual ISkeletonState AddAnimationEvent (string eventName, Action<TrackEntry, Spine.Event> action)
		{
			if (_eventDic.TryGetValue(eventName, out Action<TrackEntry, Spine.Event> e))
			{
				return this;
			}

			_eventDic.Add(eventName, action);
			return this;
		}
	}

}

