using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    /*
    public ElementType attackElement;
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        // キャラクターならダメージ処理
        var target = other.GetComponent<CharacterAttributes>();
        if (target != null)
        {
            Debug.Log($"攻撃ヒット！（キャラ）対象: {target.name} 属性: {attackElement}");
            target.ApplyDamage(damage, attackElement);
            return;
        }

        // キューブなど受け身のオブジェクトにも対応
        var receiver = other.GetComponent<DamageReceiver>();
        if (receiver != null)
        {
            Debug.Log($"攻撃ヒット！（オブジェクト）対象: {other.name} 属性: {attackElement}");
            // 将来的に属性で色を変えるなどの処理をDamageReceiver側に追加してもOK
            receiver.OnDamage(attackElement);
        }
    }
    */
}
