using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Serialize
{

	public interface IStorageHandler<TData>
	{
		void WriteData (string fullPath, TData data);
		TData ReadData (string fullPath);
	}

}
