using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    public float damage = 10f;
    public ElementType attackElement = ElementType.None;
    public FactionType ownerFaction;
    public float cooldownTime = 1.0f;

    private Dictionary<Collider, float> hitCooldowns = new Dictionary<Collider, float>();

    void Update()
    {
        // �Â��q�b�g�L�^�̏���
        List<Collider> expired = new List<Collider>();
        foreach (var pair in hitCooldowns)
        {
            if (Time.time - pair.Value > cooldownTime)
                expired.Add(pair.Key);
        }
        foreach (var col in expired)
            hitCooldowns.Remove(col);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCooldowns.TryGetValue(other, out float lastHitTime))
        {
            if (Time.time - lastHitTime < cooldownTime)
                return; // �N�[���_�E�����͖���
        }

        CharacterAttributes targetAttr = other.GetComponent<CharacterAttributes>();
        if (targetAttr != null && targetAttr.faction != ownerFaction)
        {
            float finalDamage = damage * ElementUtils.GetEffectivenessMultiplier(attackElement, targetAttr.elementType);
            targetAttr.ApplyDamage(finalDamage, attackElement, transform.position, ownerFaction); // �� �C���I
            Debug.Log($"[�U��] {targetAttr.name} �� {finalDamage} �_���[�W�I�i{attackElement}�j");

            hitCooldowns[other] = Time.time; // �N�[���_�E���J�n
        }
    }
}