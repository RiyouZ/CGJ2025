using System.Collections;
using System.Collections.Generic;
using RuGameFramework;
using RuGameFramework.Core;
using RuGameFramework.Input;
using Spine;
using UnityEngine;

namespace CGJ2025.System.Interact
{
    public class InteractSystem : MonoBehaviour
    {

        private bool _isDraging;
        
        public MouseManager mouseManager;
        public TimerManager timerManager;
        public IInteractable currentInteractObj;
        private InteractContext _context;

        private float _dragTime;

        void Start()
        {
            _context = new InteractContext();
            _dragTime = 0;
        }

        
        void Update()
        {
            
        }

        public void Initialize(MouseManager mouseManager, TimerManager timerManager)
        {
            this.mouseManager = mouseManager;
            this.timerManager = timerManager;
        }

        public void DragUpdate()
        {
            if(Input.GetMouseButtonDown(0) && mouseManager.CurrentObject != null)
            {
                currentInteractObj = mouseManager.CurrentObject.GetComponent<IInteractable>();
                if(currentInteractObj != null)
                {
                    _context.mousePosition = mouseManager.WorldPosition;
                    _context.dragTime = _dragTime;
                    currentInteractObj.OnDragBegin(_context); 
                    _isDraging = true;
                }
            }

            

            if(_isDraging && Input.GetMouseButtonUp(0))
            {
                if(currentInteractObj != null)
                {
                    _context.mousePosition = mouseManager.WorldPosition;
                    currentInteractObj.OnDragEnd(_context);
                    _dragTime = 0;
                    currentInteractObj = null;
                }
                _isDraging = false;
            }else if(_isDraging)
            {
                _context.mousePosition = mouseManager.WorldPosition;
                _dragTime += Time.deltaTime;
                _context.dragTime = _dragTime;
                currentInteractObj.OnDragUpdate(_context);
            }
        }
    }


}
