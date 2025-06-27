using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Data
{
	public interface IRuData
	{
		// 初始化 第一次生成时执行
		public void Init ();

		public void Destory ();
		public IRuData Clone ();
	}
}