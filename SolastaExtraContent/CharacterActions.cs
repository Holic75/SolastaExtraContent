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
