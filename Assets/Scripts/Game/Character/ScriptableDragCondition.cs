using System.Collections;
using System.Collections.Generic;
using CGJ2025.System.Interact;
using UnityEngine;


namespace CGJ2025.Character
{
    public abstract class ScriptableDragCondition : ScriptableObject
    {
        public abstract bool CanDrag(InteractContext context);
        public virtual bool DragTimeout(InteractContext context) {return false;}
    }

}
