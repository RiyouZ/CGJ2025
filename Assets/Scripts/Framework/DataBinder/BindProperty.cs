using System;
using System.Collections.Generic;

namespace RuGameFramework.DataBind
{
	public abstract class BindProperty<T>
	{
		private T _value;
		protected Action<T> _onValueChange;
		protected List<Action<T>> _onValueChangeRefList = new List<Action<T>>();

		public T Value
		{
			set
			{
				if (value == null)
				{
					return;
				}

				_value = value;
				_onValueChange?.Invoke(_value);
			}
			get
			{
				return _value; 
			}
		}

		// 绑定驱动控制
		public virtual void Bind (Action<T> onValueChange)
		{
			_onValueChange += onValueChange;
			_onValueChangeRefList.Add(onValueChange);
		}
		// 解绑控制
		public virtual void UnBind ()
		{
			foreach (var actionRef in _onValueChangeRefList)
			{
				_onValueChange -= actionRef;
			}

			_onValueChangeRefList.Clear();
			_onValueChange = null;
		}
	}
}

