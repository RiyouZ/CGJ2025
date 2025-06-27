using Spine;
using System;
using System.Collections.Generic;


namespace RuGameFramework.AnimeStateMachine
{
	public interface ISkeletonState
	{
		public string AnimeName
		{
			set;
			get;
		}

		public bool IsLoop
		{
			set;
			get;
		}

		public SkeletonLayer Layer
		{
			set;
			get;
		}

		public float MixDuration
		{
			set;
			get;
		}

		public List<AnimeStateTransition> StateTransitions
		{
			set;
			get;
		}

		public ISkeletonState AddTransition (string toStateIndex, Func<bool> cond, bool canExit);
		public void OnStart (TrackEntry track);
		public void OnComplate (TrackEntry track);
		public void OnEnd (TrackEntry track);
		public void OnEvent (TrackEntry track, Spine.Event e);
		public void Dispose ();
	}
}


