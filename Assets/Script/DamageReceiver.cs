using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DamageReceiver : MonoBehaviour
{/*
    private Renderer rend;
    private Color originalColor;
    public float destroyDelay = 1.0f;

    private ElementType myElement;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        // 自分の属性を取得
        ElementalAttributes attr = GetComponent<ElementalAttributes>();
        if (attr != null)
            myElement = attr.elementType;
        else
            myElement = ElementType.None;
    }

    public void OnDamage(ElementType attackerElement)
    {
        // 攻撃側（プレイヤー）の属性が、防御側（このキューブ）に効果的か？
        if (ElementUtils.IsEffective(attackerElement, myElement))
        {
            Debug.Log("属性が効果的！オブジェクトを破壊");

            // 赤くしてから一定時間後に消える
            rend.material.color = Color.red;
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Debug.Log("属性が効かない");

            // 効果なしの場合、色を変えるだけ（ここでは灰色）
            rend.material.color = Color.gray;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        AttackHitbox attack = other.GetComponent<AttackHitbox>();
        if (attack != null)
        {
            OnDamage(attack.attackElement); // ← OnDamage に一元化
        }
    }
    */
}
