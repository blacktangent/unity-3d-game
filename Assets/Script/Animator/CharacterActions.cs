using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    private Animator animator;
    public Collider weaponHitbox; // ����̔���Collider�iTrigger�j

    private LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;
    private bool animatorActivated = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false; // �ŏ��̓A�j���[�^�[������

        // �uGround�v���C���[�������擾
        groundLayer = LayerMask.GetMask("Ground");

        if (groundLayer == 0)
        {
            Debug.LogWarning("[CharacterActions] Ground���C���[�����݂��܂���IProject�ݒ��Layers���� 'Ground' ��ǉ����Ă��������B");
        }
    }

    void Update()
    {
        if (!animatorActivated && IsGrounded())
        {
            animator.enabled = true;
            animatorActivated = true;
            PlayIdle(); // Idle�A�j���Đ�
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    // Idle �A�j���[�V�����Đ�
    public void PlayIdle()
    {
        animator.SetFloat("Speed", 0f);
    }

    //�ړ��A�j���[�V����
    public void PlayWalk(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
    //�U���A�j���[�V����
    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
    //�h��A�j���[�V����
    public void PlayDefend(bool isBlocking)
    {
        animator.SetBool("Defend", isBlocking);

    }
    //�����U���A�j���[�V����
    public void PlayMagicAttack()
    {
        animator.SetTrigger("MagicAttack");
    }
    //�W�����v�A�j���[�V����
    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }
    //�_���[�W�󂯂����A�j���[�V����
    public void PlayDamaged()
    {
        animator.SetTrigger("Damaged");
    }
    //���S�������A�j���[�V����
    public void PlayDeath()
    {
        animator.SetTrigger("Die");
    }
    //�������鎞�A�j���[�V����
    public void PlayRespawn()
    {
        animator.SetTrigger("Respawn");
    }

    public void EnableHitbox()
    {
        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = true;
            //Debug.Log("�q�b�g�{�b�N�X ON");
        }
    }

    public void DisableHitbox()
    {
        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = false;
            //Debug.Log("�q�b�g�{�b�N�X OFF");
        }
    }

}
