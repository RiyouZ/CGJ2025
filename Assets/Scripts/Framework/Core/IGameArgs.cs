using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Args
{
	// TODO 参数创建器
	public interface IGameArgs 
	{
		public void Dispose ();
	}
	public interface ISceneArgs : IGameArgs {}
}


