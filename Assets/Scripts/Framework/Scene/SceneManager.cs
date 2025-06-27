using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RuGameFramework.Scene
{
	/// <summary>
	/// 场景切换管理器 主要用于切换场景
	/// </summary>
	public static class SceneManager
	{
		private static MonoBehaviour _runner;
		private static Coroutine _asyncLoadHandle;

		private static WaitForSeconds _waitForSeconds;

		private static bool _isLoading = false;
 
		public static float progress
		{
			get;
			private set;
		}

		public static void Initialization (MonoBehaviour runner, float loadWaitTime)
		{
			_runner = runner;
			_waitForSeconds = new WaitForSeconds (loadWaitTime);
		}

		//同步切换场景的方法
		[Obsolete]
		public static void LoadScene (string name, Action callBack = null)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
			callBack?.Invoke();
			callBack = null;
		}

		//异步切换场景的方法
		public static void LoadSceneAsync (string name, Action onLoading = null,Action onComplate = null)
		{
			// 只可加载一个场景
			if (_asyncLoadHandle != null || _isLoading)
			{
				return;
			}

			_asyncLoadHandle = _runner.StartCoroutine(ReallyLoadSceneAsync(name, onLoading, onComplate));
		}

		public static IEnumerator WaitLoading ()
		{
			if(_isLoading)
				yield return null;
		}

		private static IEnumerator ReallyLoadSceneAsync (string name, Action onLoading, Action onComplate)
		{
			AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
			_isLoading = true;
			onLoading?.Invoke();

			ao.allowSceneActivation = false;
			while (!ao.isDone)
			{
				if (progress >= 0.9f)
				{
					ao.allowSceneActivation = true;
				}
				progress = ao.progress;
				yield return null;
			}
			progress = 1;
			onComplate?.Invoke();
			yield return _waitForSeconds;

			_isLoading = false;
			if (_asyncLoadHandle != null)
			{
				_runner.StopCoroutine(_asyncLoadHandle);
			}
		}
	}
}
