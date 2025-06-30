using System.Collections;
using System.Collections.Generic;
using System;
using Spine.Unity;
using UnityEngine;

namespace CGJ2025.Character
{
	[CreateAssetMenu(fileName = "ScriptableCharacter", menuName = "Character/ScriptableCharacter")]
	public class ScriptableCharacter : ScriptableObject
	{
		public string elvenName;
		[TextArea()]
		public string effectDescript;
		public SkeletonDataAsset skeletonData;
		public List<SkinInfo> skinInfoList = new List<SkinInfo>(); 
		public ScriptableDragCondition dragCondition;
	}
	
	[Serializable]
	public struct SkinInfo
	{
		public Sprite tipSprite;
		public string skinName;
	}


}
