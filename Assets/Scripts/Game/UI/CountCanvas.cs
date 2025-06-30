using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CGJ2025.UI
{
    public class CountCanvas : MonoBehaviour
    {
        public Text txtRabbit;
        public Text txtFlower;

        public void Initialization()
        {
            txtRabbit = transform.Find("TxtRabbit").GetComponent<Text>();
            txtFlower = transform.Find("TxtFlower").GetComponent<Text>();
        }

        public void UpdateData(int rabbitCnt, int flowerCnt)
        {
            txtRabbit.text = $"兔精灵 x {rabbitCnt}  减少孕育时间";
            txtFlower.text = $"雏菊精灵 x {flowerCnt}  增加花园同时孕育两个\n小精灵的概率";
        }
        
    }

}
