using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnSingleTestSelected()
    {
        GameSettings.Instance.mode = GameMode.SingleTest;
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public void OnOneVsCpuSelected()
    {
        GameSettings.Instance.mode = GameMode.OneVsCpu;
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public void OnProceedButtonPressed()
    {
        switch (GameSettings.Instance.mode)
        {
            case GameMode.SingleTest:
            case GameMode.OneVsCpu:
            case GameMode.FourVsFour:
                SceneManager.LoadScene("CharacterSelectScene");
                break;
            default:
                Debug.LogWarning("ゲームモードが未選択です！");
                break;
        }
    }
}
