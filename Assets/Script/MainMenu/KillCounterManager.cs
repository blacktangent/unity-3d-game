using TMPro;
using UnityEngine;

public class KillCounterManager : MonoBehaviour
{
    public static KillCounterManager Instance;

    public int team0Kills = 0;  // 自軍のキル数
    public int team1Kills = 0;  // 敵軍のキル数

    // UIテキスト参照（Inspectorでセット）
    public TMP_Text team0KillText;
    public TMP_Text team1KillText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill(int teamId)
    {
        if (teamId == 0)
        {
            team0Kills++;
            UpdateKillUI();
            Debug.Log($"[キルカウント] 自軍チーム（0）がキル：{team0Kills}");
        }
        else if (teamId == 1)
        {
            team1Kills++;
            UpdateKillUI();
            Debug.Log($"[キルカウント] 敵チーム（1）がキル：{team1Kills}");
        }
    }

    public void ResetKills()
    {
        team0Kills = 0;
        team1Kills = 0;
        UpdateKillUI();
    }

    private void UpdateKillUI()
    {
        if (team0KillText != null)
            team0KillText.text = $"Team 0: {team0Kills}";

        if (team1KillText != null)
            team1KillText.text = $"Team 1: {team1Kills}";
    }
}