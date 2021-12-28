using System;
using System.Collections;
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



public class CharacterActionElementalForm : CharacterAction
{
    public CharacterActionElementalForm(CharacterActionParams actionParams)
      : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        CharacterActionElementalForm characterActionElementalForm = this;
        yield return (object)null;
        IGameLocationActionService service = ServiceRepository.GetService<IGameLocationActionService>();
        CharacterActionParams actionParams = characterActionElementalForm.ActionParams.Clone();
        actionParams.ActionDefinition = service.AllActionDefinitions[ActionDefinitions.Id.PowerBonus];
        if (characterActionElementalForm.ActingCharacter.RulesetCharacter is RulesetCharacterMonster rulesetCharacter && rulesetCharacter.IsSubstitute)
        {
            rulesetCharacter.RemoveAllConditionsOfCategoryAndType("17TagConjure", "ConditionWildShapeSubstituteForm");
            actionParams.ActingCharacter = rulesetCharacter.OriginalFormCharacter.EntityImplementation as GameLocationCharacter;
        }
        service.ExecuteAction(actionParams, (CharacterAction.ActionExecutedHandler)null, true);
    }
}
