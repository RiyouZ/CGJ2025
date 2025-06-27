using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Util
{
	public class AutoIdGenerator
	{
		private readonly int MaxCapacity;
		private int _autoId;

		// ÏÐÖÃµÄId
		private Stack<int> _recycleIdStack = null;

		public AutoIdGenerator (int defaultId, int maxCapacity = 4)
		{
			_autoId = defaultId;
			MaxCapacity = maxCapacity;
		}

		public int GetAutoId ()
		{
			if (_recycleIdStack == null)
			{
				_recycleIdStack = new Stack<int>(MaxCapacity / 2);
			}

			if (_recycleIdStack.Count <= 0)
			{
				_autoId++;
				return _autoId;
			}
			int id = _recycleIdStack.Pop();
			return id;
		}
		public void RecycleAutoId (int id)
		{
			if (_recycleIdStack == null)
			{
				return;
			}
			_recycleIdStack.Push(id);
		}

	}
}

