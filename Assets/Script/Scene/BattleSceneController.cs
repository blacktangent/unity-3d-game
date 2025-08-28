using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void GoToNextScene()
    {
        SceneManager.LoadScene("BattleScene"); // ← シーン名は適宜変更
    }
}