using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Serialize
{
	public class JsonSerializer : ISerializer<string>
	{
			

		public TData Deserialize<TData> (string tbyte)
		{
			return JsonUtility.FromJson<TData>(tbyte);
		}

		public void DeserializeOverwrite<TData> (string tbyte, TData writeObj)
		{
			JsonUtility.FromJsonOverwrite(tbyte, writeObj);
		}

		public string Serialize<TData> (TData data)
		{
			return JsonUtility.ToJson(data);			
		}

	}
}

