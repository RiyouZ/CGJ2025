using System;
using System.Collections;
using System.Collections.Generic;
using RuGameFramework;
using RuGameFramework.Input;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace CGJ2025.System.Grass
{
    public class GrassSystem : MonoBehaviour
    {

        /// <summary>
        /// 回调
        /// </summary>
        public event Action OnStartRepel;
        public event Action OnEndRepel;

        public MouseManager _mouseManager;
        public List<Sprite> plantSpriteList;
        public List<float> plantRandomWeightList;

        [SerializeField]
        private GameObject grassSinglePrefab;

        [SerializeField]
        private float randomRangeX = 3f;
        [SerializeField]
        private float randomRangeY = 0.8f;
        [SerializeField]
        private int plantsCount = 20;

        public List<Renderer> grassRenderers = new List<Renderer>();

        public Material sharedMaterial;

        [Header("Repel")]
        public bool isRepelling = false;
        private float repelLerpSpeed = 5f;
        private float repelMaxAngle = 0.5f;
        private float repelInfluence = 3f;

        [Header("Normal Repel Parameters")]
        [SerializeField]
        private float normalLerpSpeed = 5f;

        [SerializeField]
        private float normalMaxAngle = 0.5f;
        [SerializeField]
        private float normalInfluence = 3f;

        [Header("Clicking Repel Parameters")]
        [SerializeField]
        private float clickingLerpSpeed = 5f;
        [SerializeField]
        private float ClickingMaxAngle = 10f;

        [SerializeField]
        private float clickingInfluence = 1f;

         // 存储每个草当前角度状态
        private Dictionary<Renderer, float> currentAngles = new Dictionary<Renderer, float>();
        // MaterialPropertyBlock 缓存
        private Dictionary<Renderer, MaterialPropertyBlock> blockCache = new Dictionary<Renderer, MaterialPropertyBlock>();

        void Start()
        {

            // Generate Grass
            float totalWeight = 0f;
            foreach (float weight in plantRandomWeightList)
                totalWeight += weight;

            for (int i = 0; i < plantsCount; i++)
            {
                GameObject go = Instantiate(grassSinglePrefab, transform);
                Vector3 pos;
                do
                {
                    float x = UnityEngine.Random.Range(-randomRangeX, randomRangeX);
                    float y = UnityEngine.Random.Range(-randomRangeY, randomRangeY);
                    pos = new Vector3(x, y, 0);
                }
                // 拒绝采样，保证在菱形区域内
                while (Mathf.Abs(pos.x / randomRangeX) + Mathf.Abs(pos.y / randomRangeY) > 1);

                go.transform.localPosition = pos;
                SpriteRenderer rend = go.GetComponent<SpriteRenderer>();
                float randomValue = UnityEngine.Random.Range(0, totalWeight);
                float cumulative = 0f;
                int plantIndex = plantRandomWeightList.Count - 1;
                for (int k = 0; k < plantRandomWeightList.Count; k++)
                {
                    cumulative += plantRandomWeightList[k];
                    if (randomValue < cumulative)
                    {
                        plantIndex = k;
                        break;
                    }
                }

                rend.sprite = plantSpriteList[plantIndex];

                grassRenderers.Add(rend);
            }

            // 初始化材质统一设置
            foreach (var rend in grassRenderers)
            {
                if (rend != null)
                {
                    rend.sharedMaterial = sharedMaterial;
                }
            }
        }

        int GetWeightedRandomIndex(List<float> weights)
        {
            float totalWeight = 0f;
            foreach (float weight in weights)
                totalWeight += weight;

            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < weights.Count; i++)
            {
                cumulative += weights[i];
                if (randomValue < cumulative)
                    return i;
            }

            return weights.Count - 1;
        }

        public void TryStartRepel() {
            if (isRepelling) return;

            Debug.Log("Start Repel");
            isRepelling = true;
            repelLerpSpeed = clickingLerpSpeed;
            repelMaxAngle = ClickingMaxAngle;
            repelInfluence = clickingInfluence;
            OnStartRepel?.Invoke();
        }
        public void TryEndRepel() {
            if (!isRepelling) return;

            Debug.Log("End Repel");
            isRepelling = false;
            repelLerpSpeed = normalLerpSpeed;
            repelMaxAngle = normalMaxAngle;
            repelInfluence = normalInfluence;
            OnEndRepel?.Invoke();
        }

        void Update()
        {
            foreach (var renderer in grassRenderers)
            {
                if (renderer == null) continue;

                // 初始化缓存
                if (!currentAngles.ContainsKey(renderer))
                    currentAngles[renderer] = 0f;
                if (!blockCache.ContainsKey(renderer))
                    blockCache[renderer] = new MaterialPropertyBlock();

                Vector3 rootWorld = renderer.transform.position;
                Vector2 toMouse = _mouseManager.WorldPosition - rootWorld;
                float dist = toMouse.magnitude;
                float influence = Mathf.Exp(-dist * repelInfluence);
                float side = Mathf.Sign(toMouse.x);
                float targetAngle = side * influence * repelMaxAngle;

                // 插值当前角度
                float angle = Mathf.Lerp(currentAngles[renderer], targetAngle, Time.deltaTime * repelLerpSpeed);
                currentAngles[renderer] = angle;

                // 更新属性块
                var block = blockCache[renderer];
                renderer.GetPropertyBlock(block);
                block.SetVector("_RootWorldPos", rootWorld);
                block.SetVector("_ManualSwayOffset", new Vector4(angle, 0, 0, 0));
                renderer.SetPropertyBlock(block);
            }
        }
        
    }


}

