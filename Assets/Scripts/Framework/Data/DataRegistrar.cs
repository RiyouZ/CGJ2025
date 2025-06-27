using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuGameFramework.Data
{
	public static class DataRegistrar
	{
		private static int DefaultCapacity = 16;
		private static Dictionary<Type, IRuData> _dataDic = new Dictionary<Type, IRuData>(DefaultCapacity);

		public static void RegisterData (Type dataType)
		{
			if (_dataDic.TryGetValue(dataType, out IRuData data))
			{
				return;
			}

			var ruData = Activator.CreateInstance(dataType) as IRuData;
			ruData.Init();

			_dataDic.Add(dataType, ruData);
		}

		public static void RegisterDataLazy (Type dataType)
		{
			if (_dataDic.TryGetValue(dataType, out IRuData data))
			{
				return;
			}

			_dataDic.Add(dataType, null);
		}

		public static void RegisterData<T> (bool isLazy = false) where T : IRuData
		{
			if (!isLazy)
			{
				RegisterData(typeof(T));
				return;
			}

			RegisterDataLazy(typeof(T));
		}

		public static void UnRegisterData<T> () where T : IRuData
		{
			UnRegisterData(typeof(T));
		}

		public static void UnRegisterData (Type dataType)
		{
			if (!_dataDic.TryGetValue(dataType, out IRuData data))
			{
				return;
			}
			_dataDic.Remove(dataType);
		}

		public static IRuData GetData (Type dataType)
		{
			if (!_dataDic.TryGetValue(dataType, out IRuData data))
			{
#if UNITY_EDITOR
				Debug.LogError($"未注册类型 {dataType.Name}");
#endif
				return null;
			}

			return data;
		}

		public static T GetData<T> (bool isLazy = false) where T : class, IRuData
		{
			if (!isLazy)
			{
				return GetData(typeof(T)) as T;
			}
			return GetDataLazy(typeof(T)) as T;
		}

		public static IRuData GetDataLazy (Type dataType)
		{
			if (!_dataDic.TryGetValue(dataType, out IRuData data))
			{
#if UNITY_EDITOR
				Debug.LogError($"未注册类型 {dataType.Name}");
#endif
				return null;
			}

			if (data == null)
			{
				data = Activator.CreateInstance(dataType) as IRuData;
				data.Init();
				_dataDic[dataType] = data;
			}
			return data;
		}

		public static IRuData GetClone<T> (bool isLazy = false) where T : IRuData
		{
			if (!isLazy)
			{
				return GetClone(typeof(T));
			}

			return GetCloneLazy(typeof(T));
		}

		public static IRuData GetClone (Type dataType)
		{
			var data = GetData(dataType);
			if (data == null)
			{
				return null;
			}
			return data.Clone();
		}


		public static IRuData GetCloneLazy (Type dataType)
		{
			var data = GetDataLazy(dataType);
			if (data == null)
			{
				return null;
			}

			return data.Clone();
		}
	}

}
