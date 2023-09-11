
public class Enemy4 : Monster
{
    public override void Damage(float damage, float armorpen, DamageSource source, IAttacking killer)
    {
        MoveSpeed.Modify(-0.99f, BonusType.Percentage, "enemy4giddy", 0.2f, 1);

        anim.SetTrigger("giddy");

        base.Damage(damage, armorpen, source, killer);  
    }
}
