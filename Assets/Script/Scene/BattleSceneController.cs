using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void GoToNextScene()
    {
        SceneManager.LoadScene("BattleScene"); // �� �V�[�����͓K�X�ύX
    }
}