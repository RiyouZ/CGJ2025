using RuGameFramework.Assets;
using RuGameFramework.Input;
using RuGameFramework.Core;
using System.Collections.Generic;
using UnityEngine;
using RuGameFramework.Mono;
using CGJ2025.Scene;
using RuGameFramework.Scene;
using CGJ2025.UI.Tip;
using RuGameFramework.Event;
using CGJ2025.UI;
using CGJ2025.SceneCell;

namespace RuGameFramework
{
	public class App : MonoBehaviour
	{
		public const string EVENT_GAME_COMPLETE = "GameComplete";

		public static readonly float SampleRate = 0.016f;
		private static string GameUIPath = "UI/GameUI";

		public static bool IsComplete;
		private static App _instance;
		public static App Instance => _instance;

		[SerializeField] private GameMono _gameMono;

		[SerializeField] private MouseManager _mouseManager;
		public MouseManager MouseManager => _mouseManager;

		[SerializeField] private TimerManager _timerManager;
		public TimerManager TimerManager => _timerManager;

		private Dictionary<int, GameObject> _dontDestroyDic = new Dictionary<int, GameObject>(8);

		private Timer timer;

		public List<Texture2D> cursorTexList = new List<Texture2D>();

		public Transform GameUI;
		public Transform titleCanvas;
		public Transform endCanvas;
		public TipCanvas tipCanvas;
		public CountCanvas countCanvas;
		public Transform taskCanvas;
		public Camera uiCamera;

		private static readonly Vector3 OutScreen = new Vector3(Screen.width * 5, Screen.height * 5, 0);

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

			Application.targetFrameRate = 120;
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

		public void Restart()
		{
			IsComplete = false;
			endCanvas.gameObject.SetActive(false);
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
		}

		public void Complete()
		{
			IsComplete = true;
			Cursor.visible = true;
			endCanvas.gameObject.SetActive(true);
			EventManager.InvokeEvent(EVENT_GAME_COMPLETE, null);
		}

		public void Quit()
		{
			Application.Quit();
		}

		private void OnGameStartInit ()
		{
			InitManager();

			TimerManager.StartSchedule(TimerManager.TimeType.RealTimeSinceStartUp, SampleRate);
			
			AddDontDestroyList(GameUI.gameObject);
			// RuUI.CreateGameUI(new ResAssetLoadAdapter(this), GameUIPath, OnCreateGameUI);
			ChangeCursorDefault();
		}

		public void ShowEffectTip(Vector3 mousePos, string name, Sprite sprite, string desc)
		{
			Vector3 screenPos = MouseManager.MousePosition;

			Vector2 localPoint;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				tipCanvas.transform.parent.transform as RectTransform,
				screenPos,
				uiCamera,
				out localPoint))
			{
				tipCanvas.rectTransform.anchoredPosition = localPoint + Vector2.right * tipCanvas.offset;
			}
			tipCanvas.SetData(name,sprite, desc);

		}

		public void HideEffectTip()
		{
			if(tipCanvas == null)
			{
				return;
			}
			tipCanvas.rectTransform.anchoredPosition = (Vector2)OutScreen;
		}

		public void ShowCountTip(Cell cell)
		{	
			(countCanvas.transform as RectTransform).anchoredPosition = Vector2.zero;
			countCanvas.UpdateData(cell.rabbitCnt, cell.flowerCnt);
		}

		public void HideCountTip()
		{
			if(countCanvas == null)
			{
				return;
			}

			if((countCanvas.transform as RectTransform).anchoredPosition == (Vector2)OutScreen)
			{
				return;
			}

			(countCanvas.transform as RectTransform).anchoredPosition = (Vector2)OutScreen;
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
			SceneManager.LoadSceneAsync("GameScene", null, ()=>
			{
				IsComplete = false;
				countCanvas.gameObject.SetActive(true);
				countCanvas.Initialization();
				titleCanvas.gameObject.SetActive(false);
				taskCanvas.gameObject.SetActive(true);
				tipCanvas.gameObject.SetActive(true);
				tipCanvas.Initialize();
				tipCanvas.rectTransform.anchoredPosition = OutScreen;
			});
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
