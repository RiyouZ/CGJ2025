using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace RuGameFramework.Assets
{
	// Resources用作敏捷开发 不做资源管理
	public class ResManager 
	{
		public struct AssetInfo
		{
			public string path;
			public Action<GameObject> callback;
			public AssetInfo (string path, Action<GameObject> callback)
			{
				this.path = path;
				this.callback = callback;
			}
		}

		private static int Capacity = 16;

		private static int LoadPreframe = 3;

		private static ResManager _instance;
		public static ResManager Instance => _instance ?? new ResManager();

		private MonoBehaviour _runner;

		public MonoBehaviour Runner 
		{
			set => _runner = value;
		}

		private Dictionary<string, UnityEngine.Object> _assetsCache = new Dictionary<string, UnityEngine.Object>(Capacity);

		private Dictionary<string, Coroutine> _loadAsyncHandleDic = new Dictionary<string, Coroutine>(Capacity);

		private Dictionary<string, bool> _loadLockDic = new Dictionary<string, bool>(Capacity);

		private Queue<AssetInfo> _loadQueue = new Queue<AssetInfo> (Capacity);

		private bool _isLoading = false;

		//同步加载资源
		public T LoadAssets<T> (string name) where T : UnityEngine.Object
		{
			if (_assetsCache.TryGetValue(name, out UnityEngine.Object obj))
			{
				if (obj is GameObject)
				{
					return GameObject.Instantiate(_assetsCache[name] as T);
				}

				return obj as T;
			}

			T res = UnityEngine.Resources.Load<T>(name);

			_assetsCache.Add(name, res);
			if (res is GameObject)
			{
				return GameObject.Instantiate(res);
			}

			return res;
		}

		//异步加载资源
		public IEnumerator LoadAssets<T> (string name, Action<T> callback, Action<T> onFail = null) where T : UnityEngine.Object
		{
			if (_assetsCache.TryGetValue(name, out UnityEngine.Object obj))
			{
				if (obj is GameObject)
				{
					var gobj = GameObject.Instantiate(_assetsCache[name]);
					callback(gobj as T);
					yield break;
				}

				callback(obj as T);
				yield break;
			}

			// 正在加载
			if (_loadAsyncHandleDic.TryGetValue(name, out Coroutine handle) || (_loadLockDic.ContainsKey(name) && _loadLockDic[name]))
			{
				string waitCortName = $"{name}_wait";
				var cortWait =_runner.StartCoroutine(WaitLoadAsync(waitCortName, callback));
				_loadAsyncHandleDic.Add(waitCortName, cortWait);
				yield return cortWait;
			}

			//开启异步加载的协程
			var cort = App.Instance.StartCoroutine(ReallyLoadAsync(name, callback));
			_loadAsyncHandleDic.Add(name, cort);
		}

		public void UnLoadAssets (string path)
		{
			if (!_assetsCache.TryGetValue(path, out var asset))
			{
				return;
			}
			UnityEngine.Resources.UnloadAsset(asset);
		}

		public void UnLoadAssets (UnityEngine.Object asset)
		{
			UnityEngine.Resources.UnloadAsset(asset);
		}

		// 不缓存加载同时加载多个Resources会卡死
		private IEnumerator ReallyLoadAsync()
		{
			while(_isLoading) yield return null;

			_isLoading = true;

			while (_loadQueue.Count > 0)
			{
				int count = 0;
				while (count < LoadPreframe && _loadQueue.Count > 0)
				{
					var info = _loadQueue.Dequeue();
					yield return ReallyLoadAsync<GameObject>(info.path, info.callback);
					count++;
				}
				yield return null;
			}

			_isLoading = false;
		}

		private IEnumerator ReallyLoadAsync<T> (string name, Action<T> callback) where T : UnityEngine.Object
		{
			if (!_loadLockDic.ContainsKey(name))
			{
				_loadLockDic.Add(name, true);
			}

			_loadLockDic[name] = true;

			ResourceRequest r = UnityEngine.Resources.LoadAsync<T>(name);

			yield return r;

			if (!_assetsCache.TryGetValue(name, out var asset))
			{
				_assetsCache.Add(name, r.asset);
			}

			_loadLockDic[name] = false;

			if (r.asset is GameObject)
			{
				var asyncHandle = GameObject.InstantiateAsync<GameObject>(r.asset as GameObject);
				yield return asyncHandle;
				callback?.Invoke(asyncHandle.Result[0] as T);
			}
			else
			{
				callback?.Invoke(r.asset as T);
			}

			if (_loadAsyncHandleDic.TryGetValue(name, out Coroutine handle))
			{
				_runner.StopCoroutine(handle);
				_loadAsyncHandleDic.Remove(name);
			}
		}

		private IEnumerator WaitLoadAsync<T> (string assetName, Action<T> onComplate) where T : UnityEngine.Object
		{
			if (!_loadLockDic.ContainsKey(assetName))
			{
				yield break;
			}

			while (_loadLockDic[assetName])
				yield return null;

			if (!_assetsCache.TryGetValue(assetName, out UnityEngine.Object asset))
			{
#if UNITY_EDITOR
				Debug.LogWarning($"[ResourcesManager.WaitLoad] Not Found Load Asset {assetName}");
#endif
				yield break;
			}

			if (asset is GameObject)
			{
				var asyncHandle = GameObject.InstantiateAsync<GameObject>(asset as GameObject);
				yield return asyncHandle;
				onComplate?.Invoke(asyncHandle.Result[0] as T);
			}
			else
			{
				onComplate?.Invoke(asset as T);
			}

			if (_loadAsyncHandleDic.TryGetValue($"{assetName}_wait", out Coroutine waitHandle))
			{
				// 优化掉MonoMgr 非必要强依赖
				_runner.StopCoroutine(waitHandle);
				_loadAsyncHandleDic.Remove(assetName);
			}
		}

		// 释放缓存
		public void ClearCache ()
		{
			_assetsCache.Clear();
		}

		public void Destroy (GameObject prefab)
		{
			GameObject.Destroy(prefab);
		}

		public GameObject Instantiate (GameObject prefab)
		{
			return GameObject.Instantiate(prefab);
		}

		public GameObject Instantiate (string prefab)
		{
			if (_assetsCache.TryGetValue(prefab, out var gameObj))
			{
				if (!( gameObj is GameObject ))
				{
					return null;
				}

				return GameObject.Instantiate(gameObj as GameObject);
			}

			return LoadAssets<GameObject>(prefab);
		}

		public IEnumerator AsyncInstantiate (string prefab, Action<GameObject> onComplate)
		{
			if (_loadLockDic.ContainsKey(prefab) && _loadLockDic[prefab])
			{
				yield return WaitLoadAsync<GameObject>(prefab, onComplate);
				yield break;
			}

			if (_assetsCache.TryGetValue(prefab, out var gameObj))
			{
				if (!( gameObj is GameObject ))
				{
					yield break;
				}

				AsyncInstantiateOperation<GameObject> handle = GameObject.InstantiateAsync<GameObject>(gameObj as GameObject);
				yield return handle;

				onComplate(handle.Result[0]);
				yield break;
			}

			_loadQueue.Enqueue(new AssetInfo(prefab, onComplate));

			yield return ReallyLoadAsync();
		}
	}
}
