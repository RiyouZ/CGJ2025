using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Serialize
{
	[Serializable]
	public class SerializedList<T> : IEnumerable<T>
	{
		[SerializeField]
		private List<T> _value;

		public SerializedList (List<T> collection)
		{
			_value = collection;
		}

		public List<T> ToList ()
		{
			return _value as List<T>;
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return _value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

	}

}
