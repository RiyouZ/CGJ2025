using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public class TimerManager : MonoBehaviour
	{
		public enum TimeType
		{
			RealTimeSinceStartUp,
			Time
		}
		private TimeType _timeType;

		private float PassTime
		{
			get => GetTimeTick(_timeType) - _startTime;
		}

		private WaitForSecondsRealtime _waitForSec;

		private ulong _autoId = 0;
		private float _startTime;

		private Coroutine _asyncScheduleHandle;

		private List<ulong> _recycleId = new List<ulong>(16);
		private ObjectPool<Timer> _timerPool = new ObjectPool<Timer>(16);
		private Dictionary<ulong, Timer> _timerDic = new Dictionary<ulong, Timer>(16);
		private List<Timer> _timerRemoveList = new List<Timer>();

		// TODO 堆时间排序优化
		private LinkedList<Timer> _taskList = new LinkedList<Timer>();

		private ulong GetAutoId ()
		{
			if (_recycleId.Count == 0)
			{
				_autoId++;
				return _autoId;
			}
			ulong autoId = _recycleId[_recycleId.Count - 1];
			_recycleId.RemoveAt(_recycleId.Count - 1);

			return autoId;
		}

		private Timer GetTimer ()
		{
			var Timer = _timerPool.Spawn();
			return Timer;
		}

		private void RecycleTimer (Timer timer)
		{
			timer.Reset();
			_timerPool.Collection(timer);
		}

		private float GetTimeTick (TimeType type)
		{
			switch (type)
			{
				case TimeType.RealTimeSinceStartUp:
					return Time.realtimeSinceStartup;
				case TimeType.Time:
					return Time.time;
			}
			return 0;
		}

		public Timer SetTimeout (Action<float> action, float delayTime)
		{
			return SetInterval(action, 1, delayTime);
		}

		public Timer SetInterval(Action<float> action, float interval, float delayTime, int repeatCount = Timer.Infinity)
		{
			ulong autoId = GetAutoId ();
			Timer timer = GetTimer();

			timer.Init(autoId, PassTime, action, interval, delayTime, repeatCount);

			// 已经持有的Id直接覆盖
			if (_timerDic.ContainsKey(autoId))
			{
				_timerDic[autoId].Release();
				_timerDic[autoId] = timer;
				_taskList.AddLast(timer);

				return _timerDic[autoId];
			}

			_timerDic.Add(autoId, timer);
			_taskList.AddLast(timer);

			return timer;
		}

		public void ReleaseTimer (Timer timer)
		{
			if (!_timerDic.TryGetValue(timer.Id, out timer))
			{
				return;
			}

			timer.MarkRelease();
			_timerRemoveList.Add(timer);
		}

		public void StartSchedule (TimeType timeType, float interval)
		{
			_startTime = GetTimeTick(timeType);
			_waitForSec = new WaitForSecondsRealtime(interval);
			_asyncScheduleHandle = StartCoroutine(AsyncTimeSchedule());
		}

		public void StopSchedule ()
		{
			if (_asyncScheduleHandle == null)
			{
				return;
			}

			StopCoroutine(_asyncScheduleHandle);
		}

		private IEnumerator AsyncTimeSchedule ()
		{
			while (true)
			{
				if (_taskList.Count <= 0)
				{
					yield return _waitForSec;
				}

				// 遍历计时器
				foreach (var timerTask in _taskList)
				{
					if (timerTask.IsReleased)
					{
						_timerRemoveList.Add(timerTask);
						continue;
					}

					timerTask.Update(PassTime);
				}
				// 清空释放的定时器
				UpdateReleaseTimer();

				yield return _waitForSec;
			}
		}

		private void UpdateReleaseTimer ()
		{
			foreach (var timer in _timerRemoveList)
			{
				_taskList.Remove(timer);
				_timerDic.Remove(timer.Id);
				// 回收Timer
				_recycleId.Add(timer.Id);
				RecycleTimer(timer);
			}

			_timerRemoveList.Clear();
		}



	}



}