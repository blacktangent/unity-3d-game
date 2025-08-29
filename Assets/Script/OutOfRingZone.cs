using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����O�O�ɏo�����̎����_���[�W
public class OutOfRingZone : MonoBehaviour
{
    public float damagePerSecond = 5f;
    public FactionType ownerFaction; // �U���҃`�[��

    private void OnTriggerStay(Collider other)
    {
        CharacterAttributes attr = other.GetComponent<CharacterAttributes>();
        if (attr != null && !attr.isDead)
        {
            float damageAmount = damagePerSecond * Time.deltaTime;
            attr.ApplyDamage(damageAmount, ElementType.None, transform.position, ownerFaction);

            Debug.Log($"[�����O�O] {other.name} �ɖ��b{damageAmount}�_���[�W");

            if (attr.isDead)
            {
                KillCounterManager.Instance.AddKill((int)ownerFaction);
                Debug.Log($"[�����O�O] {ownerFaction} �ɃL�����_");
            }
        }
    }
}