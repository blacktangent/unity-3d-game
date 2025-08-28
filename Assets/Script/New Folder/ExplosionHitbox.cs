using UnityEngine;
using System.Collections.Generic;

public class ExplosionHitbox : MonoBehaviour
{
    public float damage = 15f;
    public ElementType attackElement = ElementType.Fire;
    public FactionType ownerFaction;

    public float cooldownTime = 1.0f; // 同じ相手に再び当たれるまでの時間（秒）

    private Dictionary<Collider, float> hitCooldowns = new Dictionary<Collider, float>();

    void Update()
    {
        // 一定時間が経過したら、当たり判定リセット
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
        if (hitCooldowns.ContainsKey(other)) return; // クールタイム中

        CharacterAttributes target = other.GetComponent<CharacterAttributes>();
        if (target != null && target.faction != ownerFaction)
        {
            float finalDamage = damage * ElementUtils.GetEffectivenessMultiplier(attackElement, target.elementType);
            target.ApplyDamage(finalDamage, attackElement, transform.position, ownerFaction); // 爆発の発生位置を攻撃者の位置として渡す
            Debug.Log($"[魔法弾] {target.name} に {finalDamage} ダメージ！（{attackElement}）");

            hitCooldowns[other] = Time.time; // クールタイム開始
        }
    }
}