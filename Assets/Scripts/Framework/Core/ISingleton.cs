using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public interface ISingleton<T>
	{
		public static T Instance
		{
			get;
		}
	}
}
