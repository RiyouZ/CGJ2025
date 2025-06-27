using RuGameFramework.Assets;
using RuGameFramework.Input;
using RuGameFramework.Core;
using System.Collections.Generic;
using UnityEngine;
using RuGameFramework.Mono;

namespace RuGameFramework
{
	public class App : MonoBehaviour
	{
		private static string GameUIPath = "UI/GameUI";

		private static App _instance;
		public static App Instance => _instance;

		private GameMono _gameMono;

		private MouseManager _mouseManager;
		public MouseManager MouseManager => _mouseManager;

		private TimerManager _timerManager;
		public TimerManager TimerManager => _timerManager;

		private Dictionary<int, GameObject> _dontDestroyDic = new Dictionary<int, GameObject>(8);

		private Timer timer;

		void Awake ()
		{
			if (_instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				_instance = this;
			}

			DontDestroyOnLoad(this);

			Application.targetFrameRate = 60;
		}

		void Start ()
		{
			OnGameStartInit();
		}

		// Addressable初始化完成
		private void OnGameStartInit ()
		{
			InitManager();

			TimerManager.StartSchedule(TimerManager.TimeType.RealTimeSinceStartUp, 0.016f);
			// 创建UI
			RuUI.CreateGameUI(new ResAssetLoadAdapter(this), GameUIPath, OnCreateGameUI);
			RegisterData();
		}

		// UI创建完毕
		private void OnCreateGameUI (GameObject uiObj)
		{
			
		}

		private void GameStart ()
		{

		}

		void Update ()
		{

		}

		private void InitManager ()
		{
			if (_gameMono == null)
			{
				_gameMono = gameObject.AddComponent<GameMono>();
			}

			if (_mouseManager == null)
			{
				_mouseManager = gameObject.AddComponent<MouseManager>();
			}

			if (_timerManager == null)
			{
				_timerManager = gameObject.AddComponent<TimerManager>();
			}
		}

		private void RegisterData ()
		{

		}

		private void UnRegisterData ()
		{

		}

		public void AddDontDestroyList (GameObject obj)
		{
			if (obj == null)
			{
				return;
			}

			if (_dontDestroyDic.TryGetValue(obj.GetInstanceID(), out var dontDestroyObj))
			{
				return;
			}

			this._dontDestroyDic.Add(obj.GetInstanceID(), obj);	
			DontDestroyOnLoad(obj);
		}
		
		public void DestroyDontDestroyList (GameObject obj)
		{
			if (obj == null)
			{
				return;
			}
			
			if (!_dontDestroyDic.TryGetValue(obj.GetInstanceID(), out var dontDestroyObj))
			{
				return;
			}
			
			Destroy(obj);
			this._dontDestroyDic.Remove(obj.GetInstanceID());	
		}

		public void ClearDontDestroyList ()
		{
			if (this._dontDestroyDic == null)
			{
				return;
			}

			foreach (var obj in _dontDestroyDic.Values)
			{
				DestroyImmediate(obj);
			}

			this._dontDestroyDic.Clear();
			this._dontDestroyDic = null;
		}

	}

}
