using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElementUtils
{
    // 有効な攻撃か（効果バツグンか）
    public static bool IsEffective(ElementType attacker, ElementType target)
    {
        return
            (attacker == ElementType.Fire && target == ElementType.Esp) ||
            (attacker == ElementType.Water && target == ElementType.Fire) ||
            (attacker == ElementType.Electric && target == ElementType.Water) ||
            (attacker == ElementType.Light && target == ElementType.Electric) ||
            (attacker == ElementType.Esp && target == ElementType.Light) ||
            (attacker == ElementType.Star && target == ElementType.Fire);
    }

    // 弱点判定（逆向きでも同じ判定が取れる）
    public static bool IsWeakAgainst(ElementType target, ElementType attacker)
    {
        return IsEffective(attacker, target);
    }

    // 相性に応じたダメージ倍率
    public static float GetEffectivenessMultiplier(ElementType attacker, ElementType defender)
    {
        if (attacker == ElementType.None) return 1.0f;
        if (IsEffective(attacker, defender)) return 2.0f;
        if (IsEffective(defender, attacker)) return 0.5f;
        return 1.0f; // 通常
    }
}
