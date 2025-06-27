using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.UI
{
	public interface IRuCanvas
	{
		// 绑定的UI游戏对象 
		public GameObject UIGameObject {get;}
		// UI对象的路径
		public string CanvasPath {set; get;}
		// 可交互的
		public bool IsInteractable {set; get;}
		public Transform CanvasTs {get;}
		// UI层级名
		public string UILayer {set; get;}
		// UI的SortOrder
		public int UIOrder {set; get;}

		// 初始化
		public void Init ();

		// 重置UI
		public void UIReset ();

		// 销毁
		public void UIDestory ();

		// 显示
		public void Show ();

		// 隐藏
		public void Hide ();
		
		public void ShowFast ();
		public void HideFast ();
		
		// 设置参数
		public void Configure (ICanvasArgs args);

	}
}

