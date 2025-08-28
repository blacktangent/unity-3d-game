using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    public void OnProfileButtonPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
