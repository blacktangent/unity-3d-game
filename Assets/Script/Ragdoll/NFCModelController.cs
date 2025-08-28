using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFCModelController : MonoBehaviour
{
    public GameObject bearModel;
    public GameObject catModel;

    void Start()
    {
        string tagId = NFCReaderMock.scannedTagId;

        if (tagId == "nfc_tag_001")
        {
            bearModel.SetActive(true);
            catModel.SetActive(false);
        }
        else if (tagId == "nfc_tag_002")
        {
            bearModel.SetActive(false);
            catModel.SetActive(true);
        }
        else
        {
            bearModel.SetActive(false);
            catModel.SetActive(false);
            Debug.LogWarning("Unknown NFC tag: " + tagId);
        }
    }
}
