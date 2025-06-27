using RuGameFramework.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RuGameFramework.UI
{
	public abstract class RuCanvas : SerializedMonoBehaviour, IRuCanvas
	{
		[DictionaryDrawerSettings]
		[ShowInInspector]
		[SerializeField]
		private Dictionary<string, IRuUICom> _uiComDic = new Dictionary<string, IRuUICom>();

		private string _canvasPath;

		public GameObject UIGameObject
		{
			get => gameObject;
		}

		public string CanvasPath
		{
			get => _canvasPath;
			set => _canvasPath = value;
		}

		private bool _isInteractable;
		public bool IsInteractable
		{
			get => _isInteractable;
			set => _isInteractable = value;
		}

		public Transform CanvasTs => gameObject.transform;

		private Canvas canvas;

		public string UILayer
		{
			get
			{
				TryOverrideCanvasLayer();
				
				return canvas.sortingLayerName;
			}
			set
			{
				TryOverrideCanvasLayer();
				
				canvas.sortingLayerName = value;
				
				Debug.LogWarning($"[RuCanvas] Canvas layer: {canvas.sortingLayerName}, {value}");
			}
		}
		public int UIOrder
		{
			get 
			{
				TryOverrideCanvasLayer();

				return canvas.sortingOrder;
			}
			set
			{
				TryOverrideCanvasLayer();

				canvas.sortingOrder = value;
			}
		}

		protected virtual void Start ()
		{
			InitComponent();
		}

		public abstract void UIDestory ();

		public abstract void Hide ();

		public abstract void HideFast ();

		public abstract void Init ();

		public abstract void UIReset ();

		public abstract void Show ();

		public abstract void ShowFast ();

		private void InitComponent ()
		{
			if (_uiComDic.Count <= 0)
			{
				return;
			}

			foreach (var com in _uiComDic.Values)
			{
				com.Init();
			}
		}

		private void DestoryComponent ()
		{
			if (_uiComDic.Count <= 0)
			{
				return;
			}

			foreach (var com in _uiComDic.Values)
			{
				com.Destroy();
			}
		}
		
		// 尝试获取覆盖sortOrder
		private void TryOverrideCanvasLayer ()
		{
			if (canvas == null)
			{
				canvas = GetComponent<Canvas>();
			}

			if (canvas.overrideSorting == false)
			{
				canvas.overrideSorting = true;
			}
		}
		
		// 获取Canvas内的UI组件
		protected T GetRuUICom<T> (string comName) where T : class, IRuUICom
		{
			if (!_uiComDic.TryGetValue(comName, out IRuUICom uiCom))
			{
				return null;
			}

			return uiCom as T;
		}
		
		// 配置UI参数
		public abstract void Configure (ICanvasArgs args);
	}

}
