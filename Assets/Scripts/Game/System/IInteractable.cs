using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGJ2025.System.Interact
{
	public interface IInteractable
	{
		public void OnDragBegin (InteractContext context);
		public void OnDragUpdate (InteractContext context);
		public void OnDragEnd (InteractContext context);
	}

}
