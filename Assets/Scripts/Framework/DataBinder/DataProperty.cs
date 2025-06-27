using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.DataBind
{
	public class DataProperty<T> : BindProperty<T>
	{
		public DataProperty (T value)
		{
			Value = value;
		}
	}
}

