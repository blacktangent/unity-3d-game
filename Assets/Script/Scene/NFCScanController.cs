using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NFCScanController : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
