using RuGameFramework.Args;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.UI
{
	public interface ICanvasArgs : IGameArgs {}

	public class NullArgs : ICanvasArgs
	{
		public void Dispose ()
		{

		}
	}
}

