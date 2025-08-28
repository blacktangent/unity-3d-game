using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CPU_AI_Controller : MonoBehaviour
{
    // �������@�G�t�F�N�g�̃v���n�u
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private GameObject waterEffect;
    [SerializeField] private GameObject electricEffect;
    [SerializeField] private GameObject lightEffect;
    [SerializeField] private GameObject espEffect;
    [SerializeField] private GameObject starEffect;
    [SerializeField] private float destroyAfter = 2f; // ���@�G�t�F�N�g�̍폜����

    // ���G�ƃp�g���[���֘A�̐ݒ�
    public float detectionRange = 10f;     // ���G�͈�
    public float attackRange = 1.5f;        // �U���\����
    public float attackCooldown = 1.5f;     // �ʏ�U���̃N�[���_�E��
    public float magicCooldown = 5f;        // ���@�U���̃N�[���_�E��
    public float patrolWaitTime = 2f;       // �����_���ړ��̑ҋ@����
    public float patrolRadius = 10f;        // �����_���p�g���[�����a

    private float lastAttackTime = -999f;
    private float lastMagicTime = -999f;
    private Transform target;               // ���݂̓G�^�[�Q�b�g
    private NavMeshAgent agent;             // NavMeshAgent �ňړ�����

    private CharacterAttributes selfAttributes;
    private CharacterActions characterActions;
    public Collider weaponHitbox;           // �ߐڕ���̓����蔻��
    public float attackDuration = 0.3f;     // �q�b�g�{�b�N�X�L������

    private enum AIState { Patrol, Chasing, Attacking, MagicAttack, Fleeing }
    private AIState currentState = AIState.Patrol;

    private int groundLayer;
    private bool hasLanded = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        selfAttributes = GetComponent<CharacterAttributes>();
        characterActions = GetComponent<CharacterActions>();
        groundLayer = LayerMask.NameToLayer("Ground");
        if (agent != null) agent.enabled = false;
    }

    void Start()
    {
        if (weaponHitbox == null)
        {
            WeaponHitbox hitbox = GetComponentInChildren<WeaponHitbox>();
            if (hitbox != null)
            {
                weaponHitbox = hitbox.GetComponent<Collider>();
                Debug.Log("[AI] weaponHitbox �����擾: " + weaponHitbox?.name);
            }
        }
        if (weaponHitbox != null) weaponHitbox.enabled = false;

        if (selfAttributes != null)
            Debug.Log($"[CPU AI Init] Name: {gameObject.name}, Faction: {selfAttributes.faction}");
    }

    void Update()
    {
        if (!hasLanded && IsGrounded())
        {
            hasLanded = true;
            if (agent != null && !agent.enabled)
            {
                agent.enabled = true;
                Debug.Log($"[NavMesh] {gameObject.name} NavMeshAgent ��n�ʐڐG�ŗL����");
            }
        }

        if (!hasLanded || selfAttributes == null || selfAttributes.isDead || !agent.enabled) return;

        FindNearestEnemy();

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (selfAttributes.currentHP <= selfAttributes.maxHP * 0.25f)
                currentState = AIState.Fleeing;
            else if (selfAttributes.currentHP <= selfAttributes.maxHP * 0.5f && Time.time >= lastMagicTime + magicCooldown)
                currentState = AIState.MagicAttack;
            else if (dist <= attackRange)
                currentState = AIState.Attacking;
            else if (dist <= detectionRange)
                currentState = AIState.Chasing;
        }
        else
        {
            currentState = AIState.Patrol;
        }

        switch (currentState)
        {
            case AIState.Patrol:
                agent.speed = 7f;
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    StartCoroutine(WaitAndPatrolTowardLastKnownEnemy());
                break;

            case AIState.Chasing:
                agent.speed = 10f;
                agent.SetDestination(target.position);
                characterActions.PlayWalk(1f);
                break;

            case AIState.Attacking:
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    characterActions.PlayAttack();
                    StartCoroutine(EnableHitboxTemporary());
                    lastAttackTime = Time.time;
                }
                agent.ResetPath();
                break;

            case AIState.MagicAttack:
                characterActions.PlayMagicAttack();
                LaunchMagicEffect();
                lastMagicTime = Time.time;
                agent.ResetPath();
                break;

            case AIState.Fleeing:
                Vector3 fleeDir = (transform.position - target.position).normalized;
                agent.SetDestination(transform.position + fleeDir * 5f);
                characterActions.PlayWalk(1f);
                break;
        }
    }

    public void OnRespawn()
    {
        hasLanded = false;
        if (agent != null)
        {
            agent.enabled = false;
            agent.ResetPath();
        }
    }

    IEnumerator WaitAndPatrolTowardLastKnownEnemy()
    {
        yield return new WaitForSeconds(patrolWaitTime);

        Vector3 basePos = transform.position;
        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            basePos += dirToTarget * patrolRadius * 0.5f;
        }

        Vector3 randomOffset = Random.insideUnitSphere * patrolRadius * 0.5f;
        Vector3 targetPos = basePos + randomOffset;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FindNearestEnemy()
    {
        float minDist = detectionRange;
        target = null;
        foreach (CharacterAttributes other in FindObjectsOfType<CharacterAttributes>())
        {
            if (other == selfAttributes || other.isDead) continue;
            if (other.faction == selfAttributes.faction) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = other.transform;
            }
        }
    }

    void LaunchMagicEffect()
    {
        GameObject selectedEffect = null;
        switch (selfAttributes.elementType)
        {
            case ElementType.Fire: selectedEffect = fireEffect; break;
            case ElementType.Water: selectedEffect = waterEffect; break;
            case ElementType.Electric: selectedEffect = electricEffect; break;
            case ElementType.Light: selectedEffect = lightEffect; break;
            case ElementType.Esp: selectedEffect = espEffect; break;
            case ElementType.Star: selectedEffect = starEffect; break;
            default:
                Debug.LogWarning("[CPU���@] �������ݒ肳��Ă��܂���B");
                return;
        }

        Vector3 spawnPos = transform.position + transform.forward * 1.5f;
        Quaternion spawnRot = Quaternion.LookRotation(transform.forward);
        GameObject effect = Instantiate(selectedEffect, spawnPos, spawnRot);

        Rigidbody rb = effect.GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = transform.forward * 10f;

        ExplosionHitbox hitbox = effect.GetComponent<ExplosionHitbox>();
        if (hitbox != null)
        {
            hitbox.attackElement = selfAttributes.elementType;
            hitbox.ownerFaction = selfAttributes.faction;
        }

        Destroy(effect, destroyAfter);
    }

    IEnumerator EnableHitboxTemporary()
    {
        if (weaponHitbox == null) yield break;
        weaponHitbox.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        weaponHitbox.enabled = false;
    }

    private bool IsGrounded()
    {
        if (groundLayer == -1)
        {
            Debug.LogWarning("Ground ���C���[�����݂��܂���I");
            return false;
        }
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        return Physics.Raycast(ray, 0.3f, 1 << groundLayer);
    }
}
