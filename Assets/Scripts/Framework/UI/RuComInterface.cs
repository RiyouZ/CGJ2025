using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.UI
{
	public interface IUIGroupable
	{
		public int GroupIndex {set; get;}

		public void ChangeOn ();
		public void ChangeOff ();
	}

}

