using RuGameFramework.Serialize;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RuGameFramework.Serialize
{
	public class FileTextStorageHandler : IStorageHandler<string>
	{
		public string ReadData (string fullPath)
		{
			if (!File.Exists(fullPath))
			{
#if UNITY_EDITOR
				Debug.LogError($"[FileStorageHandler.ReadData] {fullPath} Not Exists");
#endif
			}

			return File.ReadAllText(fullPath, Encoding.UTF8);
		}

		public void WriteData (string fullPath, string data)
		{
			File.WriteAllText(fullPath, data, Encoding.UTF8);
		}
	}

}
