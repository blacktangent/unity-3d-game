using System.Collections;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public GameObject attackHitboxPrefab;
    public float attackDistance = 1.5f;
    public float attackDuration = 0.3f;
    private GameObject currentAttackObj = null;

    private ElementType myElement;

    void Start()
    {
        if (SelectedCharacterData.Instance != null)
            myElement = SelectedCharacterData.Instance.elementType;
        else
            myElement = ElementType.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // すでに攻撃判定があるなら生成しない
            if (currentAttackObj != null)
                return;

            Vector3 spawnPos = transform.position + transform.forward * attackDistance;
            Quaternion rot = Quaternion.LookRotation(transform.forward);
            GameObject attackObj = Instantiate(attackHitboxPrefab, spawnPos, rot);
            attackObj.SetActive(true);

            // 必ず currentAttackObj に代入！
            currentAttackObj = attackObj;

            AttackHitbox hitbox = attackObj.GetComponent<AttackHitbox>();
            if (hitbox != null)
                //hitbox.attackElement = myElement;

            // 明示的な破棄は不要（Destroy() でもOK）
            StartCoroutine(ClearCurrentAttackAfterDelay(currentAttackObj, attackDuration));
        }
    }

    private IEnumerator ClearCurrentAttackAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
            Destroy(obj);

        if (obj == currentAttackObj)
            currentAttackObj = null;
    }
}