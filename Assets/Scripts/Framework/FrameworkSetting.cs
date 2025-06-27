using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Core
{
	public enum SystemType
	{
		None,
		OperateSystem,
		BaseSceneSystem,
		DepolySystem
	}

	public enum ComponentType
	{
		None,
		SortingLayer
	}

	public enum SceneType
	{
		ChapterScene,
		BattleScene
	}

	public static	 class UISortLayer
	{
		public const string Hud = "HudUI";
		public const string Scene = "SceneUI";
		public const string Window = "WindowUI";
		public const string Effect = "EffectUI";
		public const string Loading = "LoadingUI";
	}


}