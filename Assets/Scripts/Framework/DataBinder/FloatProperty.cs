using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuGameFramework.DataBind
{
	public class FloatProperty : BindProperty<float>, IUIBindProperty
	{
		private UnityAction<string> _callback;
		private HashSet<TMP_InputField> _inputSet;

		public FloatProperty ()
		{
			_callback = (value) =>
			{
				if (float.TryParse(value, out float coverValue))
				{
					Value = coverValue;
				}
			};
			Value = 0;
		}

		public FloatProperty (float value)
		{
			_callback = (value) =>
			{
				if (float.TryParse(value, out float coverValue))
				{
					Value = coverValue;
				}
			};
			Value = value;
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

		public void BindToText (TMP_Text textCom)
		{
			if (textCom == null)
			{
				return;
			}

			textCom.text = Value.ToString();
			Action<float> onValueChange = (value) =>
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
			Action<float> onValueChange = (value) =>
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
				_inputSet = new HashSet<TMP_InputField>();
			}

			_inputSet.Add(inputFieldCom);

			inputFieldCom.text = Value.ToString();
			inputFieldCom.onValueChanged.AddListener(_callback);
			Action<float> onValueChange = (value) =>
			{
				inputFieldCom.text = value.ToString();
			};

			_onValueChange += onValueChange;
			_onValueChangeRefList.Add(onValueChange);
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
