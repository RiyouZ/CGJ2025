using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Serialize
{
	public class RuSerializer<TData, TByte> where TData : IDataSerializable
	{
		private ISerializer<TByte> _serializer;
		private IStorageHandler<TByte> _storageHandler;

		public RuSerializer (ISerializer<TByte> serializer, IStorageHandler<TByte> storageHandler)
		{
			_serializer = serializer;
			_storageHandler = storageHandler;
		}

		public void Save(string fullPath, TData data)
		{
			TByte serialzebyte =  _serializer.Serialize(data);
			_storageHandler.WriteData(fullPath, serialzebyte);
		}

		public TData Load (string fullPath)
		{
			var data = _storageHandler.ReadData(fullPath);
			return _serializer.Deserialize<TData>(data);
		}

		public void Load (string fullPath, TData obj)
		{
			var data = _storageHandler.ReadData(fullPath);
			_serializer.DeserializeOverwrite(data, obj);
		}

	}
}

