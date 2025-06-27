using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuGameFramework.Mono
{
	public class GameMono : MonoBehaviour
	{
		public static int ErrorHandle = -1;
		private static int MaxCapacity = 16;
		private static Util.AutoIdGenerator _autoIdGen = new Util.AutoIdGenerator(0, MaxCapacity);
		private static Dictionary<int, LinkedListNode<IMonoSystem>> _monoNodeDic = new Dictionary<int, LinkedListNode<IMonoSystem>>(MaxCapacity);
		private static LinkedList<IMonoSystem> _monoSystemList = new LinkedList<IMonoSystem>();

		void Update ()
		{
			foreach (var monoSys in _monoSystemList)
			{
				monoSys.MonoUpdate();
			}	
		}

		void FixUpdate ()
		{
			foreach (var monoSys in _monoSystemList)
			{
				monoSys.MonoFixUpdate();
			}
		}

		private static LinkedListNode<IMonoSystem> GetNodeById (int id)
		{
			if (!_monoNodeDic.TryGetValue(id, out LinkedListNode<IMonoSystem> node))
			{
				return null;
			}

			return node;
		}

		public static MonoHandle AddToMono (IMonoSystem monoSys)
		{
			MonoHandle monoHandle = new MonoHandle();
			var autoId = _autoIdGen.GetAutoId();

			var node = GetNodeById(autoId);

			if (node != null)
			{
				monoHandle.id = ErrorHandle;
				return monoHandle;
			}

			node = new LinkedListNode<IMonoSystem>(monoSys);

			if (_monoNodeDic.ContainsKey(autoId))
				{
				_monoNodeDic[autoId] = node;
			}
			else
			{
				_monoNodeDic.Add(autoId, node);
			}

			monoSys.MonoStart();
			_monoSystemList.AddLast(node);

			return monoHandle;
		}

		public static void RemoveToMono (MonoHandle monoHandle)
		{
			var node =GetNodeById(monoHandle.id);
			if (node == null)
			{
				return;
			}

			if (_monoNodeDic.ContainsKey(monoHandle.id))
			{
				_monoNodeDic[monoHandle.id] = null;
			}

			node.Value.MonoDestory();
			_monoSystemList.Remove(node);
		}
	}

}
