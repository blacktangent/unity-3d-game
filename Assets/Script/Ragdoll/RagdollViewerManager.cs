using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RagdollViewerManager : MonoBehaviour
{
    public Transform modelHolder;
    public float targetSize = 1.0f;
    public Slider scaleSlider;

    private GameObject currentModel;
    public TextMeshProUGUI uidText;          // UID表示用UI
    public TextMeshProUGUI modelNameText;    // モデル名表示用UI

    public float fadeDuration = 1.0f;
    public float targetScale = 1.0f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // ゆっくり拡大

    private Coroutine currentEffectCoroutine;
    private Vector3 baseScale = Vector3.one; // スケーリングの基準値


    void Start()
    {
        string uid = NFCDataHolder.Instance?.scannedUID;
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("UIDが読み込まれていません。");
            return;
        }

        string modelName = UIDModelMapper.GetModelNameFromUID(uid);
        if (string.IsNullOrEmpty(modelName))
        {
            Debug.LogWarning($"UID {uid} に対応するモデルが見つかりません。");
            return;
        }

        Debug.Log("UID from NFCDataHolder: " + uid);
        Debug.Log("Model name from UID: " + modelName);

        // UIテキストに表示
        if (uidText != null) uidText.text = "UID: " + uid;
        if (modelNameText != null) modelNameText.text = "Model: " + modelName;

        GameObject prefab = Resources.Load<GameObject>("Models/" + modelName);
        if (prefab != null)
        {
            foreach (Transform child in modelHolder)
                Destroy(child.gameObject);

            // wrapper（回転補正）
            GameObject wrapper = new GameObject("ModelWrapper");
            wrapper.transform.SetParent(modelHolder, false);
            wrapper.transform.localPosition = Vector3.zero;
            wrapper.transform.localRotation = Quaternion.Euler(0, 90, 0);
            wrapper.transform.localScale = Vector3.one;

            // scaleRoot（スケーリング対象）
            GameObject scaleRoot = new GameObject("ScaleRoot");
            scaleRoot.transform.SetParent(wrapper.transform, false);
            scaleRoot.transform.localPosition = Vector3.zero;
            scaleRoot.transform.localRotation = Quaternion.identity;
            scaleRoot.transform.localScale = Vector3.one;

            // pivotAdjuster（ピボット補正）
            GameObject pivotAdjuster = new GameObject("PivotAdjuster");
            pivotAdjuster.transform.SetParent(scaleRoot.transform, false);
            pivotAdjuster.transform.localPosition = Vector3.zero;
            pivotAdjuster.transform.localRotation = Quaternion.identity;
            pivotAdjuster.transform.localScale = Vector3.one;

            // モデルのインスタンス化
            currentModel = Instantiate(prefab, pivotAdjuster.transform);

            // Bounds取得と中心補正（pivotAdjusterに適用）
            Bounds bounds = CalculateBounds(currentModel);
            pivotAdjuster.transform.localPosition = -bounds.center;

            // 自動スケーリング（scaleRootに対して）
            float scaleFactor = AutoScaleModel(scaleRoot);
            targetScale = 1f;
            baseScale = scaleRoot.transform.localScale;

            // フェードインエフェクト開始
            StartFadeInEffect(scaleRoot);

            // スライダー設定
            if (scaleSlider != null)
            {
                scaleSlider.onValueChanged.RemoveAllListeners();
                scaleSlider.minValue = 0.5f;
                scaleSlider.maxValue = 1.5f;
                scaleSlider.value = 1f;
                scaleSlider.onValueChanged.AddListener((value) =>
                {
                    if (scaleRoot != null)
                    {
                        scaleRoot.transform.localScale = baseScale * value;
                    }
                });
            }

            FindObjectOfType<ModelController>()?.UpdateTarget();
        }
        else
        {
            Debug.LogWarning($"モデル '{modelName}' がResourcesに見つかりません。");
        }
    }

    float AutoScaleModel(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 1f;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxSize == 0) return 1f;

        float scaleFactor = targetSize / maxSize;
        model.transform.localScale = Vector3.one * scaleFactor;

        Debug.Log($"モデルを自動スケーリングしました: 最大サイズ={maxSize}, スケール倍率={scaleFactor}");
        return scaleFactor;
    }

    /*Bounds CalculateBounds(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        // ワールド中心をローカルに変換
        Vector3 localCenter = model.transform.InverseTransformPoint(bounds.center);
        bounds.center = localCenter;
        return bounds;
    }*/

    Bounds CalculateBounds(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        Vector3 localCenter = model.transform.InverseTransformPoint(bounds.center);
        bounds.center = localCenter;
        return bounds;
    }



    void StartFadeInEffect(GameObject model)
    {
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);

        currentEffectCoroutine = StartCoroutine(FadeInEffectCoroutine(model));
    }

    IEnumerator FadeInEffectCoroutine(GameObject scaleRoot)
    {
        float elapsed = 0f;

        scaleRoot.transform.localScale = baseScale * 0.01f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            float scale = scaleCurve.Evaluate(t);
            scaleRoot.transform.localScale = baseScale * scale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        scaleRoot.transform.localScale = baseScale;
        currentEffectCoroutine = null;
    }

}

