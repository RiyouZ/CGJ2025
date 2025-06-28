using RuGameFramework.Core;
using RuGameFramework.Util;
using UnityEngine;

namespace RuGameFramework.SortingLayer
{
	public class GameSortLayerCom : MonoBehaviour
	{
		public bool isStatic;
		[SerializeField ]private string _sortLayerName = string.Empty;
		public string SortLayerName
		{
			get
			{
				return _sortLayerName; 
			}
			set
			{
				_sortLayerName = value;

				if (_spriteRenderer != null)
				{
					_spriteRenderer.sortingLayerName = _sortLayerName;	
				}

				if (_meshRenderer != null)
				{
					_meshRenderer.sortingLayerName = _sortLayerName;
				}
			}
		}

		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private MeshRenderer _meshRenderer;

		void Start ()
		{
			InitComponent();
			UpdateSpriteSortLayer();
			UpdateMeshRendererSortLayer();
		}

		void LateUpdate ()
		{
			if(isStatic)
			{
				return;
			}
			UpdateSpriteSortLayer();
			UpdateMeshRendererSortLayer();
		}

		public void UpdateSpriteSortLayer ()
		{
			if (_spriteRenderer == null)
			{
				return;
			}

			int sortOrder = SortLayerUtil.YAxisConverSortOrderValue(transform.position.y);
			_spriteRenderer.sortingOrder = sortOrder;
		}

		public void UpdateMeshRendererSortLayer ()
		{
			if (_meshRenderer == null)
			{
				return;
			}
			int sortOrder = SortLayerUtil.YAxisConverSortOrderValue(transform.position.y);
			_meshRenderer.sortingOrder = sortOrder;
		}

		public void InitComponent ()
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			if (_meshRenderer != null)
			{
				_meshRenderer.sortingLayerName = SortLayerName;
				return;
			}

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (_spriteRenderer != null)
			{
				_spriteRenderer.sortingLayerName = SortLayerName;
				return;
			}
		}
	}

}

