using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum ElementType
{
    None,
    Fire,
    Water,
    Electric,
    Light,
    Esp,
    Star
}

[System.Serializable]
public class CharacterData
{
    public int index;
    public string name;
    public Sprite image;
    public string prefabKey;
    public ElementType elementType;
}

[System.Serializable]
public class CharacterPrefabEntry
{
    public string key;
    public GameObject prefab;
}

public class CharacterSelectManager : MonoBehaviour
{
    public List<CharacterData> characterList;
    public List<CharacterPrefabEntry> prefabEntries;
    public Transform gridParent;
    public Button nextButton;

    private List<CharacterSelectButton> allButtons = new();
    private Dictionary<string, GameObject> prefabDict;
    private bool characterSelected = false;
    private CharacterSelectButton currentSelectedButton = null;

    void Start()
    {
        Debug.Log("[Start] GameSettings.Instance = " + (GameSettings.Instance == null ? "NULL" : "OK"));
        Debug.Log("[Start] 選択中のモード: " + GameSettings.Instance.mode);

        prefabDict = new Dictionary<string, GameObject>();
        foreach (var entry in prefabEntries)
        {
            if (!prefabDict.ContainsKey(entry.key))
                prefabDict.Add(entry.key, entry.prefab);
        }



        // キャラクターセレクト ボタン生成コード
        foreach (var data in characterList)
        {
            Debug.Log($"[チェック中] キャラ名: {data.name}, prefabKey: '{data.prefabKey}'");

            if (string.IsNullOrEmpty(data.prefabKey))
            {
                Debug.LogError($"[エラー] '{data.name}' に prefabKey が設定されていません！");
                continue;
            }


            if (prefabDict.TryGetValue(data.prefabKey, out GameObject prefab))
            {
                GameObject btnObj = Instantiate(prefab, gridParent);
                CharacterSelectButton button = btnObj.GetComponent<CharacterSelectButton>();
                if (button != null)
                {
                    button.Setup(data.name, data.image, data.elementType, data.prefabKey);
                    allButtons.Add(button);
                    button.button.onClick.AddListener(() => OnCharacterButtonClicked(button));
                }
                else
                {
                    Debug.LogWarning("プレハブに CharacterSelectButton がアタッチされていません。");
                }
            }
            else
            {
                Debug.LogWarning($"'{data.prefabKey}' に対応するプレハブが見つかりません。");
            }
        }

        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // ★ ゲームモードによって自動セット
        if (GameSettings.Instance.mode == GameMode.FourVsFour)
        {
            nextButton.gameObject.SetActive(true); // すぐ進めるようにする
        }
    }

    public void OnCharacterButtonClicked(CharacterSelectButton selectedButton)
    {
        if (characterSelected && selectedButton == currentSelectedButton)
        {
            characterSelected = false;
            currentSelectedButton = null;
            selectedButton.SetSelected(false);
            foreach (var btn in allButtons)
                btn.button.interactable = true;

            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            SelectedCharacterData.Instance.SetSinglePlayerCharacter(0, "", "", ElementType.None);
            return;
        }

        if (characterSelected) return;

        characterSelected = true;
        currentSelectedButton = selectedButton;

        foreach (var btn in allButtons)
        {
            btn.SetSelected(btn == selectedButton);
            btn.button.interactable = (btn == selectedButton);
        }

        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        // ★ 名前ではなく prefabKey で特定する
        var data = characterList.Find(c => c.prefabKey == selectedButton.prefabKey);
        if (data != null)
        {
            SelectedCharacterData.Instance.SetSinglePlayerCharacter(0, data.name, data.prefabKey, data.elementType);
            Debug.Log($"[Manager] プレイヤーキャラ保存: {data.name}, {data.prefabKey}, {data.elementType}");

            //1PvsCPU
            if (GameSettings.Instance.mode == GameMode.OneVsCpu)
            {
                SetCpuCharacter();
            }
            if (GameSettings.Instance.mode == GameMode.FourVsFour)
            {
                SetupRandomCharactersFor4vs4(selectedButton.prefabKey);
            }

        }
        else
        {
            Debug.LogError("一致するキャラクターが CharacterList に見つかりませんでした！");
        }
    }

