using RuGameFramework.Util;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuGameFramework.UI
{
	public class RuButton : MonoBehaviour, 
		IRuUICom,
		IUIGroupable,
		IPointerClickHandler, 
		IPointerEnterHandler, 
		IPointerExitHandler
	{
		public enum RuButtonState
		{
			Default,
			Enter,
			Hover,
			Exit
		}

		[LabelText("按钮显示状态")]
		[ShowInInspector]
		private RuButtonState _buttonState;
		public RuButtonState State => _buttonState;

		[LabelText("未激活显示的对象")]
		[SerializeField]
		private GameObject _deActiveObj;
		public GameObject DeActiveObj => _deActiveObj;
		
		[LabelText("激活显示的对象")]
		[SerializeField]
		private GameObject _activeObj;
		public GameObject ActiveObj => _activeObj;

		private AutoIdGenerator _autoIdGen = new AutoIdGenerator(0);

		private Action<RuButton> _onButtonClick;
		private Action<RuButton> _onButtonEnter;
		private Action<RuButton> _onButtonExit;

		private Action<RuButton> _onButtonEnterShow;
		public Action<RuButton> OnButtonEnterShow
		{
			set
			{
				_onButtonEnterShow = value;	
			}
		}	

		private Action<RuButton> _onButtonExitShow;
		public Action<RuButton> OnButtonExitShow
		{
			set
			{
				_onButtonExitShow = value;
			}
		}

		// TODO 点击延迟
		private float _cdTime;

		private int _groupIndex;

		public int GroupIndex
		{
			get => _groupIndex;
			set => _groupIndex = value;
		}

		void Start ()
		{
			_buttonState = RuButtonState.Default;
		}

		public void Destroy ()
		{

		}

		public void Init ()
		{
			
		}

		public void OnPointerClick (PointerEventData eventData)
		{
			_onButtonClick?.Invoke(this);
		}

		public void OnPointerEnter (PointerEventData eventData)
		{
			_onButtonEnter?.Invoke(this);
			UpdateButtonShow(RuButtonState.Enter);
		}

		public void OnPointerExit (PointerEventData eventData)
		{
			_onButtonExit?.Invoke(this);
			UpdateButtonShow(RuButtonState.Exit);
		}

		private void UpdateButtonShow (RuButtonState state)
		{
			if (_buttonState == state)
			{
				return;
			}

			_buttonState = state;
			switch (_buttonState)
			{
				case RuButtonState.Enter:
					OnEnterShow();
					break;
				case RuButtonState.Exit:
					OnExitShow();
					break;
			}
		}

		protected virtual void OnEnterShow ()
		{
			_onButtonEnterShow?.Invoke(this);
		}

		protected virtual void OnExitShow ()
		{
			_onButtonExitShow?.Invoke(this);
		}

		public void ChangeOn ()
		{
			
		}

		public void ChangeOff ()
		{
			
		}

		public Event.EventHandle<Action<RuButton>> AddBtnClickedListener (Action<RuButton> action)
		{

			var handle = new Event.EventHandle<Action<RuButton>>(_autoIdGen.GetAutoId(), action);
			_onButtonClick += action;
			return handle;
		}

		public void RemoveBtnClickedListener (Action<RuButton> action)
		{
			if (action == null)
			{
				return;
			}

			_onButtonClick -= action;
		}

		public void RemoveBtnClickedListener (Event.EventHandle<Action<RuButton>> handle)
		{
			_autoIdGen.RecycleAutoId(handle.eventId);
			_onButtonClick -= handle.action as Action<RuButton>;
		}

	}
}

