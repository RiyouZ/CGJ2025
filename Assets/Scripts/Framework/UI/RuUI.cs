using RuGameFramework;
using RuGameFramework.Assets;
using RuGameFramework.Core;
using RuGameFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RuUI
{
	public static readonly int ErrorOrder = -1;
	public static readonly int DefaultCapacity = 4;
	// 开启的Canvas栈
	private static Stack<IRuCanvas> _canvasStk;
	// Canvas缓存
	private static Dictionary<string, IRuCanvas> _canvasCache;

	private class CanvasLayerInfo
	{
		public Canvas canvas;
		public int uiOrder;

		public CanvasLayerInfo ()
		{
			canvas = null;
			uiOrder = 0;
		}
	}

	private static Dictionary<string, CanvasLayerInfo> _canvasLayerMapping = new Dictionary<string, CanvasLayerInfo>
		{
			{UISortLayer.Scene, new CanvasLayerInfo()},
			{UISortLayer.Hud, new CanvasLayerInfo()},
			{UISortLayer.Window, new CanvasLayerInfo()},
			{UISortLayer.Effect, new CanvasLayerInfo()},
			{UISortLayer.Loading, new CanvasLayerInfo()},
		};

	// UI渲染相机
	private static Camera _uiCamera;
	public static Camera UICamera => _uiCamera;

	// 同时加载的Canvas数量
	private static int _multiLoad;
	private static Dictionary<string, Coroutine> _asyncLoadHandleDic;

	// 加载适配器
	private static IAsyncAssetLoadAdapter _loadAdapter;

	private static Vector2 _originArchorPos;
	private static Vector2 _originSizeDelta;
	private static Vector2 _originPivot;
	private static Vector2 _originArchorMax;
	private static Vector2 _originArchorMin;
	private static Vector3 _originLoacalPos;
	private static Vector3 _originLoacalScale;


	public static void CreateGameUI (IAsyncAssetLoadAdapter assetLoadAdapter, string gameUIPath, Action<GameObject> onCreate = null, int multiLoad = 2)
	{
		if (assetLoadAdapter == null)
		{
			return;
		}

		if (_loadAdapter == null)
		{
			_loadAdapter = assetLoadAdapter;
		}

		_canvasStk = new Stack<IRuCanvas>(DefaultCapacity);
		_canvasCache = new Dictionary<string, IRuCanvas>(DefaultCapacity);
		_multiLoad = multiLoad;

		if (_asyncLoadHandleDic == null)
		{
			_asyncLoadHandleDic = new Dictionary<string, Coroutine>(_multiLoad + 1);
		}

		if (!_asyncLoadHandleDic.ContainsKey(gameUIPath))
		{
			_asyncLoadHandleDic.Add(gameUIPath, null);
		}

		_asyncLoadHandleDic[gameUIPath] = _loadAdapter.AsyncInstantiate(gameUIPath, (gameUIObj) =>
		{
			OnCreateGameUI(gameUIObj, onCreate);
			if (_asyncLoadHandleDic != null)
			{
				_loadAdapter.StopCoroutine(_asyncLoadHandleDic[gameUIPath]);
				_asyncLoadHandleDic.Remove(gameUIPath);
			}
		});
	}

	private static void OnCreateGameUI (GameObject gameUIObj, Action<GameObject> onCreate = null)
	{
		var gameUITs = gameUIObj.GetComponent<Transform>();
		gameUITs.name = "GameUI";

		_canvasLayerMapping[UISortLayer.Scene].canvas = gameUITs.Find(UISortLayer.Scene).GetComponent<Canvas>();
		_canvasLayerMapping[UISortLayer.Hud].canvas = gameUITs.Find(UISortLayer.Hud).GetComponent<Canvas>();
		_canvasLayerMapping[UISortLayer.Window].canvas = gameUITs.Find(UISortLayer.Window).GetComponent<Canvas>();
		_canvasLayerMapping[UISortLayer.Effect].canvas = gameUITs.Find(UISortLayer.Effect).GetComponent<Canvas>();
		_canvasLayerMapping[UISortLayer.Loading].canvas = gameUITs.Find(UISortLayer.Loading).GetComponent<Canvas>();

		_uiCamera = gameUITs.Find("UICamera").GetComponent<Camera>();

		InitCanvasHandle(_canvasLayerMapping[UISortLayer.Scene].canvas);
		InitCanvasHandle(_canvasLayerMapping[UISortLayer.Hud].canvas);
		InitCanvasHandle(_canvasLayerMapping[UISortLayer.Window].canvas);
		InitCanvasHandle(_canvasLayerMapping[UISortLayer.Effect].canvas);
		InitCanvasHandle(_canvasLayerMapping[UISortLayer.Loading].canvas);

		onCreate?.Invoke(gameUIObj);
	}

	// 显示UI
	public static void ShowCanvas (Action<IRuCanvas> onShow, string uiPath, string uiLayer)
	{
		// 存在缓存
		if (_canvasCache.TryGetValue(uiPath, out IRuCanvas ruCanvas))
		{
			ruCanvas.UIReset();
			ruCanvas.ShowFast();
			UpdateUIParent(ruCanvas);
			MarkCanvasOpen(ruCanvas);
			onShow(ruCanvas);
			return;
		}

		if (!_asyncLoadHandleDic.ContainsKey(uiPath) && _asyncLoadHandleDic.Count <= _multiLoad)
		{
			_asyncLoadHandleDic.Add(uiPath, null);
		}

		Transform parentTs = null;
		if (uiLayer != null)
		{
			parentTs = GetUIParentByUILayer(uiLayer);
		}

		_asyncLoadHandleDic[uiPath] = _loadAdapter.AsyncInstantiate(uiPath, (prefab) =>
		{
			//  实例引用计数
			GameObject canvas = prefab;

			var canvasCom = canvas.GetComponent<IRuCanvas>();
			if (canvasCom == null)
			{
				return;
			}

			// 设置Parent

			if (canvas.transform.parent == null)
			{
				var rectTs = canvas.GetComponent<RectTransform>();
				SaveRect(rectTs);
				canvas.transform.SetParent(parentTs);
				LoadRect(rectTs);
			}

			OnCanvasLoad(canvasCom, uiPath, uiLayer);
			onShow(canvasCom);

			if (_asyncLoadHandleDic != null)
			{
				_loadAdapter.StopCoroutine(_asyncLoadHandleDic[uiPath]);
				_asyncLoadHandleDic.Remove(uiPath);
			}
			// 为了与Prefab一致 实例化时就需要设置parent
		}, parentTs);
	}

	private static void SaveRect (RectTransform rect)
	{
		_originArchorPos = rect.anchoredPosition;
		_originArchorMax = rect.anchorMax;
		_originArchorMin = rect.anchorMin;
		_originPivot = rect.pivot;
		_originSizeDelta = rect.sizeDelta;
		_originLoacalPos = rect.localPosition;
		_originLoacalScale = rect.lossyScale;
	}

	private static void LoadRect (RectTransform rect)
	{
		rect.anchoredPosition = _originArchorPos;
		rect.anchorMax = _originArchorMax;
		rect.anchorMin = _originArchorMin;
		rect.pivot = _originPivot;
		rect.sizeDelta = _originSizeDelta;
		rect.localPosition = _originLoacalPos;
		rect.localScale = _originLoacalScale;
	}

	public static void ShowCanvas (IRuCanvas ruCanvas)
	{
		var canvas = GetCanvasCache(ruCanvas.CanvasPath);
		if (canvas == null)
		{
#if UNITY_EDITOR
			Debug.LogWarning($"[RuUI] Canvas cache not found: {ruCanvas.CanvasPath}]");
#endif
			return;
		}

		canvas.UIReset();
		canvas.ShowFast();
	}

	private static void OnCanvasLoad (IRuCanvas ruCanvas, string canvasPath, string uiLayer = null)
	{
		if (ruCanvas == null)
		{
			return;
		}

		if (uiLayer != null)
		{
			ruCanvas.UILayer = uiLayer;
			Debug.LogWarning($"[RuUI] OnCanvasLoad {canvasPath} UILayer: {ruCanvas.UILayer}");
		}

		if (ruCanvas.CanvasPath == null)
		{
			ruCanvas.CanvasPath = canvasPath;
		}

		// 计算Order
		int order = GetUIOrderByUILayer(ruCanvas.UILayer);
		ruCanvas.UIOrder = order;

		// 添加缓存
		AddCanvasCache(canvasPath, ruCanvas);
		// 添加到打开的UI栈
		MarkCanvasOpen(ruCanvas);
		// 第一次打开
		ruCanvas.Show();
	}

	private static void InitCanvasHandle (Canvas canvas)
	{
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.worldCamera = _uiCamera;
	}

	// 标记Canvas为打开
	private static void MarkCanvasOpen (IRuCanvas canvas)
	{
		if (canvas == null)
		{
			return;
		}

		// 栈顶的Canvas可以互动
		canvas.IsInteractable = true;
		if (_canvasStk.Count > 0)
		{
			var topCanvas = _canvasStk.Peek();
			topCanvas.IsInteractable = false;
		}
		_canvasStk.Push(canvas);
	}

	// 关闭最上层Canvas
	public static void CloseTopCanvas (bool isFast = false)
	{
		if (_canvasStk.Count <= 0)
		{
			return;
		}

		var topCanvas = _canvasStk.Pop();
		topCanvas.IsInteractable = false;

		DecreaseUIOrderByUILayer(topCanvas.UILayer);

		HideCanvasHandle(topCanvas, isFast);
	}

	private static void AddCanvasCache (string canvasPath, IRuCanvas canvas)
	{
		if (_canvasCache.ContainsKey(canvasPath))
		{
			return;
		}
		canvas.Init();
		_canvasCache.Add(canvasPath, canvas);
	}

	private static void RemoveCanvasCache (string canvasPath)
	{
		if (!_canvasCache.TryGetValue(canvasPath, out IRuCanvas canvas))
		{
			return;
		}
		canvas.UIDestory();
		_canvasCache.Remove(canvasPath);
	}

	private static IRuCanvas GetCanvasCache (string canvasPath)
	{
		if (!_canvasCache.TryGetValue(canvasPath, out IRuCanvas canvas))
		{
			return null;
		}
		return canvas;
	}

	// 添加到对应的parent 更新Order
	private static void UpdateUIParent (IRuCanvas canvas)
	{
		if (canvas == null)
		{
			return;
		}

		string layer = canvas.UILayer;
		Transform canvasTs = canvas.CanvasTs;

		if (canvasTs == null)
		{
			return;
		}

		Transform parent = GetUIParentByUILayer(layer);
		if (parent != null && canvasTs.parent != parent)
		{
			canvasTs.parent = parent;
		}

		int order = GetUIOrderByUILayer(layer);

		if (order != ErrorOrder)
		{
			canvas.UIOrder = order;
		}

	}

	private static Transform GetUIParentByUILayer (string uiLayer)
	{
		if (uiLayer == null)
		{
			return null;
		}

		if (!_canvasLayerMapping.TryGetValue(uiLayer, out var canvasInfo))
		{
#if UNITY_EDITOR
			Debug.LogError($"[RuUI] {uiLayer} 没有物体");
#endif
			return null;
		}

		return canvasInfo.canvas.transform;
	}

	private static int GetUIOrderByUILayer (string uiLayer)
	{
		if (uiLayer == null)
		{
			return ErrorOrder;
		}

		if (!_canvasLayerMapping.TryGetValue(uiLayer, out var canvasInfo))
		{
#if UNITY_EDITOR
			Debug.LogError($"[RuUI] {uiLayer} 没有物体");
#endif
			return ErrorOrder;
		}

		canvasInfo.uiOrder++;

		return canvasInfo.uiOrder;
	}

	private static void DecreaseUIOrderByUILayer (string uiLayer)
	{
		if (uiLayer == null)
		{
			return;
		}

		if (!_canvasLayerMapping.TryGetValue(uiLayer, out var canvasInfo))
		{
#if UNITY_EDITOR
			Debug.LogError($"[RuUI] {uiLayer} 没有物体");
#endif
			return;
		}

		canvasInfo.uiOrder--;
	}

	// 销毁当前所有的Canvas 不会触发隐藏逻辑
	public static void ClearAllCanvas ()
	{
		_canvasStk.Clear();
		_canvasStk = null;

		foreach (var canvas in _canvasCache.Values)
		{
			canvas.UIDestory();
			_loadAdapter.Destroy(canvas.UIGameObject);
		}
		_canvasCache.Clear();
	}

	// 隐藏UI
	public static void HideCanvas (string canvasPath, bool isFast = false)
	{
		if (!_canvasCache.TryGetValue(canvasPath, out IRuCanvas canvas))
		{
			return;
		}

		// 没有打开的 canvas
		var topCanvas = _canvasStk.Peek();
		if (topCanvas == canvas)
		{
			CloseTopCanvas(isFast);
			return;
		}

		HideCanvasHandle(canvas);
	}

	public static void HideCanvas (IRuCanvas canvas, bool isFast = false)
	{
		if (!_canvasCache.TryGetValue(canvas.CanvasPath, out IRuCanvas ruCanvas))
		{
			return;
		}
		HideCanvasHandle(canvas, isFast);
	}

	// 隐藏UI处理
	private static void HideCanvasHandle (IRuCanvas canvas, bool isFast = false)
	{
		if (!isFast)
		{
			canvas.Hide();
			RemoveCanvasCache(canvas.CanvasPath);
			// 释放Canvas
			_loadAdapter.Destroy(canvas.UIGameObject);
		}
		else
		{
			canvas.HideFast();
		}
	}
}
