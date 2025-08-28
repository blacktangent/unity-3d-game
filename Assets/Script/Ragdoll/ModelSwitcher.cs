using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSwitcher : MonoBehaviour
{
    public GameObject inputPanel; // InputFieldとSubmitボタンを含むPanel
    public Button toggleUIButton;
    public Button submitButton;
    public InputField inputField;
    public Slider scaleSlider;
    // モデルを配置する場所
    public Transform modelHolder; 
    public float targetSize = 1.0f;
    private GameObject currentModel;
    //ローディング用のUI
    public GameObject loadingImagePanel;
    void Start()
    {
        inputPanel.SetActive(false);

        toggleUIButton.onClick.AddListener(() =>
        {
            inputPanel.SetActive(!inputPanel.activeSelf);
        });

        submitButton.onClick.AddListener(OnSubmit);

        /*
        // NFCで読み込まれたモデルがすでにロード済みならスキップ
        if (modelHolder.childCount == 0)
        {
            // デフォルトモデル（MyModel）をロードして表示
            GameObject prefab = Resources.Load<GameObject>("Models/MyModel");
            if (prefab != null)
            {
                currentModel = Instantiate(prefab, modelHolder);
                currentModel.transform.localPosition = Vector3.zero;
                currentModel.transform.localRotation = Quaternion.identity;
                currentModel.transform.localScale = Vector3.one;

                // モデルコントローラーに通知
                FindObjectOfType<ModelController>()?.UpdateTarget();
            }
            else
            {
                Debug.LogWarning("初期モデル 'MyModel' が Resources に見つかりません。");
            }
        }
        */
    }

    void OnSubmit()
    {
        string modelName = inputField.text;

        if (string.IsNullOrWhiteSpace(modelName)) return;

        StartCoroutine(LoadModelCoroutine(modelName));
    }
    IEnumerator LoadModelCoroutine(string modelName)
    {

        // モデル削除
        foreach (Transform child in modelHolder)
        {
            Destroy(child.gameObject);
        }

        // ローディング画像を表示
        loadingImagePanel.SetActive(true);

        // 1フレーム待機（UI表示のため）
        yield return new WaitForSeconds(3f);
        // モデルをResourcesからロード
        GameObject prefab = Resources.Load<GameObject>("Models/" + modelName);
        if (prefab != null)
        {
            foreach (Transform child in modelHolder)
            {
                Destroy(child.gameObject);
            }

            GameObject wrapper = new GameObject("ModelWrapper");
            wrapper.transform.SetParent(modelHolder, false);
            wrapper.transform.localPosition = Vector3.zero;
            wrapper.transform.localRotation = Quaternion.Euler(0, 180f, 0); // ★ 全モデル共通の回転
            //wrapper.transform.localScale = Vector3.one;

            // モデルを生成して wrapper の子に
            currentModel = Instantiate(prefab, wrapper.transform);

            // モデルの中心を補正
            Bounds bounds = CalculateBounds(currentModel);
            currentModel.transform.localPosition = -bounds.center;


            float scaleFactor = AutoScaleModel(wrapper);

            // スライダーの初期設定
            if (scaleSlider != null)
            {
                scaleSlider.onValueChanged.RemoveAllListeners();
                scaleSlider.minValue = scaleFactor * 0.5f;
                scaleSlider.maxValue = scaleFactor * 2f;
                scaleSlider.value = scaleFactor;

                scaleSlider.onValueChanged.AddListener((value) =>
                {
                    if (wrapper != null)
                    {
                        wrapper.transform.localScale = Vector3.one * value;
                    }
                });
            }
            // もう1フレーム待機（安全のため）
            yield return null; 
            // モデルコントローラーに新しいターゲットを通知
            StartCoroutine(NotifyModelControllerNextFrame());
        }
        else
        {
            Debug.LogWarning("モデルが見つかりません: " + modelName);
            loadingImagePanel.SetActive(false); // 忘れず閉じる
            yield break; 
        }


        loadingImagePanel.SetActive(false); // ← ロード完了


    }

    float AutoScaleModel(GameObject wrapper)
        {
        Renderer[] renderers = wrapper.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 1f;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxSize == 0) return 1f;

        float scaleFactor = targetSize / maxSize;
        wrapper.transform.localScale = Vector3.one * scaleFactor;

        Debug.Log($"モデルを自動スケーリングしました: 最大サイズ={maxSize}, スケール倍率={scaleFactor}");
        return scaleFactor;
         }


    private System.Collections.IEnumerator NotifyModelControllerNextFrame()
    {
        yield return null; // 1フレーム待機
        Debug.Log("ModelControllerにUpdateTargetを通知");
        FindObjectOfType<ModelController>()?.UpdateTarget();
    }

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

        // ワールド空間の中心をローカル空間に変換
        Vector3 localCenter = model.transform.InverseTransformPoint(bounds.center);
        bounds.center = localCenter;
        return bounds;
    }


}