using System;
using System.Collections;
using System.Collections.Generic;
using RuGameFramework;
using RuGameFramework.Input;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;



public class GrassUI : MonoBehaviour
{
    public MouseManager _mouseManager;
    public List<Renderer> grassRenderers = new List<Renderer>();

    public Material sharedMaterial;

    [Header("Repel")]
    [SerializeField]
    public bool isRepelling = true;
    [SerializeField]
    private float repelLerpSpeed = 5f;
    [SerializeField]
    private float repelMaxAngle = 0.5f;
    [SerializeField]
    private float repelInfluence = 3f;



    // 存储每个草当前角度状态
    private Dictionary<Renderer, float> currentAngles = new Dictionary<Renderer, float>();
    // MaterialPropertyBlock 缓存
    private Dictionary<Renderer, MaterialPropertyBlock> blockCache = new Dictionary<Renderer, MaterialPropertyBlock>();

    void Start()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            grassRenderers.Add(renderer);
            renderer.material = sharedMaterial;
        }
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
