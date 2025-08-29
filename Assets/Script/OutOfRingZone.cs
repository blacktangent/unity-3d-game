using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//リング外に出た時の持続ダメージ
public class OutOfRingZone : MonoBehaviour
{
    public float damagePerSecond = 5f;
    public FactionType ownerFaction; // 攻撃者チーム

    private void OnTriggerStay(Collider other)
    {
        CharacterAttributes attr = other.GetComponent<CharacterAttributes>();
        if (attr != null && !attr.isDead)
        {
            float damageAmount = damagePerSecond * Time.deltaTime;
            attr.ApplyDamage(damageAmount, ElementType.None, transform.position, ownerFaction);

            Debug.Log($"[リング外] {other.name} に毎秒{damageAmount}ダメージ");

            if (attr.isDead)
            {
                KillCounterManager.Instance.AddKill((int)ownerFaction);
                Debug.Log($"[リング外] {ownerFaction} にキル加点");
            }
        }
    }
}