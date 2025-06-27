using System;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Serialize
{
	[Serializable]
	public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<TKey> _keyList;
		[SerializeField]
		private List<TValue> _valueList;

		public SerializedDictionary () : base()
		{
			_keyList = new List<TKey>();
			_valueList = new List<TValue>();
		}

		public SerializedDictionary (int capacity) : base(capacity)
		{
			_keyList = new List<TKey>(capacity);
			_valueList = new List<TValue>(capacity);
		}

		public SerializedDictionary (IDictionary<TKey, TValue> dictinary) : base(dictinary)
		{
			_keyList = new List<TKey>(dictinary.Count);
			_valueList = new List<TValue>(dictinary.Count);
		}


		public void OnAfterDeserialize ()
		{
			Clear();
			for (int i = 0; i < _keyList.Count; i++)
			{
				this.Add(_keyList[i], _valueList[i]);
			}
		}

		public void OnBeforeSerialize ()
		{
			_keyList.Clear();
			_valueList.Clear();
			foreach (var pair in this)
			{
				_keyList.Add(pair.Key);
				_valueList.Add(pair.Value);
			}
		}
	}

}
