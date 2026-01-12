public interface IDamageable
{
    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="dmg">伤害</param>
    /// <param name="attackDir">攻击方向（用于 knock back）</param>
    void TakeDamage(float dmg, float attackDir);
}