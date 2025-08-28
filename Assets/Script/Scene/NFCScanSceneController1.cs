using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NFCScanSceneController : MonoBehaviour
{
    public void OnProfileButtonPressed()
    {
        SceneManager.LoadScene("NFCScanScene");
    }
}
