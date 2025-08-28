using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    private Animator animator;
    public Collider weaponHitbox; // 武器の判定Collider（Trigger）

    private LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;
    private bool animatorActivated = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false; // 最初はアニメーター無効化

        // 「Ground」レイヤーを自動取得
        groundLayer = LayerMask.GetMask("Ground");

        if (groundLayer == 0)
        {
            Debug.LogWarning("[CharacterActions] Groundレイヤーが存在しません！Project設定のLayersから 'Ground' を追加してください。");
        }
    }

    void Update()
    {
        if (!animatorActivated && IsGrounded())
        {
            animator.enabled = true;
            animatorActivated = true;
            PlayIdle(); // Idleアニメ再生
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    // Idle アニメーション再生
    public void PlayIdle()
    {
        animator.SetFloat("Speed", 0f);
    }

    //移動アニメーション
    public void PlayWalk(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
    //攻撃アニメーション
    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
    //防御アニメーション
    public void PlayDefend(bool isBlocking)
    {
        animator.SetBool("Defend", isBlocking);

    }
    //属性攻撃アニメーション
    public void PlayMagicAttack()
    {
        animator.SetTrigger("MagicAttack");
    }
    //ジャンプアニメーション
    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }
    //ダメージ受けた時アニメーション
    public void PlayDamaged()
    {
        animator.SetTrigger("Damaged");
    }
    //死亡した時アニメーション
    public void PlayDeath()
    {
        animator.SetTrigger("Die");
    }
    //復活する時アニメーション
    public void PlayRespawn()
    {
        animator.SetTrigger("Respawn");
    }

    public void EnableHitbox()
    {
        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = true;
            //Debug.Log("ヒットボックス ON");
        }
    }

    public void DisableHitbox()
    {
        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = false;
            //Debug.Log("ヒットボックス OFF");
        }
    }

}
