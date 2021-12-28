using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CharacterActionFuriousAttack : CharacterActionApplyConditionsToSelfUntilRoundEnd
{
    public CharacterActionFuriousAttack(CharacterActionParams actionParams)
        : base(actionParams)
    {
    }

    public override string[] getConditions()
    {
        return new string[] { "FuriousFeatPowerAttackCondition" };
    }
}



public class CharacterActionRapidShot : CharacterActionApplyConditionsToSelfUntilRoundEnd
{
    public CharacterActionRapidShot(CharacterActionParams actionParams)
        : base(actionParams)
    {
    }

    public override string[] getConditions()
    {
        return new string[] { "FastShooterFeatRapidShotCondition" };
    }
}
