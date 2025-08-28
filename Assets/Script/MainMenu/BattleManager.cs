using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public float matchDuration = 120f; // 制限時間（秒）
    private float timer;

    public TMP_Text timerText;
    public TMP_Text resultText;

    private bool isBattleEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        timer = matchDuration;
        UpdateTimerUI();
    }

    void Update()
    {
        if (isBattleEnded) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            EndBattle();
        }
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void EndBattle()
    {
        isBattleEnded = true;

        int team0Kills = KillCounterManager.Instance.team0Kills;
        int team1Kills = KillCounterManager.Instance.team1Kills;

        string result;
        if (team0Kills > team1Kills) result = "Team0 Win";
        else if (team1Kills > team0Kills) result = "Team1 Win";
        else result = "draw";

        Debug.Log($"[試合終了] チーム0: {team0Kills}キル / チーム1: {team1Kills}キル → {result}");

        if (resultText != null)
        {
            resultText.text = result;
        }

        // TODO: リザルト画面表示や再戦ボタンの処理をここに追加
    }
}