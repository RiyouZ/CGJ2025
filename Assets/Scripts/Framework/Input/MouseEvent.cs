using RuGameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Input.Event
{
	public static class MouseEvent
	{
		public static string OnMouseLeftUpdate = "OnMouseLeftUpdate";
		public static string OnMouseRightUpdate = "OnMouseRightUpdate";
	}

	public class MouseEventArgs : IGameEventArgs
	{
		public MouseData mouseData;

		public MouseEventArgs(MouseData mouseData)
		{
			this.mouseData = mouseData;
		}

		public void Dispose ()
		{
			mouseData = null;
		}
	}

}

