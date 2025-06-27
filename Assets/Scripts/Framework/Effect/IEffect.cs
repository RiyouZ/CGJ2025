using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Effect
{
	public interface IEffect 
	{
		public string EffectName
		{
			set;
		}

		public Transform Transform
		{
			get;
		}

		public GameObject GameObject
		{
			get;
		}

		public string PrefabPath
		{
			set; get;
		}

		public IEffect OnCreate (Action<IEffect> onCreate);

		public IEffect OnDestory (Action<IEffect> onDestory);

		// 手动唤醒
		public void Invoke ();

		// 手动销毁
		public void Destory ();
		
	}
}

