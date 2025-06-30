using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CGJ2025.UI.Tip
{
    public class TipCanvas : MonoBehaviour
    {

        public Text txtName;
        public Image imgIcon;
        public Text txtDesc;
        public Vector2 offset;

        public RectTransform rectTransform
        {
            get; private set;
        }

        void Start()
        {
            
        }

        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            txtName = transform.Find("TxtName").GetComponent<Text>();
            imgIcon = transform.Find("ImgIcon").GetComponent<Image>();
            txtDesc = transform.Find("TxtDescript").GetComponent<Text>();
        }

        public void SetData(string name, Sprite icon, string desc)
        {
            txtName.text = name;
            imgIcon.sprite = icon;
            txtDesc.text = desc;
        }
    }

}
