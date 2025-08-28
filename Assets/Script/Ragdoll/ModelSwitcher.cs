using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSwitcher : MonoBehaviour
{
    public GameObject inputPanel; // InputField��Submit�{�^�����܂�Panel
    public Button toggleUIButton;
    public Button submitButton;
    public InputField inputField;
    public Slider scaleSlider;
    // ���f����z�u����ꏊ
    public Transform modelHolder; 
    public float targetSize = 1.0f;
    private GameObject currentModel;
    //���[�f�B���O�p��UI
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
        // NFC�œǂݍ��܂ꂽ���f�������łɃ��[�h�ς݂Ȃ�X�L�b�v
        if (modelHolder.childCount == 0)
        {
            // �f�t�H���g���f���iMyModel�j�����[�h���ĕ\��
            GameObject prefab = Resources.Load<GameObject>("Models/MyModel");
            if (prefab != null)
            {
                currentModel = Instantiate(prefab, modelHolder);
                currentModel.transform.localPosition = Vector3.zero;
                currentModel.transform.localRotation = Quaternion.identity;
                currentModel.transform.localScale = Vector3.one;

                // ���f���R���g���[���[�ɒʒm
                FindObjectOfType<ModelController>()?.UpdateTarget();
            }
            else
            {
                Debug.LogWarning("�������f�� 'MyModel' �� Resources �Ɍ�����܂���B");
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

        // ���f���폜
        foreach (Transform child in modelHolder)
        {
            Destroy(child.gameObject);
        }

        // ���[�f�B���O�摜��\��
        loadingImagePanel.SetActive(true);

        // 1�t���[���ҋ@�iUI�\���̂��߁j
        yield return new WaitForSeconds(3f);
        // ���f����Resources���烍�[�h
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
            wrapper.transform.localRotation = Quaternion.Euler(0, 180f, 0); // �� �S���f�����ʂ̉�]
            //wrapper.transform.localScale = Vector3.one;

            // ���f���𐶐����� wrapper �̎q��
            currentModel = Instantiate(prefab, wrapper.transform);

            // ���f���̒��S��␳
            Bounds bounds = CalculateBounds(currentModel);
            currentModel.transform.localPosition = -bounds.center;


            float scaleFactor = AutoScaleModel(wrapper);

            // �X���C�_�[�̏����ݒ�
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
            // ����1�t���[���ҋ@�i���S�̂��߁j
            yield return null; 
            // ���f���R���g���[���[�ɐV�����^�[�Q�b�g��ʒm
            StartCoroutine(NotifyModelControllerNextFrame());
        }
        else
        {
            Debug.LogWarning("���f����������܂���: " + modelName);
            loadingImagePanel.SetActive(false); // �Y�ꂸ����
            yield break; 
        }


        loadingImagePanel.SetActive(false); // �� ���[�h����


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

        Debug.Log($"���f���������X�P�[�����O���܂���: �ő�T�C�Y={maxSize}, �X�P�[���{��={scaleFactor}");
        return scaleFactor;
         }


    private System.Collections.IEnumerator NotifyModelControllerNextFrame()
    {
        yield return null; // 1�t���[���ҋ@
        Debug.Log("ModelController��UpdateTarget��ʒm");
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

        // ���[���h��Ԃ̒��S�����[�J����Ԃɕϊ�
        Vector3 localCenter = model.transform.InverseTransformPoint(bounds.center);
        bounds.center = localCenter;
        return bounds;
    }


}