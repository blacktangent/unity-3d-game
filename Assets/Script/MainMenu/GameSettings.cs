using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//現在のゲームモード（例：1人テスト、1vsCPU、4vs4）やプレイヤー/CPUの設定を保持し、
//ゲーム全体で共有できるようにするためのコードです。Unityプロジェクト全体で使い回す
//「設定管理シングルトン」

public enum GameMode
{
    None,
    SingleTest,
    OneVsCpu,
    FourVsFour
}

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public GameMode mode;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public static class GamePlayerCPUSettings
    {
        public static int playerCount = 1;
        public static int cpuCount = 1;
    }

}