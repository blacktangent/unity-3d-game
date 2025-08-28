using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform targetCamera;
    public Vector3 offset = new Vector3(0, 3, -6);
    public float smoothSpeed = 10f;
    public LayerMask collisionLayers;

    public float rotationSpeed = 5f;
    private float currentYaw = 0f;

    public string targetTag = "Player";
    public bool findTargetAtStart = true;

    public float pitch = 6f;//10f;               // 通常時の縦角度
    public float upwardPitch = 14f;         // Wキー時の縦角度
    public float pitchSmooth = 5f;          // 補間速度
    private float currentPitch = 6f;       // 現在のピッチ

    private bool justStarted = true; // 再生直後は即追従

    void Awake()
    {
        offset = new Vector3(0, 3, -6); // 強制的に初期化
    }

    void Start()
    {


        if (findTargetAtStart && targetCamera == null)
        {
            TryFindTarget();
        }

        if (targetCamera != null)
        {
            // 初期位置を瞬時に合わせる
            Vector3 rotatedOffset = Quaternion.Euler(0f, currentYaw, 0f) * offset;
            transform.position = targetCamera.position + rotatedOffset;

            Vector3 lookTarget = targetCamera.position + Vector3.up * pitch;
            transform.LookAt(lookTarget);
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null)
        {
            TryFindTarget();
            return;
        }

        // ピッチの制限
        currentPitch = Mathf.Clamp(currentPitch, -20f, 60f);

        // カメラの回転を作成
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // 回転をかけたオフセットを計算し、キャラ位置に加算
        Vector3 desiredPosition = targetCamera.position + rotation * offset;

        // 衝突判定
        if (Physics.Linecast(targetCamera.position, desiredPosition, out RaycastHit hit, collisionLayers))
        {
            desiredPosition = hit.point + hit.normal * 0.2f;
        }

        // 距離に応じてスムーズ追従
        float distance = Vector3.Distance(transform.position, desiredPosition);
        if (justStarted || distance > 3f)
        {
            transform.position = desiredPosition;
            justStarted = false;
        }
        else
        {
            float enhancedSmooth = Mathf.Lerp(20f, 60f, distance / 3f);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, enhancedSmooth * Time.deltaTime);
        }

        // カメラの向きを設定（キャラの方向を向くわけではなく、回転管理する）
        transform.rotation = rotation;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = -Input.GetAxis("Mouse Y"); // Y軸反転はお好みで

            currentYaw += horizontal * rotationSpeed;
            currentPitch += vertical * rotationSpeed;
        }
    }

    void TryFindTarget()
    {
        GameObject found = GameObject.FindWithTag(targetTag);
        if (found != null)
        {
            targetCamera = found.transform;
            Debug.Log("ターゲットを自動取得: " + found.name);

            // 出現時に即追従
            Vector3 rotatedOffset = Quaternion.Euler(0f, currentYaw, 0f) * offset;
            transform.position = targetCamera.position + rotatedOffset;
        }
    }
}