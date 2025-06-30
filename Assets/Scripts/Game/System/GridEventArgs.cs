using System.Collections;
using System.Collections.Generic;
using RuGameFramework.Event;
using UnityEngine;

namespace CGJ2025.System
{
    public class GridEventArgs : IGameEventArgs
    {
        public SceneCell.Cell grassCell;
        public Vector2Int genIndex;

        public void Dispose()
        {

        }
    }

}
