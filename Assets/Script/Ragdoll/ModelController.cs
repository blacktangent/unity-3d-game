using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModelController : MonoBehaviour
{
    public string targetObjectName = "MyModel"; // �q�G�����L�[���3D�I�u�W�F�N�g��
    private Transform target;

    public Slider rotationSliderX;
    public Slider rotationSliderY;
    public Slider scaleSlider;



    IEnumerator Start() // �� Start �� Coroutine ��
    {
        yield return null; // 1�t���[���҂��Ă�����s
        UpdateTarget();

    }

    // �����ւ����ɌĂяo���āA���݂̃��f�����擾����
    public void UpdateTarget()
    {
        GameObject holder = GameObject.Find("ModelHolder");
        if (holder != null && holder.transform.childCount > 0)
        {
            target = holder.transform.GetChild(0);

            Debug.Log("�^�[�Q�b�g���X�V���܂���: " + target.name);

            // �X���C�_�[���ēo�^�i�����ɏW�񂵂Ă����j
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
            Debug.LogWarning("���f���^�[�Q�b�g��������܂���");
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