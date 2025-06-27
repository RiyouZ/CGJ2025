using RuGameFramework.SortingLayer;
using System;
using UnityEngine;


namespace RuGameFramework.Effect
{
	[RequireComponent(typeof(GameSortLayerCom))]
	public class Effect2DAniCom : MonoBehaviour, IEffect
	{
		private Action<IEffect> _onCreate;
		private Action<IEffect> _onDestory;

		private GameSortLayerCom _sortLayerCom;
		public Animator animator;

		private bool _isLoop;
		public bool IsLoop
		{
			set
			{
				_isLoop = value;
			}
			get
			{
				return _isLoop;
			}
		}

		private string _prefabPath;

		public string PrefabPath
		{
			get => _prefabPath;
			set => _prefabPath = value;
		}

		public GameObject GameObject => this.gameObject;

		public Transform Transform => this.transform;

		public string EffectName
		{
			set => transform.name = value;
		}

		void Start ()
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}

			if (_sortLayerCom == null)
			{
				_sortLayerCom = GetComponent<GameSortLayerCom>();
				_sortLayerCom.SortLayerName = "Effect";
			}

		}

		void Update ()
		{
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !IsLoop)
			{
				_onDestory?.Invoke(this);
			}
        }

		public IEffect OnCreate (Action<IEffect> onCreate)
		{
			_onCreate = onCreate;
			return this;
		}

		public IEffect OnDestory (Action<IEffect> onDestory)
		{
			_onDestory = onDestory;
			return this;
		}

		public void Destory ()
		{
			_onDestory?.Invoke(this);
			_onCreate = null;
			_onDestory = null;
			RuEffect2D.DestoryEffect(this);
		}

		public void Invoke ()
		{
			_onCreate?.Invoke(this);
			gameObject.SetActive(true);
		}
	}
}

