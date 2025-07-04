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

        public bool _isDraging { get; private set; }
        
        public MouseManager mouseManager;
        public TimerManager timerManager;
        public IInteractable currentInteractObj;
        private InteractContext _context;

        public Collider2D CurrentObject;
        private float _dragTime;

        public Camera _mainCamera;

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
            if(currentInteractObj != null && currentInteractObj.IsDestroy)
            {
                currentInteractObj = null;
            }

            if(_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            var pos =  _mainCamera.ScreenToWorldPoint(Input.mousePosition);
			CurrentObject = Physics2D.OverlapPoint(pos, 1 << 8);

            if(Input.GetMouseButtonDown(0) && CurrentObject != null)
            {
                // 已经有抓过的
                if(currentInteractObj != null)
                {
                    return;
                }

                currentInteractObj = CurrentObject.GetComponent<IInteractable>();
                if(currentInteractObj != null && !currentInteractObj.IsDestroy)
                {
                    _context.mousePosition = mouseManager.WorldPosition;
                    _context.dragTime = _dragTime;
                    currentInteractObj.OnDragBegin(_context); 
                    _isDraging = true;
                }
            }
            else if(_isDraging && Input.GetMouseButtonUp(0))
            {
                _isDraging = false;
                if(currentInteractObj != null && !currentInteractObj.IsDestroy)
                {
                    _context.mousePosition = mouseManager.WorldPosition;
                    _dragTime += Time.deltaTime;
                    _context.dragTime = _dragTime;
                    currentInteractObj.OnDragEnd(_context);
                    _dragTime = 0;
                    currentInteractObj = null;
                }
            }
            else if(_isDraging && currentInteractObj != null)
            {
                _context.mousePosition = mouseManager.WorldPosition;
                _dragTime += Time.deltaTime;
                _context.dragTime = _dragTime;

                if(currentInteractObj.IsDestroy)
                {
                    return;
                }
                currentInteractObj.OnDragUpdate(_context);
            }
        }
    }


}
