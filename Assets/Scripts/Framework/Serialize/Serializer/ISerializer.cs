using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Serialize
{

	public interface ISerializer { }
	public interface ISerializer<TByte> : ISerializer
	{
		// 序列化
		TByte Serialize<TData> (TData data);	

		// 反序列化
		TData Deserialize<TData> (TByte tbyte);

		// 反序列化覆盖现有对象
		void  DeserializeOverwrite<TData> (TByte tbyte, TData writeObj);


	}

}