    //CPUをランダムで選択する
    //1vsCPUモード
    void SetCpuCharacter()
    {
        List<CharacterData> candidates = new List<CharacterData>(characterList);
        string playerKey = SelectedCharacterData.Instance.prefabKey;

        if (!string.IsNullOrEmpty(playerKey))
        {
            candidates.RemoveAll(c => c.prefabKey == playerKey);
        }

        if (candidates.Count == 0) return;

        CharacterData cpu = candidates[Random.Range(0, candidates.Count)];
        SelectedCharacterData.Instance.SetCpuCharacter(cpu.name, cpu.prefabKey, cpu.elementType);

        Debug.Log($"[CPU] 選ばれたキャラ: {cpu.name}, {cpu.prefabKey}");
    }


    private void SetupRandomCharactersFor4vs4(string playerKey)
    {
        Debug.Log("[4vs4] プレイヤー選択後、CPU7体をランダムで設定します");

        if (characterList == null || characterList.Count < 8)
        {
            Debug.LogError("[4vs4] 登録されているキャラが8体未満のため、セットアップできません！");
            return;
        }

        // プレイヤーキャラ登録（インデックス0）
        var playerData = characterList.Find(c => c.prefabKey == playerKey);
        if (playerData == null)
        {
            Debug.LogError("[4vs4] プレイヤーキャラが見つかりません");
            return;
        }

        SelectedCharacterData.Instance.SetTeamCharacter(0, playerData.name, playerData.prefabKey, playerData.elementType);
        SelectedCharacterData.Instance.selectedCharacters[0].isPlayerControlled = true;
        SelectedCharacterData.Instance.selectedCharacters[0].teamId = 0;
        Debug.Log($"[4vs4] 1P登録: {playerData.name} {playerData.prefabKey}");

        // プレイヤーを除いたキャラをランダムに7体選ぶ
        List<CharacterData> candidates = new List<CharacterData>(characterList);
        candidates.RemoveAll(c => c.prefabKey == playerKey);

        if (candidates.Count < 7)
        {
            Debug.LogError("[4vs4] CPU用のキャラが不足しています");
            return;
        }

        ShuffleList(candidates);

        // チームA（1Pチーム）の残り3体（index 1～3）
        for (int i = 0; i < 3; i++)
        {
            var cpuData = candidates[i];
            SelectedCharacterData.Instance.SetTeamCharacter(i + 1, cpuData.name, cpuData.prefabKey, cpuData.elementType);
            SelectedCharacterData.Instance.selectedCharacters[i + 1].isPlayerControlled = false;
            SelectedCharacterData.Instance.selectedCharacters[i + 1].teamId = 0;
            Debug.Log($"[4vs4] 味方CPU登録 {i + 1}: {cpuData.prefabKey}");
        }

        // チームB（敵CPUチーム）4体（index 4～7）
        for (int i = 3; i < 7; i++)
        {
            var cpuData = candidates[i];
            int idx = i + 1; // index 4～7
            SelectedCharacterData.Instance.SetTeamCharacter(idx, cpuData.name, cpuData.prefabKey, cpuData.elementType);
            SelectedCharacterData.Instance.selectedCharacters[idx].isPlayerControlled = false;
            SelectedCharacterData.Instance.selectedCharacters[idx].teamId = 1;
            Debug.Log($"[4vs4] 敵CPU登録 {idx}: {cpuData.prefabKey}");
        }

        Debug.Log("[4vs4] キャラ選定完了");
    }

    // Fisher-Yates シャッフル
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }


    void LogSelectedCharacterCounts()
    {
        int totalSelected = SelectedCharacterData.Instance.selectedCharacters.Count;

        int playerCount = 0;
        int cpuCount = 0;

        for (int i = 0; i < totalSelected; i++)
        {
            var c = SelectedCharacterData.Instance.GetCharacter(i);
            if (c == null) continue;

            if (i == 0) playerCount++;
            else cpuCount++;
        }

        Debug.Log($"[キャラセレクト] プレイヤー数: {playerCount} / CPU数: {cpuCount} / 合計: {totalSelected}");
    }
}