using RuGameFramework.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.UI
{
	public class RuUIGroup : SerializedMonoBehaviour, IRuUICom
	{
		[LabelText("Group元素列表")]
		[ShowInInspector]
		[SerializeField]
		private List<IUIGroupable> _groupList = new List<IUIGroupable>();
		public List<IUIGroupable> GroupList => _groupList;

		[LabelText("是否是多选")]
		[ShowInInspector]
		private bool _isMultiChoice;

		public void ActiveItem (int index, bool isOn)
		{
			if (!_isMultiChoice)
			{
				SingleChoiceHandle(index, isOn);
			}
			else
			{
				MultiChoiceHandle(index, isOn);
			}
		}

		public IUIGroupable GetGroupItem (int index)
		{
			var groupItem = _groupList[index];
			if (groupItem == null)
			{
				return null;
			}

			groupItem.GroupIndex = index;

			return groupItem;
		}

		private void SingleChoiceHandle (int index, bool isOn)
		{
			var item = GetGroupItem (index);

			InvokeChange(item, isOn);

			UpdateOrtherItem(item, !isOn);
		}

		private void MultiChoiceHandle (int index, bool isOn)
		{
			var item = GetGroupItem(index);

			InvokeChange(item, isOn);
		}


		private void UpdateOrtherItem (IUIGroupable curItem, bool isOn)
		{
			for (int i = 0; i <= _groupList.Count; i++ )
			{
				if (i == curItem.GroupIndex)
				{
					continue;
				}

				InvokeChange(_groupList[i], isOn);
			}
		}

		private void InvokeChange (IUIGroupable item, bool isOn)
		{
			if (isOn)
			{
				item.ChangeOn();
			}
			else
			{
				item.ChangeOff();
			}
		}

		public void Init ()
		{
			
		}

		public void Destroy ()
		{
			
		}
	}
}

