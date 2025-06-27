using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuGameFramework.DataBind
{
	public interface IUIBindProperty 
	{
		public void BindToUI (UIBehaviour ui);

		public void UnBindUI ();
	}

}
