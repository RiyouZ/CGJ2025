using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public interface IFactory<TType, TData>
	{
		public TCreate Create<TCreate> (TData data) where TCreate : TType, new();
		public void Destroy (TType type);
	}
}

