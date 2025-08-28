using TMPro;
using UnityEngine;

public class KillCounterManager : MonoBehaviour
{
    public static KillCounterManager Instance;

    public int team0Kills = 0;  // ���R�̃L����
    public int team1Kills = 0;  // �G�R�̃L����

    // UI�e�L�X�g�Q�ƁiInspector�ŃZ�b�g�j
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
            Debug.Log($"[�L���J�E���g] ���R�`�[���i0�j���L���F{team0Kills}");
        }
        else if (teamId == 1)
        {
            team1Kills++;
            UpdateKillUI();
            Debug.Log($"[�L���J�E���g] �G�`�[���i1�j���L���F{team1Kills}");
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