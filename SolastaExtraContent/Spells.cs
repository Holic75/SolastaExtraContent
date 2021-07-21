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

        internal static void create()
        {
            createHellishRebuke();
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
