using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModHelpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;

namespace SolastaExtraContent
{
    public class Spells
    {
        static public NewFeatureDefinitions.ReactionOnDamageSpell hellish_rebuke;
        static public SpellDefinition polymorph;

        internal static void create()
        {
            createHellishRebuke();
            createPolymorph();
        }


        static void createPolymorph()
        {
            var title_string = "Spell/&PolymorphTitle";
            var description_string = "Spell/&PolymorphDescription";
            var sprite = DatabaseHelper.SpellDefinitions.IdentifyCreatures.GuiPresentation.spriteReference;
            DatabaseHelper.MonsterDefinitions.ConjuredOneBeastTiger_Drake.SetFullyControlledWhenAllied(true);

            var wolf = Common.createPolymoprhUnit(DatabaseHelper.MonsterDefinitions.Wolf, "PolymorphTestWolf", "", "", "");
            var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.Polymorph>.createFeature("PolymorphTestFeature",
                                                                                                 "",
                                                                                                 title_string,
                                                                                                 description_string,
                                                                                                 null,
                                                                                                 a =>
                                                                                                 {
                                                                                                     a.monster = wolf;
                                                                                                     a.transferFeatures = true;
                                                                                                     a.statsToTransfer = new string[] { Helpers.Stats.Charisma, Helpers.Stats.Intelligence, Helpers.Stats.Wisdom };
                                                                                                     a.transferSpells = true;
                                                                                                 });
            
            var condition = Helpers.ConditionBuilder.createCondition("PolymorphCondition",
                                                                    "",
                                                                    title_string,
                                                                    Common.common_no_title,
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBarkskin,
                                                                    feature
                                                                    );

            
            condition.conditionTags.Clear();
            condition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Barkskin.EffectDescription);
            effect.EffectForms.Clear();

            effect.durationType = RuleDefinitions.DurationType.Minute;

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);
            
            polymorph = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithRestricitons>.createSpell("PolymorphTestSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       wolf.GuiPresentation.spriteReference,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       1,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Transmutation
                                                                                       );
            polymorph.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizard, polymorph);
        }

        static void createHellishRebuke()
        {
            var title_string = "Spell/&HellishRebukeTitle";
            var description_string = "Spell/&HellishRebukeDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("HellishRebukeCantripImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/HellishRebuke.png",
                                                    128, 128);

  
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.SacredFlame.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 12;
            effect.ignoreCover = false;


            var effect_form = new EffectForm();

            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 2;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D10;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Fire;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

            hellish_rebuke = Helpers.GenericSpellBuilder<NewFeatureDefinitions.ReactionOnDamageSpell>.createSpell("HellishRebukeSpell",
                                                                                                                   "",
                                                                                                                   title_string,
                                                                                                                   description_string,
                                                                                                                   sprite,
                                                                                                                   effect,
                                                                                                                   RuleDefinitions.ActivationTime.Reaction,
                                                                                                                   1,
                                                                                                                   false,
                                                                                                                   true,
                                                                                                                   true,
                                                                                                                   Helpers.SpellSchools.Evocation
                                                                                                                   );
            hellish_rebuke.materialComponentType = RuleDefinitions.MaterialComponentType.None;
        }
    }
}
