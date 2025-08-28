using UnityEngine;
using System.Collections.Generic;

public class ExplosionHitbox : MonoBehaviour
{
    public float damage = 15f;
    public ElementType attackElement = ElementType.Fire;
    public FactionType ownerFaction;

    public float cooldownTime = 1.0f; // ��������ɍĂѓ������܂ł̎��ԁi�b�j

    private Dictionary<Collider, float> hitCooldowns = new Dictionary<Collider, float>();

    void Update()
    {
        // ��莞�Ԃ��o�߂�����A�����蔻�胊�Z�b�g
        List<Collider> expired = new List<Collider>();
        foreach (var pair in hitCooldowns)
        {
            if (Time.time - pair.Value > cooldownTime)
            {
                expired.Add(pair.Key);
            }
        }
        foreach (var col in expired)
        {
            hitCooldowns.Remove(col);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCooldowns.ContainsKey(other)) return; // �N�[���^�C����

        CharacterAttributes target = other.GetComponent<CharacterAttributes>();
        if (target != null && target.faction != ownerFaction)
        {
            float finalDamage = damage * ElementUtils.GetEffectivenessMultiplier(attackElement, target.elementType);
            target.ApplyDamage(finalDamage, attackElement, transform.position, ownerFaction); // �����̔����ʒu���U���҂̈ʒu�Ƃ��ēn��
            Debug.Log($"[���@�e] {target.name} �� {finalDamage} �_���[�W�I�i{attackElement}�j");

            hitCooldowns[other] = Time.time; // �N�[���^�C���J�n
        }
    }
}