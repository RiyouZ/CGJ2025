using RuGameFramework.SortingLayer;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Effect
{
	[RequireComponent(typeof(GameSortLayerCom))]
	public class Effect2DSpineCom : MonoBehaviour, IEffect
	{
		private string _effect = "FX";
		public string EffectName
		{
			set => _effect = value;
		}
		public Transform Transform => this.transform;
		public GameObject GameObject => this.gameObject;

		private GameSortLayerCom _sortLayerCom;
		public SkeletonAnimation skeleton;

		private Action<IEffect> _onCreate;
		private Action<IEffect> _onDestory;

		private string _prefabPath;

		public string PrefabPath
		{
			get => _prefabPath;
			set => _prefabPath = value;
		}

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

		private void OnAnimeComplete (TrackEntry trackEntry)
		{
			_onDestory?.Invoke(this);
			
			skeleton.AnimationState.ClearTrack(0);
			skeleton.Skeleton.SetBonesToSetupPose();

			skeleton.AnimationState.Complete -= OnAnimeComplete;
		}

		void Start ()
		{
			if (_sortLayerCom == null)
			{
				_sortLayerCom = GetComponent<GameSortLayerCom>();
				_sortLayerCom.SortLayerName = "Effect";
			}
		}

		public void Invoke ()
		{
			// Spine 在Start阶段未加载完成无法获取 
			if (skeleton == null)
			{
				skeleton = GetComponent<SkeletonAnimation>();
			}

			_onCreate?.Invoke(this);
			skeleton.AnimationState.SetAnimation(0, _effect, IsLoop);
			gameObject.SetActive(true);

			if (IsLoop)
			{
				return;
			}

			skeleton.AnimationState.Complete += OnAnimeComplete;
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
			RuEffect2D.DestoryEffect (this);
		}
	}
}

