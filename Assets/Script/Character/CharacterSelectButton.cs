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
            Debug.Log("Buttonを自動取得しました: " + button);
        }

        if (button == null)
        {
            Debug.LogError("Buttonが見つかりません！");
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
        // 表示変更のみ、保存処理は Manager 側が行う
        //nameText.text = characterName + " (選択済み)";
        Debug.Log($"[OnClick] 選択されたキャラクター: {characterName}");

        // 他のボタンを未選択に
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
                nameText.text = "LOCKED";  // または "キャラ名 (選択済み)" どちらかに絞る
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
                nameText.text = characterName + " (選択済み)";
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