using RuGameFramework.Assets;
using RuGameFramework.Input;
using RuGameFramework.Core;
using System.Collections.Generic;
using UnityEngine;
using RuGameFramework.Mono;
using CGJ2025.Scene;
using RuGameFramework.Scene;

namespace RuGameFramework
{
	public class App : MonoBehaviour
	{
		public static readonly float SampleRate = 0.016f;
		private static string GameUIPath = "UI/GameUI";

		private static App _instance;
		public static App Instance => _instance;

		[SerializeField] private GameMono _gameMono;

		[SerializeField] private MouseManager _mouseManager;
		public MouseManager MouseManager => _mouseManager;

		[SerializeField] private TimerManager _timerManager;
		public TimerManager TimerManager => _timerManager;

		private Dictionary<int, GameObject> _dontDestroyDic = new Dictionary<int, GameObject>(8);

		private Timer timer;

		public GameScene gameScene;

		public List<Texture2D> cursorTexList = new List<Texture2D>();

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
			OnGameStartInit();
		}

		void Start ()
		{

		}

		public void ChangeCursorDefault()
		{
			Texture2D tex = cursorTexList[1];
			Vector2 hotspot = new Vector2(tex.width / 2f, tex.height / 2f);
			Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
		}

		public void ChangeCursorHold()
		{
			Texture2D tex = cursorTexList[0];
			Vector2 hotspot = new Vector2(tex.width / 2f, tex.height / 2f);
			Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
		}

		private void OnGameStartInit ()
		{
			InitManager();

			TimerManager.StartSchedule(TimerManager.TimeType.RealTimeSinceStartUp, SampleRate);
			
			// RuUI.CreateGameUI(new ResAssetLoadAdapter(this), GameUIPath, OnCreateGameUI);
			ChangeCursorDefault();
		}

		private void OnCreateGameUI (GameObject uiObj)
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

			SceneManager.Initialization(this, 0);
		}

		public void LoadGameScene()
		{
			SceneManager.LoadSceneAsync("GameScene");
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
