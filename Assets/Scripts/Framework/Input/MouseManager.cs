using System;
using System.Drawing;
using UnityEngine;

namespace RuGameFramework.Input
{
	public class MouseManager : MonoBehaviour , IDisposable
	{
		private Camera _mainCamera = null;

		private Vector3 _mousePosition = Vector2.zero;

		public MouseData _mouseData = new MouseData();

		private GameObject _cursorObj;
		public GameObject Cursor
		{
			set
			{
				_cursorObj = value;
			}
		}

		public Vector3 MousePosition
		{
			get
			{
				return _mousePosition;
			}
		}

		public Vector3 WorldPosition
		{
			get
			{
				return _mouseData.worldPosistion;
			}
		}

		public GameObject CurrentObject {
			get
			{
				if (_mouseData.collider == null)
				{
					return null;
				}

				return _mouseData.collider.gameObject;
			}
			
		}

		// Start is called before the first frame update
		void Start ()
		{
			Init();
		}

		private void Init ()
		{
			_mainCamera = Camera.main;
			_mousePosition = UnityEngine.Input.mousePosition;
		}

		private void UpdateMouseInfo ()
		{
			_mousePosition = UnityEngine.Input.mousePosition;
			if (_mainCamera == null)
			{
				_mainCamera = Camera.main;
			}
			_mouseData.worldPosistion.z = -_mainCamera.transform.position.z;
			_mouseData.worldPosistion = _mainCamera.ScreenToWorldPoint(_mousePosition);

			_mouseData.collider = Physics2D.OverlapPoint(_mouseData.worldPosistion);
			if(_mouseData.collider != null)
			{
				Debug.Log($"[MouseManager.Hit] {_mouseData.collider.name} Hit");
			}

			
			if (_cursorObj != null)
			{
				_cursorObj.transform.position = _mouseData.worldPosistion;
			}

		}

		private void UpdateMouseButtonState ()
		{
			if (UnityEngine.Input.GetMouseButtonDown(0))
			{
				_mouseData.LeftState = MouseButtonState.Down;
			} 
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				_mouseData.LeftState = MouseButtonState.Up;
			}

			if (UnityEngine.Input.GetMouseButtonDown(1))
			{
				_mouseData.RightState = MouseButtonState.Down;
			} 
			else if (UnityEngine.Input.GetMouseButtonUp(1))
			{
				_mouseData.RightState = MouseButtonState.Up;
			}
		}

		void Update ()
		{
			UpdateMouseInfo();
			UpdateMouseButtonState();
		}

		public void Dispose ()
		{
			_mouseData = null;
			_mainCamera = null;
		}
	}

}
