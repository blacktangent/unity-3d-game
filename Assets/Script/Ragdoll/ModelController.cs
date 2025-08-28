using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModelController : MonoBehaviour
{
    public string targetObjectName = "MyModel"; // ヒエラルキー上の3Dオブジェクト名
    private Transform target;

    public Slider rotationSliderX;
    public Slider rotationSliderY;
    public Slider scaleSlider;



    IEnumerator Start() // ← Start を Coroutine に
    {
        yield return null; // 1フレーム待ってから実行
        UpdateTarget();

    }

    // 差し替え時に呼び出して、現在のモデルを取得する
    public void UpdateTarget()
    {
        GameObject holder = GameObject.Find("ModelHolder");
        if (holder != null && holder.transform.childCount > 0)
        {
            target = holder.transform.GetChild(0);

            Debug.Log("ターゲットを更新しました: " + target.name);

            // スライダーを再登録（ここに集約しておく）
            if (rotationSliderX != null)
            {
                rotationSliderX.onValueChanged.RemoveAllListeners();
                rotationSliderX.onValueChanged.AddListener(UpdateRotation);
            }

            if (rotationSliderY != null)
            {
                rotationSliderY.onValueChanged.RemoveAllListeners();
                rotationSliderY.onValueChanged.AddListener(UpdateRotation);
            }

            if (scaleSlider != null)
            {
                scaleSlider.onValueChanged.RemoveAllListeners();
                scaleSlider.onValueChanged.AddListener(UpdateScale);
            }
        }
        else
        {
            Debug.LogWarning("モデルターゲットが見つかりません");
            target = null;
        }
    }

    void UpdateRotation(float _)
    {
        if (target != null)
        {
            float x = rotationSliderX != null ? rotationSliderX.value : 0f;
            float y = rotationSliderY != null ? rotationSliderY.value : 0f;
            target.rotation = Quaternion.Euler(x, y, 0f);
        }
    }

    void UpdateScale(float _)
    {
        if (target != null && scaleSlider != null)
        {
            float scale = scaleSlider.value;
            target.localScale = Vector3.one * scale;
        }
    }
}