using System.Collections;
using System.Collections.Generic;
using RuGameFramework.Input;
using Spine;
using UnityEngine;

namespace CGJ2025.System.Interact
{
    public class InteractSystem : MonoBehaviour
    {

        private bool _isDraging;
        
        public MouseManager mouseManager;
        public IInteractable currentDragObj;
        private InteractContext _context;


        void Start()
        {
            _context = new InteractContext();
        }

        
        void Update()
        {
            
        }

        public void DragUpdate()
        {
            if(Input.GetMouseButtonDown(0) && mouseManager.CurrentObject != null)
            {
                currentDragObj = mouseManager.CurrentObject.GetComponent<IInteractable>();
                if(currentDragObj != null)
                {
                    _context.mousePosition = mouseManager.WorldPosition;
                    currentDragObj.OnDragBegin(_context);     
                    _isDraging = true;
                }
            }

            

            if(_isDraging && Input.GetMouseButtonUp(0))
            {
                if(currentDragObj != null)
                {
                    _context.mousePosition = mouseManager.WorldPosition;
                    currentDragObj.OnDragEnd(_context);
                    currentDragObj = null;
                }
                _isDraging = false;
            }else if(_isDraging)
            {
                _context.mousePosition = mouseManager.WorldPosition;
                currentDragObj.OnDragUpdate(_context);
            }
        }
    }


}
