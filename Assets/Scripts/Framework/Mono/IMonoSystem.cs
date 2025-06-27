using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Mono
{
	public interface IMonoSystem
	{
		public void MonoStart ();
		public void MonoUpdate ();
		public void MonoFixUpdate ();
		public void MonoDestory ();
	}

}

