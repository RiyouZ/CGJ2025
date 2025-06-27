using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Core
{
	public class Timer :  IRuTimer, IPoolItem
	{
		public const int Infinity = -1;
		private ulong _id;
		public ulong Id => _id;
		private float _repeatCount;
		private float _interval;
		private float _intervalTime;
		private float _delayTime;
		public float delayTime => _delayTime;
		private Action<float> _onTimeoutEvent;
		private float _startTime;
		public float StartTime => _startTime;
		private float _passTime;
		public float PassTime => _passTime;
		private bool _isReleased;
		public bool IsReleased => _isReleased;

		public Timer ()
		{

		}

		public void Update (float time)
		{
			if (_isReleased)
			{
				return;
			}
			
			// 经过总时间
			_passTime = time - _startTime;

			// 未到延迟执行时间
			if (_delayTime > _passTime)
			{
				return;
			}

			// 经过时间 > 更新时间
			if (_passTime >= _intervalTime || _passTime == 0)
			{
				_intervalTime = _passTime + _interval;

				_onTimeoutEvent?.Invoke(_interval);
				if (_repeatCount != Infinity)
				{
					_repeatCount--;
					if (_repeatCount <= 0)
					{
						MarkRelease();
					}
				}
			}
		}

		public void MarkRelease ()
		{
			_isReleased = true;
			_onTimeoutEvent = null;
		}

		public void Release ()
		{
			if (_isReleased)
			{
				return;
			}
			MarkRelease();
		}

		public void Init (ulong id, float startTime, Action<float> onTimeoutEvent, float interval, float delayTime = 0, int repeatCount = Infinity)
		{
			_id = id;
			_startTime = startTime;
			_repeatCount = Math.Max(repeatCount, Infinity);
			_interval = interval;
			_intervalTime = interval;
			_delayTime = delayTime;
			_onTimeoutEvent = onTimeoutEvent;
			_isReleased = false;
		}

		public void Reset ()
		{
			_id = 0;
			_startTime = 0;	
			_repeatCount = 0;
			_interval = 0;
			_intervalTime = 0;
			_delayTime = 0;
			_onTimeoutEvent = null;
			_isReleased = false;
		}

		public void Init ()
		{

		}
	}
}

