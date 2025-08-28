using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Button proceedButton; // �V�[���J�ڃ{�^��

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        // ������ԂŃ{�^���𖳌��ɂ���
        proceedButton.interactable = false;
    }

    void OnDropdownChanged(int index)
    {
        if (GameSettings.Instance == null) return;

        switch (index)
        {
            case 1: // SingleTest
                GameSettings.Instance.mode = GameMode.SingleTest;
                GameSettings.GamePlayerCPUSettings.playerCount = 1;
                GameSettings.GamePlayerCPUSettings.cpuCount = 0;
                break;
            case 2: // OneVsCpu
                GameSettings.Instance.mode = GameMode.OneVsCpu;
                GameSettings.GamePlayerCPUSettings.playerCount = 1;
                GameSettings.GamePlayerCPUSettings.cpuCount = 1;
                break;
            case 3: // FourVsFour
                GameSettings.Instance.mode = GameMode.FourVsFour;
                GameSettings.GamePlayerCPUSettings.playerCount = 4;
                GameSettings.GamePlayerCPUSettings.cpuCount = 4;
                break;
            default:
                GameSettings.Instance.mode = GameMode.None;
                GameSettings.GamePlayerCPUSettings.playerCount = 0;
                GameSettings.GamePlayerCPUSettings.cpuCount = 0;
                break;
        }

        // index��1�ȏ�̂Ƃ������{�^���L����
        proceedButton.interactable = (index > 0);
    }
}
