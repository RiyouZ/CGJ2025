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

        public Collider2D CurrentObject;
        private float _dragTime;

        private Camera _mainCamera;

        void Start()
        {
            _context = new InteractContext();
            _dragTime = 0;
            _mainCamera = Camera.main;
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
            var pos =  _mainCamera.ScreenToWorldPoint(Input.mousePosition);
			CurrentObject = Physics2D.OverlapPoint(pos, 1 << 8);

            if(Input.GetMouseButtonDown(0) && CurrentObject != null)
            {
                currentInteractObj = CurrentObject.GetComponent<IInteractable>();
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
                    _dragTime += Time.deltaTime;
                    _context.dragTime = _dragTime;
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
