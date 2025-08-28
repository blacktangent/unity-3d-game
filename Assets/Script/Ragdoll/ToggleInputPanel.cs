using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInputPanel : MonoBehaviour
{
    public GameObject inputPanel;

    public void TogglePanel()
    {
        if (inputPanel != null)
        {
            inputPanel.SetActive(!inputPanel.activeSelf);
        }
    }
}
