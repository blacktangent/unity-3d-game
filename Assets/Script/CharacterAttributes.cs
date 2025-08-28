using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum FactionType
{
    Player,
    Enemy
}

//HPに関するダメージ計算や復活演出
//
public class CharacterAttributes : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public bool isDead = false;
    public ElementType elementType = ElementType.None;
    public bool isPlayerControlled = false; // 自キャラかどうか
    public FactionType faction = FactionType.Player; // 所属（プレイヤー or CPU）
    public Transform respawnPoint;
    public float invincibleDuration = 2f;
    private bool isInvincible = false;

    //インスペクターで設定する
    public TMP_Text statusText; // 状態表示用テキスト
    public TMP_Text hpText;     // HP表示用テキスト 

    private CharacterActions characterActions;
    private CharacterMovement movement;

    //死亡判定の高さ
    public float fallDeathY = -50f;

    private void Awake()
    {
        characterActions = GetComponent<CharacterActions>();
        movement = GetComponent<CharacterMovement>();
        isDead = false;
        currentHP = maxHP;
        UpdateStatusUI("");
    }
    private void Start()
    {
        currentHP = maxHP;
        UpdateHPText();
    }

    private void Update()
    {
        CheckFallDeath();
        /*
        // デバッグ用：Hキーで自分にダメージを与える（自キャラのみ）
        if (isPlayerControlled && Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("[デバッグ] 自分にダメージテスト");
            ApplyDamage(5f, ElementType.Fire);
        }
        */
    }

    public void ApplyDamage(float amount, ElementType attackerElement, Vector3 attackerPosition, FactionType attackerFaction)
    {
        // 自キャラ以外はダメージ処理を行わない
        //if (!isPlayerControlled) return;
        if (isDead || isInvincible) return;

        // 属性相性を計算（別クラスから計算）
        float multiplier = ElementUtils.GetEffectivenessMultiplier(attackerElement, elementType);
        float finalDamage = amount * multiplier;

        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0f);
        UpdateHPText();

        Debug.Log($"<color=red>{gameObject.name} が {attackerElement} 属性の攻撃を受けた！</color>");
        Debug.Log($"<color=yellow>ダメージ: {finalDamage:F1} / 残HP: {currentHP:F1}</color>");

        UpdateStatusUI($"Damage: {finalDamage:F1}");

        if (currentHP <= 0f && !isDead)
        {
            isDead = true;
            Debug.Log($"<color=gray>{gameObject.name} は倒れました！</color>");
            UpdateStatusUI("knocked down");


            // 💥ここでキルカウントを追加
            if (KillCounterManager.Instance != null)
            {
                KillCounterManager.Instance.AddKill((attackerFaction == FactionType.Player) ? 0 : 1);
            }


            // 死亡アニメ再生
            if (characterActions != null)
            {
                characterActions.PlayDeath();
            }

            // 死亡時の操作不可切り替え（CharacterMovementの生死フラグを変更）
            if (movement != null)
            {
                movement.SetAlive(false);
            }

            StartCoroutine(RespawnAfterDelay(3f));
        }
        else
        {
            // ダメージアニメ再生
            if (characterActions != null)
            {
                characterActions.PlayDamaged();
            }

            // ノックバック処理
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 knockbackDir = (transform.position - attackerPosition).normalized;
                rb.AddForce(knockbackDir * 5f, ForceMode.Impulse); // 数値調整OK
            }
        }
    }

    //復活、演出
    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // HP回復・位置復元
        currentHP = maxHP;
        transform.position = respawnPoint != null ? respawnPoint.position : Vector3.zero;
        Debug.Log($"<color=green>{gameObject.name} が復活しました！</color>");

        // UIのHP表示も更新する
        UpdateHPText();

        UpdateStatusUI("Respawned! Invincible");

        isDead = false;
        // ★移動操作復活
        // リスポーン時の操作復活
        if (movement != null)
        {
            movement.SetAlive(true);
        }

        // 復活アニメーション→アニメーションのリセット
        if (characterActions != null)
        {
            characterActions.PlayRespawn();
        }

        // 無敵状態へ（UIも）
        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
        UpdateStatusUI(""); // 無敵終了後にUI消す
    }

    private void UpdateStatusUI(string message)
    {
        if (isPlayerControlled && statusText != null)
        {
            statusText.text = message;
        }
    }

   //HPのUIの更新
    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {currentHP:F0}";
            Debug.Log($"[HP更新] {gameObject.name} の currentHP = {currentHP}, 表示 = {hpText.text}");
        }
        else
        {
            Debug.LogWarning($"[HP更新失敗] {gameObject.name} の hpText が未設定です！");
        }
    }

    private void CheckFallDeath()
    {
        if (!isDead && transform.position.y < fallDeathY)
        {
            Debug.LogWarning($"{gameObject.name} が落下死しました！");
            DieByFalling();
        }
    }

    private void DieByFalling()
    {
        currentHP = 0;
        isDead = true;

        UpdateHPText();
        UpdateStatusUI("落下死！");

        // アニメ・モーション
        if (characterActions != null)
            characterActions.PlayDeath();

        // 死亡時の操作不可切り替え（CharacterMovementの生死フラグを変更）
        if (movement != null)
        {
            movement.SetAlive(false);
        }

        StartCoroutine(RespawnAfterDelay(5f)); // 復活待ち
    }

    //死亡した時
    public void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} は死亡しました");

        // 移動停止処理など、別のコンポーネントに通知してもいい
        CharacterMovement movement = GetComponent<CharacterMovement>();
        if (movement != null)
            movement.SetAlive(false);
    }

    public void Respawn(Vector3 position)
    {
        isDead = false;
        currentHP = maxHP;

        transform.position = position;

        CharacterMovement movement = GetComponent<CharacterMovement>();
        if (movement != null)
            movement.SetAlive(true);
    }


}