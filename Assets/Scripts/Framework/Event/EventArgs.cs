using RuGameFramework.Args;
using System;


namespace RuGameFramework.Event
{
	public interface IGameEventArgs : IGameArgs {}

	public class NullArgs : IGameEventArgs
	{
		public void Dispose ()
		{

		}
	}

}
