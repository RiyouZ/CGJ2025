using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrassUI : MonoBehaviour
{
    public Canvas canvas;

    public List<Image> grassImages = new List<Image>();


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


    [Header("Repel")]
    public bool isRepelling = false;
    private float repelLerpSpeed = 5f;
    private float repelMaxAngle = 0.5f;
    private float repelInfluence = 3f;
        // 存储每个草当前角度状态
    private Dictionary<Image, float> currentAngles = new Dictionary<Image, float>();

    private Dictionary<Image, RectTransform> rects = new Dictionary<Image, RectTransform>();
    // MaterialPropertyBlock 缓存
    private Dictionary<Image, MaterialPropertyBlock> blockCache = new Dictionary<Image, MaterialPropertyBlock>();


    public void TryStartRepel()
    {
        if (isRepelling) return;

        Debug.Log("Start Repel");
        isRepelling = true;
        repelLerpSpeed = clickingLerpSpeed;
        repelMaxAngle = ClickingMaxAngle;
        repelInfluence = clickingInfluence;
    }
    public void TryEndRepel() {
        if (!isRepelling) return;

        Debug.Log("End Repel");
        isRepelling = false;
        repelLerpSpeed = normalLerpSpeed;
        repelMaxAngle = normalMaxAngle;
        repelInfluence = normalInfluence;
    }
    void Start()
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            grassImages.Add(image);
        }
    }
    void Update()
    {

        foreach (var image in grassImages)
        {
            if (image == null) continue;


            // 初始化缓存
            if (!currentAngles.ContainsKey(image))
                currentAngles[image] = 0f;
            if (!blockCache.ContainsKey(image))
                blockCache[image] = new MaterialPropertyBlock();

            if (!rects.ContainsKey(image))
                rects[image] = image.transform.GetComponent<RectTransform>();


            // Converts mouse position to local position relative to uiRoot
            Vector2 toMouse;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rects[image],
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out toMouse
                );
            Debug.Log(toMouse);

            float dist = toMouse.magnitude;
            float influence = Mathf.Exp(-dist * repelInfluence);
            float side = Mathf.Sign(toMouse.x);
            float targetAngle = side * influence * repelMaxAngle;

            // 插值当前角度
            float angle = Mathf.Lerp(currentAngles[image], targetAngle, Time.deltaTime * repelLerpSpeed);
            currentAngles[image] = angle;

            // 更新属性块
            // var block = blockCache[image];
            // image.GetPropertyBlock(block);
            // block.SetVector("_RootWorldPos", image.transform.position);
            // block.SetVector("_ManualSwayOffset", new Vector4(angle, 0, 0, 0));
            // image.SetPropertyBlock(block);
        }
    }
}
