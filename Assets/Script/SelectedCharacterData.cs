using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterSelection
{
    public string characterName;
    public string prefabKey;
    public ElementType elementType;
    public bool isPlayerControlled;
    public int teamId;
}

public class SelectedCharacterData : MonoBehaviour
{
    public static SelectedCharacterData Instance;

    //Player�p
    public string characterName;
    public string prefabKey;
    public ElementType elementType;

    //CPU�p
    public string cpuCharacterName;
    public string cpuPrefabKey;
    public ElementType cpuElementType;

    public List<CharacterSelection> selectedCharacters = new List<CharacterSelection>();

    private void Awake()
    {
        Debug.Log("[SelectedCharacterData] Awake �Ă΂ꂽ");
        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[SelectedCharacterData] Singleton�C���X�^���X��������");
    }

    //1P�E1vsCPU�p�֐�
    public void SetSinglePlayerCharacter(int index, string name, string prefabKey, ElementType type)
    {
        this.characterName = name;
        this.prefabKey = prefabKey;
        this.elementType = type;
    }

    public void SetCpuCharacter(string name, string key, ElementType type)
    {
        cpuCharacterName = name;
        cpuPrefabKey = key;
        cpuElementType = type;
    }

    //4vs4�E���l���p�֐�
    public void SetTeamCharacter(int index, string name, string prefabKey, ElementType type)
    {
        Debug.Log($"[SetTeamCharacter] index={index}, name={name}, key={prefabKey}, type={type}");

        CharacterSelection selection = new CharacterSelection
        {
            characterName = name,
            prefabKey = prefabKey,
            elementType = type
        };

        while (selectedCharacters.Count <= index)
        {
            selectedCharacters.Add(new CharacterSelection());
        }

        selectedCharacters[index] = selection;
    }


    public CharacterSelection GetCharacter(int index)
    {
        if (index < selectedCharacters.Count)
            return selectedCharacters[index];
        return null;
    }

    public bool IsCharacterAlreadySelected(string key)
    {
        return selectedCharacters.Exists(c => c.prefabKey == key);
    }
}
