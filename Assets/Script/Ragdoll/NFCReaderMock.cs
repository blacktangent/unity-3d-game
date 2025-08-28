using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitsNFCToolkit;
using UnityEngine.SceneManagement;


public class NFCReaderMock : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private NativeNFC nfc;
#endif

    public Button scanButton;
    public Image resultImage;
    public TMP_Text statusText;
    public Sprite plushieSprite; // 読み取り成功後に表示するぬいぐるみ画像

    bool NFCconnection = false;
    public static string scannedTagId = null;

    void Start()
    {
        // UID保持オブジェクトがなければ作成
        if (NFCDataHolder.Instance == null)
        {
            new GameObject("NFCDataHolder").AddComponent<NFCDataHolder>();
        }
        resultImage.gameObject.SetActive(false);
        statusText.text = "Please scan NFC";
        scanButton.onClick.AddListener(OnScanButtonPressed);
    }

    void OnScanButtonPressed()
    {
        statusText.text = "Reading..";
        scanButton.interactable = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        NFCconnection = true;
        GameObject nfcObj = new GameObject("NFCManager");
        nfc = nfcObj.AddComponent<AndroidNFC>();
        nfc.NFCTagDetected += OnTagDetected;

        nfc.Initialize();
        nfc.Enable();
#endif

        if (NFCconnection == false)
        {
            Invoke(nameof(SimulateNFCSuccess), 2f);
        }
    }

    void SimulateNFCSuccess()
    {
        string mockUID = "04B3550A405980";
        Debug.Log("Simulated UID: " + mockUID);
        SetUIDAndLoadScene(mockUID);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
private void OnTagDetected(NFCTag tag)
{
    string realUID = tag.ID;
    SetUIDAndLoadScene(realUID);
}
#endif

    void SetUIDAndLoadScene(string uid)
    {
        NFCDataHolder.Instance.scannedUID = uid;

        statusText.text = "読み取り成功: " + uid;
        resultImage.sprite = plushieSprite;
        resultImage.gameObject.SetActive(true);

        SceneManager.LoadScene("RagdollViewer");
    }


}