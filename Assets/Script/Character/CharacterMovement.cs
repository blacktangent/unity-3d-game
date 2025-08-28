using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Transform cam;
    private bool isAlive = true;
    private float rotationSpeed = 10f;

    private CharacterActions characterActions;

    public Collider weaponHitbox;
    public float attackDuration = 0.3f;
    public float destroyAfter = 2f;
    private bool hasLanded = false;
    private bool isBlocking = false;

    public GameObject fireEffect;
    public GameObject waterEffect;
    public GameObject electricEffect;
    public GameObject lightEffect;
    public GameObject espEffect;
    public GameObject starEffect;

    public FixedJoystick joystick;  // UIコントローラー（スマホ用）
    private bool isMobile;
    private void Awake()
    {
        characterActions = GetComponent<CharacterActions>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        // 本番用:
        // isMobile = Application.platform == RuntimePlatform.Android ||
        //            Application.platform == RuntimePlatform.IPhonePlayer;

        // テスト用: 常に有効にする
        isMobile = true;

        if (joystick == null)
        {
            joystick = FindObjectOfType<FixedJoystick>();
            if (joystick != null)
            {
                Debug.Log("[自動取得] FixedJoystick を取得しました: " + joystick.name);
            }
            else
            {
                Debug.LogWarning("[警告] FixedJoystick がシーンに見つかりませんでした。");
            }
        }

        // 攻撃ボタンと魔法ボタンを自動取得してOnClick登録
        Button attackButton = GameObject.Find("AttackButton")?.GetComponent<Button>();
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButtonPressed);
            Debug.Log("[自動取得] 攻撃ボタンを登録しました");
        }
        else
        {
            Debug.LogWarning("[警告] AttackButton が見つかりませんでした");
        }

        Button magicButton = GameObject.Find("MagicButton")?.GetComponent<Button>();
        if (magicButton != null)
        {
            magicButton.onClick.AddListener(OnMagicButtonPressed);
            Debug.Log("[自動取得] 魔法ボタンを登録しました");
        }
        else
        {
            Debug.LogWarning("[警告] MagicButton が見つかりませんでした");
        }


        // 回転のみ凍結
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (weaponHitbox == null)
        {
            WeaponHitbox hitbox = GetComponentInChildren<WeaponHitbox>();
            if (hitbox != null)
            {
                weaponHitbox = hitbox.GetComponent<Collider>();
                Debug.Log($"[自動取得] weaponHitbox: {weaponHitbox?.name}");
            }
        }

        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = false;
        }

        // FixedJoystick を自動取得
        if (joystick == null)
        {
            joystick = FindObjectOfType<FixedJoystick>();
            if (joystick != null)
            {
                Debug.Log("[自動取得] FixedJoystick を取得しました: " + joystick.name);
            }
            else
            {
                Debug.LogWarning("[警告] FixedJoystick がシーンに見つかりませんでした。");
            }
        }
    }

    void Update()
    {
        animatorMove();
        CameraControl();
    }

    private Vector3 GetInputDirection()
    {
        if (isMobile && joystick != null)
        {
            return new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
        }
        else
        {
            return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        }
    }

    private void animatorMove()
    {
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        characterActions.PlayWalk(speed);

        if (Input.GetKeyDown(KeyCode.Space) && isAlive)
        {
            characterActions.PlayJump();
        }

        if (Input.GetKeyDown(KeyCode.Z) && isAlive)
        {
            characterActions.PlayAttack();
            StartCoroutine(EnableHitboxTemporary());
        }

        if (Input.GetKeyDown(KeyCode.X) && isAlive)
        {
            characterActions.PlayMagicAttack();
            StartCoroutine(EnableHitboxTemporary());
            TriggerMagicExplosion();
        }

        if (Input.GetKey(KeyCode.C) && isAlive)
        {
            if (!isBlocking)
            {
                isBlocking = true;
                characterActions.PlayDefend(isBlocking);
            }
        }
        else
        {
            if (isBlocking)
            {
                isBlocking = false;
                characterActions.PlayDefend(isBlocking);
            }
        }
    }

    IEnumerator EnableHitboxTemporary()
    {
        weaponHitbox.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        weaponHitbox.enabled = false;
    }

    private void CameraControl()
    {
        if (!isAlive)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        Vector3 inputDir = GetInputDirection();

        if (inputDir.magnitude < 0.1f)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // 停止
            return;
        }

        inputDir.Normalize();

        // カメラの方向を基準に移動方向を計算
        Vector3 camForward = cam.forward; camForward.y = 0; camForward.Normalize();
        Vector3 camRight = cam.right; camRight.y = 0; camRight.Normalize();
        Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

        Vector3 move = moveDir * moveSpeed;
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // 回転処理：後退中は回転しない（向きを維持）
        if (inputDir.z > 0.1f)
        {
            // 前進：通常通り回転
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else if (inputDir.z < -0.1f)
        {
            // 後退：向きはそのまま、後ろに動くだけ
            // アニメーションの変更も別途検討（例：PlayWalkBack）
            // ここで向きを変えないことで、後退アニメーションと一致
        }
    }

    private void TriggerMagicExplosion()
    {
        var character = SelectedCharacterData.Instance.elementType;
        if (character == null)
        {
            Debug.LogWarning("キャラクター属性情報が取得できませんでした。");
            return;
        }

        ElementType element = SelectedCharacterData.Instance.elementType;
        GameObject selectedEffect = null;
        switch (element)
        {
            case ElementType.Fire: selectedEffect = fireEffect; break;
            case ElementType.Water: selectedEffect = waterEffect; break;
            case ElementType.Electric: selectedEffect = electricEffect; break;
            case ElementType.Light: selectedEffect = lightEffect; break;
            case ElementType.Esp: selectedEffect = espEffect; break;
            case ElementType.Star: selectedEffect = starEffect; break;
            default:
                Debug.Log("属性が設定されていません（None）");
                return;
        }

        Vector3 spawnPos = transform.position + transform.forward * 1.5f;
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        GameObject magic = Instantiate(selectedEffect, spawnPos, rotation);

        Rigidbody rb = magic.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * 10f;
        }

        ExplosionHitbox hitbox = magic.GetComponent<ExplosionHitbox>();
        if (hitbox != null)
        {
            hitbox.attackElement = element;
            hitbox.ownerFaction = FactionType.Player;
        }

        Destroy(magic, destroyAfter);
    }

    public void SetAlive(bool value)
    {
        isAlive = value;
        if (isAlive)
        {
            rb.velocity = Vector3.zero;
        }
    }

    // スマホUIボタン用の攻撃処理
    public void OnAttackButtonPressed()
    {
        if (isAlive)
        {
            characterActions.PlayAttack();
            StartCoroutine(EnableHitboxTemporary());
        }
    }

    public void OnMagicButtonPressed()
    {
        if (isAlive)
        {
            characterActions.PlayMagicAttack();
            StartCoroutine(EnableHitboxTemporary());
            TriggerMagicExplosion();
        }
    }

    public float CurrentSpeed => new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
}
