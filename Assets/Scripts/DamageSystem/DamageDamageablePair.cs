public struct DamageDamageablePair
{
    public readonly DamageType     damageType;
    public readonly DamageableType damageableType;

    public DamageDamageablePair(DamageType damageType, DamageableType damageableType)
    {
        this.damageType     = damageType;
        this.damageableType = damageableType;
    }

    public int GetDamage()
    {
        return Helper.DamageValues[this];
    }
    
    public static bool operator == (DamageDamageablePair a, DamageDamageablePair b)
    {
        return a.damageType == b.damageType && a.damageableType == b.damageableType;
    }

    public static bool operator !=(DamageDamageablePair a, DamageDamageablePair b)
    {
        return a.damageType != b.damageType || a.damageableType != b.damageableType;
    }
}