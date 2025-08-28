using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectButton : MonoBehaviour
{
    public string characterName;
    public ElementType elementType;
    public string prefabKey;
    public Image characterImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI lockText;
    public Button button;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
            Debug.Log("Button�������擾���܂���: " + button);
        }

        if (button == null)
        {
            Debug.LogError("Button��������܂���I");
        }
    }

    public void Setup(string name, Sprite image, ElementType type, string key)
    {
        characterName = name;
        elementType = type;
        prefabKey = key;

        if (nameText != null) nameText.text = name;
        if (characterImage != null) characterImage.sprite = image;
    }

    public void OnClick()
    {
        // �\���ύX�̂݁A�ۑ������� Manager �����s��
        //nameText.text = characterName + " (�I���ς�)";
        Debug.Log($"[OnClick] �I�����ꂽ�L�����N�^�[: {characterName}");

        // ���̃{�^���𖢑I����
        CharacterSelectButton[] allButtons = FindObjectsOfType<CharacterSelectButton>();
        foreach (var btn in allButtons)
        {
            btn.SetSelected(btn == this);
        }
    }

    public void SetSelected(bool isSelected)
    {
        
        if (isSelected)
        {
            if (button != null && button.image != null)
                button.image.color = Color.red;
            if (nameText != null)
                nameText.text = "LOCKED";  // �܂��� "�L������ (�I���ς�)" �ǂ��炩�ɍi��
        }
        else
        {
            if (button != null && button.image != null)
                button.image.color = Color.white;
            if (nameText != null)
                nameText.text = "";
        }
        Debug.Log($"SetSelected({isSelected}) called on {characterName}");
        /*
        if (isSelected)
        {
            if (nameText != null)
                nameText.text = characterName + " (�I���ς�)";
            if (button != null && button.image != null)
                button.image.color = Color.red;
                
        }
        else
        {
            if (nameText != null)
                nameText.text = characterName;
            if (button != null && button.image != null)
                button.image.color = Color.white;
        }
        */
    }
}