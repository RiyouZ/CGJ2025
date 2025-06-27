using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public interface ITimeManager
	{
		public IRuTimer SetInterval (Action<float> action, float interval, float delayTime, int repeatCount = IRuTimer.INFINITY);
	}
}


