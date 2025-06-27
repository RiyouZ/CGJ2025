using RuGameFramework.Event;
using RuGameFramework.Input.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Input
{
	public class MouseData
	{
		private MouseButtonState _leftState;
		private MouseButtonState _rightState;
		public Vector3 worldPosistion = Vector3.zero;

		public MouseButtonState LeftState
		{
			set
			{
				_leftState = value;
				EventManager.InvokeEvent(MouseEvent.OnMouseLeftUpdate, new MouseEventArgs(this));
			}
			get
			{
				return _leftState;
			}
		}
		public MouseButtonState RightState
		{
			set
			{
				_rightState = value;
				EventManager.InvokeEvent(MouseEvent.OnMouseRightUpdate, new MouseEventArgs(this));
			}
			get
			{
				return _rightState;
			}
		}

		public MouseData ()
		{
			this._leftState = MouseButtonState.None;
			this._rightState = MouseButtonState.None;
		}
	}
}
