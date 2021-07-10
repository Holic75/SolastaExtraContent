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
using static FeatureDefinitionSavingThrowAffinity;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;

namespace SolastaExtraContent
{
    public class Cantrips
    {
        public static NewFeatureDefinitions.SpellFollowedByAttack sunlight_blade;


        internal static void create()
        {
            createSunlightBlade();
        }


        static void createSunlightBlade()
        {
            var title_string = "Spell/&SunlightBladeTitle";
            var description_string = "Spell/&SunlightBladeDescription";
            var levels = new int[] { 4, 10, 16, 20 };
            List<(int, EffectDescription)> effects = new List<(int, EffectDescription)>();

            var highlighted_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("SunlightBladeHighlightedCondition",
                                                                                                  "",
                                                                                                  "",
                                                                                                  "",
                                                                                                  null,
                                                                                                  DatabaseHelper.ConditionDefinitions.ConditionHighlighted,
                                                                                                  new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacked },
                                                                                                  DatabaseHelper.ConditionDefinitions.ConditionHighlighted.Features[0]
                                                                                                  );

            var on_hit_effect = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone>
                                                            .createFeature("SunlightBladeHighlightedOnHitEffect",
                                                                           "",
                                                                           Common.common_no_title,
                                                                           Common.common_no_title,
                                                                           Common.common_no_icon,
                                                                           a =>
                                                                           {
                                                                               a.durationType = RuleDefinitions.DurationType.Round;
                                                                               a.durationValue = 1;
                                                                               a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                               a.condition = highlighted_condition;
                                                                               a.onlyWeapon = true;
                                                                           }
                                                                           );

            for (int i = 0; i < levels.Length; i++)
            {
                var lvl = levels[i];
                var extra_damage = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("SunlightBladeExtraDamage" + lvl.ToString(),
                                                                                                                   "",
                                                                                                                   Common.common_no_title,
                                                                                                                   Common.common_no_title,
                                                                                                                   Common.common_no_icon,
                                                                                                                   DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor,
                                                                                                                   a =>
                                                                                                                   {
                                                                                                                       a.damageAdvancement = RuleDefinitions.AdditionalDamageAdvancement.None;
                                                                                                                       a.specificDamageType = Helpers.DamageTypes.Radiant;
                                                                                                                       a.SetDamageDieType(RuleDefinitions.DieType.D8);
                                                                                                                       a.SetDamageDiceNumber(i);
                                                                                                                       a.SetNotificationTag("SunlightBlade");
                                                                                                                       
                                                                                                                   }
                                                                                                                   );
                extra_damage.SetAddLightSource(true);
                extra_damage.lightSourceForm = DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.LightSourceForm;
                var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("GlacialBladeCasterCondition" + lvl.ToString(),
                                                                                          "",
                                                                                          title_string,
                                                                                          description_string,
                                                                                          null,
                                                                                          DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle,
                                                                                          new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
                                                                                          extra_damage,
                                                                                          on_hit_effect);

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.DivineFavor.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Touch);
                effect.SetRangeParameter(1);
                effect.DurationParameter = 1;
                effect.DurationType = RuleDefinitions.DurationType.Round;
                effect.EffectForms.Clear();
                effect.SetTargetType(RuleDefinitions.TargetType.Individuals);
                effect.SetTargetSide(RuleDefinitions.Side.Enemy);
                effect.SetTargetParameter(1);
                effect.SetTargetParameter2(1);

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = condition;
                effect_form.conditionForm.applyToSelf = true;
                effect.EffectForms.Add(effect_form);

                effects.Add((lvl, effect));
            }
            sunlight_blade = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellFollowedByAttack>.createSpell("SunlightBladeSpell",
                                                                                        "",
                                                                                        title_string,
                                                                                        description_string,
                                                                                        DatabaseHelper.SpellDefinitions.FlameBlade.GuiPresentation.SpriteReference,
                                                                                        effects[0].Item2,
                                                                                        RuleDefinitions.ActivationTime.Action,
                                                                                        0,
                                                                                        false,
                                                                                        true,
                                                                                        true,
                                                                                        Helpers.SpellSchools.Evocation
                                                                                        );
            sunlight_blade.materialComponentType = RuleDefinitions.MaterialComponentType.None;
            sunlight_blade.levelEffectList = effects.Skip(1).ToList();
            sunlight_blade.minCustomEffectLevel = 5;
            DatabaseHelper.SpellListDefinitions.SpellListWizard.spellsByLevel[0].spells.Add(sunlight_blade);
        }
    }
}
