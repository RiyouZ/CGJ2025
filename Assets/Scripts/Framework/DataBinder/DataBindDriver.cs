using System.Collections.Generic;
using System.Reflection;
using System;
using RuGameFramework.Data;

namespace RuGameFramework.DataBind
{
	public static class DataBindDriver
	{
		private static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private static Dictionary<Type, IDataDriver> _driverCache = new Dictionary<Type, IDataDriver>();

		// 执行数据绑定
		public static void ExecDataBind (IDataDriver obj)
		{
			var type = obj.GetType();

			// 已绑定过 无需再次绑定
			if (_driverCache.TryGetValue(type, out IDataDriver driver))
			{
				return;
			}

			_driverCache.Add(type, obj);

			// 获取Driver中的字段
			var fieldList = type.GetFields(flags);

			object[] funcArgs = new object[1];

			// 遍历所有字段
			for (int i = 0; i < fieldList.Length; i++)
			{
				// 获取字段的特性
				var attr = fieldList[i].GetCustomAttribute<DataPropertyAttribute>();
				if (attr == null)
				{
					continue;
				}
				
				// 获取绑定的数据
				var data = DataRegistrar.GetData(attr.TargetType);
				if (data == null)
				{
					continue;
				}

				// 获取对应的数据字段
				FieldInfo field = attr.TargetType.GetField(attr.Field);
				if (field == null)
				{
					continue;
				}

				// 获取DataProperty属性的Get
				object dataValue = field.GetValue(data);
				if (dataValue == null)
				{
					continue;
				}

				// 获取绑定的组件
				object comValue = fieldList[i].GetValue(obj);
				if (comValue == null)
				{
					continue;
				}

				MethodInfo bindFunc = field.FieldType.GetMethod("BindToUI", flags);
				funcArgs[0] = comValue;
				bindFunc.Invoke(dataValue, funcArgs);
			}
		}

		// 解绑
		public static void UnBindData (IDataDriver obj)
		{
			var type = obj.GetType();

			// 未绑定过 无需解绑
			if (!_driverCache.TryGetValue(type, out IDataDriver driver))
			{
				return;
			}

			// 获取Driver中的字段
			var fieldList = type.GetFields(flags);

			// 遍历所有字段
			for (int i = 0; i < fieldList.Length; i++)
			{
				// 获取字段的特性
				var attr = fieldList[i].GetCustomAttribute<DataPropertyAttribute>();
				if (attr == null)
				{
					continue;
				}

				// 获取绑定的数据
				var data = DataRegistrar.GetData(attr.TargetType);
				if (data == null)
				{
					continue;
				}

				// 获取对应的数据字段
				FieldInfo field = attr.TargetType.GetField(attr.Field);
				if (field == null)
				{
					continue;
				}

				// 获取DataProperty属性的Get
				object dataValue = field.GetValue(data);
				if (dataValue == null)
				{
					continue;
				}

				MethodInfo bindFunc = field.FieldType.GetMethod("UnBindUI", flags);
				bindFunc.Invoke(dataValue, null);
			}
		}
	}
}


