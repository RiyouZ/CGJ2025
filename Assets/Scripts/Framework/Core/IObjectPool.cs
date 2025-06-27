using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public interface IPoolItem
	{
		public void Init ();
		public void Reset ();
	}

	public interface IObjectPool<T>
	{
		// 创建对象
		public T Spawn ();
		// 回收对象
		public void Collection (T obj);
		// 销毁池
		public void Dispose ();
	}

}

