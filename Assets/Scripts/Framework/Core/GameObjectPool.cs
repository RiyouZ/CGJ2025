using RuGameFramework.Assets;
using RuGameFramework.Core;
using RuGameFramework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public class GameObjectPool : IObjectPool<GameObject>
	{
		public struct Config
		{
			public IAsyncAssetLoadAdapter assetsProvider;
			public ITimeManager timeKeeper;
		}

		private Config _config;

		private const string POOL_NAME = "Pool";
		private const int MULTI_EXPANSION = 2;
		private string _objPath;

		private int _count;
		private int _capacity;

		private Transform _objPoolTs;
		private GameObject _prefab;
		private List<GameObject> _freeList;
		private float _shrinkTime;
		private float _updateShrinkTime;

		private AutoIdGenerator _autoIdGenerator;

		private bool _shrinkLock;
		private bool _isPreload;
		public bool IsPreload => _isPreload;

		private IRuTimer _checkTimer;
		private float _timeSimple;

		private static Dictionary<int, Coroutine> _asyncLoadHandleDic;

		private bool _isNotAssetLoad;

		private Action<GameObject> _initAction;

		public Action<GameObject> InitAction
		{
			set
			{
				_initAction = value;
			}
		}

		private Action<GameObject> _collAction;
		public Action<GameObject> CollAction
		{
			set
			{
				_collAction = value;
			}
		}

		public GameObjectPool (GameObject prefab, Config config, Transform parent = null, int capacity = 2, bool canShrink = false, float timeSimple = 0.3f, float updateShrinkTime = 120f)
		{
			_config.assetsProvider = config.assetsProvider;

			_prefab = prefab;
			_autoIdGenerator = new AutoIdGenerator(0);
			_shrinkLock = false;
			_capacity = capacity;
			_count = 0;
			_freeList = new List<GameObject>(_capacity);
			_isPreload = true;

			if (parent == null)
			{
				string poolName = String.Format("{0}_{1}", prefab.name, POOL_NAME);
				_objPoolTs = new GameObject(poolName).transform;
			}
			else
			{
				_objPoolTs = parent;
			}

			for (int i = 0; i < _capacity; i++)
			{
				int autoId = _autoIdGenerator.GetAutoId();
				var obj = CreateObj(autoId, _objPath);
				_collAction?.Invoke(obj);
				_freeList.Add(obj);
			}

			_count = _freeList.Count;

			if (!canShrink)
			{
				return;
			}

			_updateShrinkTime = updateShrinkTime;
			_shrinkTime = updateShrinkTime;
			_timeSimple = timeSimple;
			_config.timeKeeper = config.timeKeeper;

			_checkTimer = _config.timeKeeper.SetInterval(Update, _timeSimple, 0);

		}

		public GameObjectPool (string objPath, Config config, Transform parent = null, int capacity = 2, bool canShrink = false, float timeSimple = 0.3f, float updateShrinkTime = 120f)
		{
			_config.assetsProvider = config.assetsProvider;
			_config.timeKeeper = config.timeKeeper;

			_objPath = objPath;
			_autoIdGenerator = new AutoIdGenerator(0);
			_shrinkLock = false;
			_capacity = capacity;
			_count = 0;
			_freeList = new List<GameObject>(_capacity);
			_isPreload = false;
			_asyncLoadHandleDic = new Dictionary<int, Coroutine>(_capacity);

			if (parent == null)
			{
				string poolName = String.Format("{0}_{1}", objPath, POOL_NAME);
				_objPoolTs = new GameObject(poolName).transform;
			}
			else
			{
				_objPoolTs = parent;
			}

			for (int i = 0; i < _capacity; i++)
			{
				int autoId = _autoIdGenerator.GetAutoId();
				CreateObjAsync(autoId, _objPath, (obj) =>
				{
					_collAction?.Invoke(obj);
					_freeList.Add(obj);

					_count++;

					if (_count == _capacity)
					{
						_isPreload = true;
					}
				});
			}

			if (!canShrink)
			{
				return;
			}

			_updateShrinkTime = updateShrinkTime;
			_shrinkTime = updateShrinkTime;
			_timeSimple = timeSimple;

			_checkTimer = _config.timeKeeper.SetInterval(Update, _timeSimple, 0);
		}

		private void Update (float deltaTime)
		{
			_updateShrinkTime -= deltaTime;
			if (_updateShrinkTime <= 0)
			{
				_updateShrinkTime = _shrinkTime;
				Shrink();
			}
		}

		public void Collection (GameObject obj)
		{
			obj.SetActive(false);
			obj.transform.parent = _objPoolTs;
			_collAction?.Invoke(obj);

			_freeList.Add(obj);
			_count++;
		}

		public void Dispose ()
		{
			if (_checkTimer != null)
			{
				_checkTimer.Release();
			}

			foreach (var obj in _freeList)
			{
				_config.assetsProvider.Destroy(obj);
			}

			_freeList.Clear();
			_freeList = null;

			_initAction = null;
			_collAction = null;

			_autoIdGenerator = null;
		}

		public IEnumerator WaitPoolLoad ()
		{
			float time = Time.time;
			while (!IsPreload)
			{
				if (Time.time - time > 10f)
				{
#if UNITY_EDITOR
					Debug.LogError($"[{_objPath} Pool.Load] Timeout !");
#endif
					yield break;
				}
				yield return null;
			}
		}

		public GameObject Spawn ()
		{
			if (!_isPreload)
			{
#if UNITY_EDITOR
				Debug.LogWarning($"[GameObjectPool.Spawn] The Pool Is Not Preloaded");
#endif
				return null;
			}

			_updateShrinkTime = _shrinkTime;

			GameObject obj = null;
			if (_count <= 0)
			{
				int preCapacity = _capacity;
				_capacity = Mathf.Max(1, _capacity * MULTI_EXPANSION);

				for (int i = preCapacity; i < _capacity; i++)
				{
					int autoId = _autoIdGenerator.GetAutoId();
					GameObject newObj = null;
					if (_isNotAssetLoad)
					{
						newObj = CreateObjNotCounter(autoId, _prefab);
					}
					else
					{
						newObj = CreateObj(autoId, _objPath);
					}

					_freeList.Add(newObj);
					_count++;
				}
			}

			obj = _freeList[_count - 1];
			_freeList.RemoveAt(_count - 1);
			_initAction?.Invoke(obj);
			obj.SetActive(true);

			_count--;
			return obj;
		}

		// 2倍缩容
		public void Shrink ()
		{
			if (!IsPreload)
			{
				return;
			}

			if (_count <= _capacity / MULTI_EXPANSION)
			{
				return;
			}

			_shrinkLock = true;

			_capacity = (int)_capacity / MULTI_EXPANSION;

			int shrinkCount = 0;
			int shrinkIndex = _count / MULTI_EXPANSION;
			for (int i = _freeList.Count - 1; i >= shrinkIndex; i--)
			{
				_config.assetsProvider.Destroy(_freeList[i]);
				shrinkCount++;
			}
			_freeList.RemoveRange(shrinkIndex, shrinkCount);
			_count = _freeList.Count;

			_shrinkLock = false;
		}

		private GameObject CreateObjNotCounter (int autoId, GameObject prefab)
		{
			var obj = GameObject.Instantiate(prefab);
			obj.name = String.Format("{0}_{1}", prefab, autoId);
			obj.SetActive(false);
			obj.transform.parent = _objPoolTs;

			return obj;
		}

		private GameObject CreateObj (int autoId, string prefab)
		{
			var obj = _config.assetsProvider.Instantiate(prefab);
			obj.name = String.Format("{0}_{1}", prefab, autoId);
			obj.transform.parent = _objPoolTs;
			obj.SetActive(false);

			return obj;
		}

		private void CreateObjAsync (int autoId, string prefab, Action<GameObject> onCreate)
		{
			if (!_asyncLoadHandleDic.ContainsKey(autoId))
			{
				_asyncLoadHandleDic.Add(autoId, null);
			}

			if (_asyncLoadHandleDic[autoId] != null)
			{
				_config.assetsProvider.StopCoroutine(_asyncLoadHandleDic[autoId]);
				_asyncLoadHandleDic[autoId] = null;
			}

			_asyncLoadHandleDic[autoId] = _config.assetsProvider.AsyncInstantiate(_objPath, (gameObject) =>
			{
				if (gameObject == null)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"[GameObjectPool.CreateObjAsync] {_objPath} AsyncInstantiate Fail");
#endif
					return;
				}

				gameObject.name = String.Format("{0}_{1}", prefab, autoId);
				gameObject.SetActive(false);
				gameObject.transform.parent = _objPoolTs;

				onCreate?.Invoke(gameObject);

				if (_asyncLoadHandleDic[autoId] != null)
				{
					_config.assetsProvider.StopCoroutine(_asyncLoadHandleDic[autoId]);
					_asyncLoadHandleDic[autoId] = null;
				}
			});
		}
	}

}


