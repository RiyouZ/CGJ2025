using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace RuGameFramework.Reflection
{
	public static class ReflectionUtility
	{
		private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
		private static HashSet<string> _assemblySet = new HashSet<string>() 
		{
			"Assembly-CSharp"
		};

		public static void AddAssembly (string assembly)
		{
			if (string.IsNullOrEmpty(assembly))
			{
				return;
			}

			_assemblySet.Add(assembly);
		}

		public static Assembly[] GetAssembly ()
		{
			var assembly = Assembly.GetExecutingAssembly();
			
			return null;
		}

		// 获取当前所有继承于接口的对象
		public static List<object> GetObjectsForInterface (Type interfaceType)
		{
			if (!interfaceType.IsInterface)
			{
				return null;
			}
			List<object> ret = null;
			var typeList = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var type in typeList)
			{
				if (!type.IsClass || type.IsAbstract || !interfaceType.IsAssignableFrom(type))
				{
					continue;
				}

				if (ret == null)
				{
					ret = new List<object>();
				}
				ret.Add(type);

			}
			return ret;
		}

		// 获取当前程序集
		public static Type GetTypeForAssembly (string typePath, bool isCache = true)
		{
			Type nType = null;
			foreach (var assembly in _assemblySet)
			{
				string typeName = string.Format("{0},{1}",typePath, assembly);

				if (!isCache)
				{
					nType = Type.GetType(typeName);
					return nType;
				}

				if (!_typeCache.TryGetValue(typeName, out Type type))
				{
					nType = Type.GetType(typeName);
					_typeCache.Add(typeName, nType);
					return nType;
				}
				else
				{
					return type;
				}
			}

			return nType;
		}
	}

}

