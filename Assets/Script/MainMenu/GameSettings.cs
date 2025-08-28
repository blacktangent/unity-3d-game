using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���݂̃Q�[�����[�h�i��F1�l�e�X�g�A1vsCPU�A4vs4�j��v���C���[/CPU�̐ݒ��ێ����A
//�Q�[���S�̂ŋ��L�ł���悤�ɂ��邽�߂̃R�[�h�ł��BUnity�v���W�F�N�g�S�̂Ŏg����
//�u�ݒ�Ǘ��V���O���g���v

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