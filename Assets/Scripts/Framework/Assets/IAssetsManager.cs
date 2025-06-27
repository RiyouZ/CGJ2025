using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Assets
{
	public interface IAssetsManager
	{
		public void LoadAsset<T> (string path, Action<T> onComplate, Action onFail = null) where T : UnityEngine.Object;
		public void Destroy (GameObject prefab);
		public GameObject Instantiate (GameObject prefab);
		public GameObject Instantiate (string prefab);

		public IEnumerator AsyncInstantiate (string prefab, Action<GameObject> onComplate, Transform parent = null);

	}

}
