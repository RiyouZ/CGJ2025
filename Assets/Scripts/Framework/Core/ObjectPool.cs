using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public class ObjectPool<T> : IObjectPool<T> where T : IPoolItem, new()
	{

		// TODO »ﬂ”‡∂‘œÛÀı»›
		private Stack<T> _pool;

		public ObjectPool (int capacity)
		{
			_pool = new Stack<T>(capacity);
		}

		public void Collection (T obj)
		{
			obj.Reset();
			_pool.Push(obj);
		}

		public T Spawn ()
		{
			T obj = default (T);
			if (_pool.Count == 0)
			{
				obj = new T();
				return obj;
			}

			obj = _pool.Pop();
			obj.Init();
			return obj;
		}

		public void Dispose ()
		{
			_pool.Clear();
			_pool = null;
		}
	}
}
