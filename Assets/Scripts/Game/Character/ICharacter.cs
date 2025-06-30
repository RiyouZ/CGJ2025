using System.Collections;
using System.Collections.Generic;
using CGJ2025.SceneCell;
using UnityEngine;

namespace CGJ2025.Character
{
    public interface ICharacter
    {
        public Cell GenCell
        {
            set; get;
        }

        public bool IsGrassRepel
        {
            set; get;
        }

        public GameObject CharacteObject
        {
            get;
        }
        public void OnCellMouseDown(Cell cell);
    }


}
