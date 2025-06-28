using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace CGJ2025.Character
{
	[CreateAssetMenu(fileName = "ScriptableCharacter", menuName = "Character/ScriptableCharacter")]
	public class ScriptableCharacter : ScriptableObject
	{
		public SkeletonDataAsset skeletonData;
	}


}
