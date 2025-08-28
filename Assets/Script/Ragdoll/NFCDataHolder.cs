using UnityEngine;

public class NFCDataHolder : MonoBehaviour
{
    public static NFCDataHolder Instance { get; private set; }

    public string scannedUID;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}