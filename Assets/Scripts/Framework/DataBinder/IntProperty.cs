using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuGameFramework.DataBind
{
	public class IntProperty : BindProperty<int>, IUIBindProperty
	{

		private UnityAction<string> _callback;
		private HashSet<TMP_InputField> _inputSet;

		public IntProperty ()
		{
			_callback = (value) => 
			{
				if (int.TryParse(value, out int coverValue))
				{
					Value = coverValue;
				}
			};
			Value = 0;
		}

		public IntProperty (int value)
		{
			_callback = (value) =>
			{
				if (int.TryParse(value, out int coverValue))
				{
					Value = coverValue;
				}
			};
			Value = value;
		}

		public void BindToText (TMP_Text textCom)
		{
			if (textCom == null)
			{
				return;
			}

			textCom.text = Value.ToString();
			Action<int> onValueChange = (value) => 
			{
				textCom.text = value.ToString();
			};

			_onValueChange += onValueChange;
			_onValueChangeRefList.Add(onValueChange);
		}

		public void BindToImg (Image imgCom)
		{
			if (imgCom == null)
			{
				return;
			}

			imgCom.fillAmount = Value;
			Action<int> onValueChange = (value) =>
			{
				imgCom.fillAmount = value;
			};

			_onValueChange += onValueChange;
			_onValueChangeRefList.Add(onValueChange);
		}

		public void BindToInputField (TMP_InputField inputFieldCom)
		{
			if (inputFieldCom == null)
			{
				return;
			}

			if (_inputSet == null)
			{
				_inputSet = new HashSet<TMP_InputField> ();
			}

			inputFieldCom.text = Value.ToString();
			_inputSet.Add(inputFieldCom);

			inputFieldCom.onValueChanged.AddListener(_callback);
			Action<int> onValueChange = (value) =>
			{
				inputFieldCom.text = value.ToString();
			};

			_onValueChange += onValueChange;
			_onValueChangeRefList.Add(onValueChange);
		}

		public void BindToUI (UIBehaviour ui)
		{
			switch (ui)
			{
				case TMP_Text text:
					BindToText(text);
					break;
				case TMP_InputField input:
					BindToInputField(input);
					break;
				case Image image:
					BindToImg(image);
					break;
				default:
					Debug.LogError("没有可绑定的UI类型");
					return;
			}
		}

		public void UnBindUI ()
		{
			foreach (var input in _inputSet)
			{
				input.onValueChanged.RemoveListener(_callback);
			}

			_callback = null;
			UnBind();
		}
	}
}

