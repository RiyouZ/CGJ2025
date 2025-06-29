using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrassUI : MonoBehaviour
{
    public Canvas canvas;

    public List<Image> grassImages = new List<Image>();


    public Material grassUIMaterial;

    [Header("Repel")]
    [SerializeField]
    private float repelLerpSpeed = 5f;
    [SerializeField]
    private float repelMaxAngle = 0.5f;
    [SerializeField]
    private float repelInfluence = 3f;
    // 存储每个草当前角度状态
    private Dictionary<Image, float> currentAngles = new Dictionary<Image, float>();

    private Dictionary<Image, RectTransform> rects = new Dictionary<Image, RectTransform>();



    void Start()
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            grassImages.Add(image);
            image.material = new Material(grassUIMaterial);
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


            float dist = toMouse.magnitude;
            float influence = Mathf.Exp(-dist / 1000f * repelInfluence);
            float side = Mathf.Sign(toMouse.x);
            float targetAngle = side * influence * repelMaxAngle;

            // 插值当前角度
            float angle = Mathf.Lerp(currentAngles[image], targetAngle, Time.deltaTime * repelLerpSpeed);
            currentAngles[image] = angle;

            Debug.Log(dist + " " + influence + " " + side + " " + targetAngle + " " + angle);
            Debug.Log(image.rectTransform.rect);

            image.material.SetVector("_RootWorldPos", image.transform.position);
            //image.material.SetVector("_ManualSwayOffset", new Vector4(angle, 0, 0, 0));
        }
    }
}
