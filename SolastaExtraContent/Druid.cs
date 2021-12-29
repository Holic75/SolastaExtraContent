using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;
using SolastaModHelpers;
using UnityEngine.AddressableAssets;
using SolastaModHelpers.NewFeatureDefinitions;

namespace SolastaExtraContent
{
    public class Druid
    {
        static public ConditionDefinition inside_spirit_area_condition;
        static public EffectProxyDefinition spirit_proxy;
        static public FeatureDefinitionFeatureSet spirits;
        static public FeatureDefinitionFeatureSet spirit_summoner;
        static public NewFeatureDefinitions.AddExtraConditionToTargetOnConditionApplication guardian_spirits;
        static public NewFeatureDefinitions.PowerBundle base_spirit;

        static public FeatureDefinition elemental_form_mark;
        static public NewFeatureDefinitions.LinkedPower elemental_form;
        static public NewFeatureDefinitions.PowerBundle elemental_healing;
        static public FeatureDefinitionActionAffinity elemental_form_affinity;
        static public FeatureDefinition elemental_healing_description;
        static public List<FeatureDefinitionPower> elemetal_healing_powers = new List<FeatureDefinitionPower>();
        static public NewFeatureDefinitions.AddAttackTagIfHasFeature primal_attacks;
        static public NewFeatureDefinitions.MonsterAdditionalDamage elemental_damage;
        static public NewFeatureDefinitions.MonsterAdditionalDamage elemental_strike;




        public static void create()
        {
            createCircleOfElements();
            createCircleOfSpirits();
        }


        static void createCircleOfElements()
        {
            createElementalHealing();
            createElementalForms();
            createPrimalAttacks();
            createElementalStrike();
            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&DruidSubclassCircleOfElementsDescription",
                    "Subclass/&DruidSubclassCircleOfElementsTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionShockArcanist.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("DruidSubclassCircleOfElements", "fce3fd2e-0bba-4fdc-98da-460c0249108e")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(elemental_form, 2)
                                                                                            .AddFeatureAtLevel(elemental_form_affinity, 2)
                                                                                            .AddFeatureAtLevel(elemental_healing_description, 2)
                                                                                            .AddFeatureAtLevel(primal_attacks, 6)
                                                                                            .AddFeatureAtLevel(elemental_strike, 10)
                                                                                            .AddToDB();
            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle.Subclasses.Add(definition.Name);
        }



        static void createElementalStrike()
        {
            var additional_damage_proxy = Helpers.FeatureBuilder<NewFeatureDefinitions.MonsterAdditionalDamageProxy>
                .createFeature("DruidSubclassCircleOfElementsElementalStrikeProxy",
                               "",
                               Common.common_no_title,
                               Common.common_no_title,
                               Common.common_no_icon,
                               a =>
                               {
                                   a.additionalDamageType = RuleDefinitions.AdditionalDamageType.SameAsBaseDamage;
                                   a.damageAdvancement = RuleDefinitions.AdditionalDamageAdvancement.SlotLevel;
                                   a.triggerCondition = RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot;
                                   a.spellcastingFeature = DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDruid;
                                   a.limitedUsage = RuleDefinitions.FeatureLimitedUsage.OnceInMyturn;
                                   a.notificationTag = "DruidSubclassCircleOfElementsElementalStrike";
                                   a.damageDieType = RuleDefinitions.DieType.D8;
                                   a.damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.Die;
                                   a.damageDiceNumber = 1;
                                   var list = new (int, int)[10];
                                   for (int i = 0; i < 10; i++)
                                   {
                                       list[i] = (i + 1, i + 1);
                                   }

                                   a.diceByRankTable = Helpers.Misc.createDiceRankTable(10, list);
                                   a.impactParticle = DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark.ImpactParticle;
                                   a.restricitons.Add(new NewFeatureDefinitions.HasFeatureRestriction(elemental_form_mark));
                               }
                               );

            elemental_strike = Helpers.FeatureBuilder<NewFeatureDefinitions.MonsterAdditionalDamage>.createFeature("DruidSubclassCircleOfElementsElementalStrike",
                                                                                                                   "",
                                                                                                                   "Feature/&DruidSubclassCircleOfElementsElementalStrikeTitle",
                                                                                                                   "Feature/&DruidSubclassCircleOfElementsElementalStrikeDescription",
                                                                                                                   Common.common_no_icon,
                                                                                                                   b =>
                                                                                                                   {
                                                                                                                       b.provider = additional_damage_proxy;
                                                                                                                   }
                                                                                                                   );
        }


        static void createPrimalAttacks()
        {
            primal_attacks = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagIfHasFeature>.createFeature("DruidSubclassCircleOfElementsPrimalAttacks",
                                                                                                                  "",
                                                                                                                  "Feature/&DruidSubclassCircleOfElementsPrimalAttacksTitle",
                                                                                                                  "Feature/&DruidSubclassCircleOfElementsPrimalAttacksDescription",
                                                                                                                  Common.common_no_icon,
                                                                                                                  a =>
                                                                                                                  {
                                                                                                                      a.requiredFeature = elemental_form_mark;
                                                                                                                      a.tag = "Magical";
                                                                                                                  }
                                                                                                                  );
        }


        static void createElementalHealing()
        {
            var base_eff = new EffectDescription();
            base_eff.Copy(DatabaseHelper.SpellDefinitions.CureWounds.EffectDescription);
            base_eff.effectForms.Clear();
            base_eff.durationType = RuleDefinitions.DurationType.UntilLongRest;

            elemental_healing = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerBundle>
                                    .createPower("DruidSubclassCircleOfElementsElementalHealingPower",
                                                 "",
                                                 "Feature/&DruidSubclassCircleOfElementsElementalHealingPowerTitle",
                                                 "Feature/&DruidSubclassCircleOfElementsElementalHealingPowerDescription",
                                                 DatabaseHelper.FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury.guiPresentation.SpriteReference,
                                                 base_eff,
                                                 RuleDefinitions.ActivationTime.BonusAction,
                                                 2,
                                                 RuleDefinitions.UsesDetermination.Fixed,
                                                 RuleDefinitions.RechargeRate.SpellSlot
                                                 );
            elemental_healing.spellcastingFeature = DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDruid;
            elemetal_healing_powers.Add(elemental_healing);
            for (int i = 1; i < 10; i++)
            {
                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.CureWounds.EffectDescription);
                effect.effectForms.Clear();
                effect.effectAdvancement.Clear();
                effect.targetType = RuleDefinitions.TargetType.Self;
                effect.rangeParameter = 1;
                effect.rangeType = RuleDefinitions.RangeType.Self;
                effect.createdByCharacter = true;
                var effect_form = new EffectForm();
                effect_form.formType = EffectForm.EffectFormType.Healing;
                effect_form.healingForm = new HealingForm();
                effect_form.healingForm.diceNumber = i;
                effect_form.healingForm.dieType = RuleDefinitions.DieType.D8;
                effect_form.healingForm.healingComputation = RuleDefinitions.HealingComputation.Dice;
                effect.EffectForms.Add(effect_form);
                
                var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>
                                                                   .createPower($"DruidSubclassCircleOfElementsElementalHealingPower{i}",
                                                                                "",
                                                                                Helpers.StringProcessing.appendToString("Feature/&DruidSubclassCircleOfElementsElementalHealingPowerTitle",
                                                                                                                        $"Feature/&DruidSubclassCircleOfElementsElementalHealingPower{i}Title",
                                                                                                                        $" {Gui.ToRoman(i)}"
                                                                                                                        ),
                                                                                "Feature/&DruidSubclassCircleOfElementsElementalHealingPowerDescription",
                                                                                elemental_healing.guiPresentation.spriteReference,
                                                                                effect,
                                                                                RuleDefinitions.ActivationTime.BonusAction,
                                                                                2,
                                                                                RuleDefinitions.UsesDetermination.Fixed,
                                                                                RuleDefinitions.RechargeRate.SpellSlot,
                                                                                cost_per_use: i
                                                                                );
                power.spellcastingFeature = DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDruid;
                elemental_healing.addSubPower(power);
                elemetal_healing_powers.Add(power);
                power.restrictions.Add(new NewFeatureDefinitions.HasAvailableSpellSlot(i, DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDruid, exact: true));
            }

            elemental_healing_description = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("DruidSubclassCircleOfElementsElementalHealing",
                                                                                                                "",
                                                                                                                "Feature/&DruidSubclassCircleOfElementsElementalHealingPowerTitle",
                                                                                                                "Feature/&DruidSubclassCircleOfElementsElementalHealingPowerDescription");
        }


        static void createElementalForms()
        {
            DatabaseHelper.MonsterDefinitions.SkarnGhoul.armorClass = 15;
            DatabaseHelper.MonsterDefinitions.SkarnGhoul.features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
            DatabaseHelper.MonsterDefinitions.SkarnGhoul.features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance);
            DatabaseHelper.MonsterDefinitions.SkarnGhoul.features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance);

            elemental_form_mark = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("DruidSubclassCircleOfElementsElementalFormMark",
                                                                                                     "",
                                                                                                     Common.common_no_title,
                                                                                                     Common.common_no_title);
            Dictionary<string, MonsterDefinition> monsters = new Dictionary<string, MonsterDefinition>
            {
                {"FireJester", DatabaseHelper.MonsterDefinitions.Fire_Jester},
                {"FireOsprey", DatabaseHelper.MonsterDefinitions.Fire_Osprey},
                {"WindSnake", DatabaseHelper.MonsterDefinitions.WindSnake},
                {"SkarnGhoul", DatabaseHelper.MonsterDefinitions.SkarnGhoul},
                {"AirElemental", DatabaseHelper.MonsterDefinitions.Air_Elemental},
                {"FireElemental", DatabaseHelper.MonsterDefinitions.Fire_Elemental},
                {"EarthElemental", DatabaseHelper.MonsterDefinitions.Earth_Elemental},
            };

            foreach (var key in monsters.Keys.ToArray())
            {
                var old_tags = monsters[key].creatureTags;
                monsters[key] = Helpers.CopyFeatureBuilder<MonsterDefinition>.createFeatureCopy($"DruidSubclassCircleOfElements{key}Unit",
                                                                                                "",
                                                                                                "",
                                                                                                "",
                                                                                                null,
                                                                                                monsters[key]);
                monsters[key].alignment = "Unaligned";
                monsters[key].creatureTags = new List<string>() { "WildShape" };
                monsters[key].creatureTags.AddRange(old_tags);
                monsters[key].features.Add(elemental_form_mark);
                monsters[key].features.AddRange(elemetal_healing_powers);
                monsters[key].fullyControlledWhenAllied = false;
                monsters[key].dungeonMakerPresence = MonsterDefinition.DungeonMaker.None;
                monsters[key].bestiaryEntry = BestiaryDefinitions.BestiaryEntry.None;
            }

            var effect_description = new EffectDescription();
            effect_description.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.effectDescription);
            effect_description.effectForms.Clear();
            var form = new EffectForm();
            form.formType = EffectForm.EffectFormType.ShapeChange;
            form.createdByCharacter = true;
            form.shapeChangeForm = new ShapeChangeForm();
            form.shapeChangeForm.keepMentalAbilityScores = true;
            form.shapeChangeForm.shapeChangeType = ShapeChangeForm.Type.ClassLevelListSelection;
            form.shapeChangeForm.specialSubstituteCondition = DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.effectDescription.effectForms[0].shapeChangeForm.specialSubstituteCondition;
            form.shapeChangeForm.shapeOptions = new List<ShapeOptionDescription>()
            {
                new ShapeOptionDescription()
                {
                    requiredLevel = 2,
                    substituteMonster = monsters["FireJester"]
                },
                new ShapeOptionDescription()
                {
                    requiredLevel = 5,
                    substituteMonster = monsters["WindSnake"]
                },
                new ShapeOptionDescription()
                {
                    requiredLevel = 5,
                    substituteMonster = monsters["SkarnGhoul"]
                },
                new ShapeOptionDescription()
                {
                    requiredLevel = 8,
                    substituteMonster = monsters["FireOsprey"]
                },
                new ShapeOptionDescription()
                {
                    requiredLevel = 11,
                    substituteMonster = monsters["AirElemental"]
                },
                new ShapeOptionDescription()
                {
                    requiredLevel = 11,
                    substituteMonster = monsters["FireElemental"]
                },
                new ShapeOptionDescription()
                {
                    requiredLevel = 11,
                    substituteMonster = monsters["EarthElemental"]
                }
            };
            effect_description.EffectForms.Add(form);
            elemental_form = Helpers.GenericPowerBuilder<NewFeatureDefinitions.LinkedPower>.createPower("DruidSubclassCircleOfElementsElementalFormPower",
                                                                                                          "",
                                                                                                          "Feature/&DruidSubclassCircleOfElementsElementalFormTitle",
                                                                                                          "Feature/&DruidSubclassCircleOfElementsElementalFormDescription",
                                                                                                          null,
                                                                                                          effect_description,
                                                                                                          RuleDefinitions.ActivationTime.BonusAction,
                                                                                                          2,
                                                                                                          RuleDefinitions.UsesDetermination.Fixed,
                                                                                                          RuleDefinitions.RechargeRate.ShortRest);
            elemental_form.delegatedToAction = true;
            elemental_form.linkedPower = DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape;


            var elemental_form_action = SolastaModHelpers.Helpers.CopyFeatureBuilder<ActionDefinition>
                                                                    .createFeatureCopy("ElementalForm",
                                                                        "9d796b3e-8d9c-485f-b906-2272da28cc92",
                                                                        elemental_form.guiPresentation.title,
                                                                        elemental_form.guiPresentation.description,
                                                                        DatabaseHelper.ActionDefinitions.WildShape.guiPresentation.spriteReference,
                                                                        DatabaseHelper.ActionDefinitions.WildShape);
            elemental_form_action.id = (ActionDefinitions.Id)ExtendedActionId.ElementalForm;
            elemental_form_action.usesPerTurn = -1;
            elemental_form_action.activatedPower = elemental_form;
            elemental_form_action.actionType = ActionDefinitions.ActionType.Bonus;
            elemental_form_action.requiresAuthorization = true;
            ActionData.addActionRestrictions(elemental_form_action, 
                                             new NewFeatureDefinitions.NoConditionRestriction(DatabaseHelper.ConditionDefinitions.ConditionWildShapeSubstituteForm),
                                             new NewFeatureDefinitions.HasAvailablePowerUses(elemental_form));

            elemental_form_affinity = Helpers.CopyFeatureBuilder<FeatureDefinitionActionAffinity>.createFeatureCopy("DruidSubclassCircleOfElementsElementalFormActionAffinity",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    null,
                                                                                                                    DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityWildShapeRevertShape,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.authorizedActions = new List<ActionDefinitions.Id>
                                                                                                                        {
                                                                                                                            elemental_form_action.id
                                                                                                                        };
                                                                                                                    }
                                                                                                                    );
            elemental_form_affinity.guiPresentation.hidden = true;
        }


        static void createCircleOfSpirits()
        {
            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&DruidSubclassCircleOfSpiritsDescription",
                    "Subclass/&DruidSubclassCircleOfSpiritsTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.SorcerousChildRift.GuiPresentation.SpriteReference)
                    .Build();

            createSpirits();
            createSpiritSummoner();
            createGuardianSpirits();
            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("DruidSubclassCircleOfSpirits", "75856caa-9d96-4d59-9b6e-64b0f9256511")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(spirits, 2)
                                                                                            .AddFeatureAtLevel(spirit_summoner, 6)
                                                                                            .AddFeatureAtLevel(guardian_spirits, 10)
                                                                                            .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle.Subclasses.Add(definition.Name);


            //regrant powers to give base_primal_harmony and wrath of the elements missing previously
            Action<RulesetCharacterHero> fix_action = c =>
            {
                if (c.ClassesAndSubclasses.ContainsValue(definition) && !c.UsablePowers.Any(u => u.powerDefinition == base_spirit))
                {
                    c.GrantPowers();
                }
            };
            Common.postload_actions.Add(fix_action);
        }


        static void createSpirits()
        {
            inside_spirit_area_condition = Helpers.ConditionBuilder.createCondition("DruidSubclassCircleOfSpiritsInsideSpiritAreaCondition",
                                                                                    "",
                                                                                    Common.common_no_title,
                                                                                    Common.common_no_title,
                                                                                    Common.common_no_icon,
                                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed
                                                                                    );
            spirit_proxy = Helpers.CopyFeatureBuilder<EffectProxyDefinition>.createFeatureCopy("DruidSubclassCircleOfSpiritsSpiritProxy",
                                                                                               "",
                                                                                               "Feature/&DruidSubclassCircleOfSpiritProxyTitle",
                                                                                               "",
                                                                                               null,
                                                                                               DatabaseHelper.EffectProxyDefinitions.ProxySilence
                                                                                               );
            //inside_spirit_area_condition.silentWhenAdded = true;
            //inside_spirit_area_condition.silentWhenRemoved = true;
            //inside_spirit_area_condition.guiPresentation.hidden = true;

            //protection spirit +1 AC, Saves and advantage on concentration checks
            var ac_bonus = DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeraldOfBattle;
            var saves_bonus = DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityHeraldOfBattle;
            var concentration_advantage = Helpers.CopyFeatureBuilder<FeatureDefinitionMagicAffinity>.createFeatureCopy("DruidSubclassCircleOfSpiritsProtectionConcentrationBonus",
                                                                                                                "",
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_title,
                                                                                                                null,
                                                                                                                DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.overConcentrationThreshold = 0;
                                                                                                                }
                                                                                                                );

            //hunt spirit
            //+1 attack / damage, advantage on perception checks
            var attack_damage_bonus = DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierHeraldOfBattle;
            var perception_advantage = Helpers.AbilityCheckAffinityBuilder.createSkillCheckAffinity("DruidSubclassCircleOfSpiritsHuntPerceptionBonus",
                                                                                                    "",
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_icon,
                                                                                                    RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                                                                                                    0,
                                                                                                    RuleDefinitions.DieType.D1,
                                                                                                    Helpers.Skills.Perception
                                                                                                    );

            //healing spirit
            //targets regain max hitpoints when healed, advantage on death saves
            var max_healing = DatabaseHelper.FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope;
            var death_saves_advantage = DatabaseHelper.FeatureDefinitionDeathSavingThrowAffinitys.DeathSavingThrowAffinityBeaconOfHope;

            var protection_spirit = createSpiritPower("DruidSubclassCircleOfSpiritsProtectionSpirit",
                                                      "Feature/&DruidSubclassCircleOfSpiritsProtectionSpiritTitle",
                                                      "Feature/&DruidSubclassCircleOfSpiritsProtectionSpiritDescription",
                                                      DatabaseHelper.FeatureDefinitionPowers.PowerPaladinAuraOfCourage.guiPresentation.spriteReference,
                                                      ac_bonus,
                                                      saves_bonus,
                                                      concentration_advantage
                                                      );

            var hunt_spirit = createSpiritPower("DruidSubclassCircleOfSpiritsHuntSpirit",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHuntSpiritTitle",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHuntSpiritDescription",
                                                  DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandVolcanicAura.guiPresentation.SpriteReference,
                                                  attack_damage_bonus,
                                                  perception_advantage
                                                  );

            var healing_spirit = createSpiritPower("DruidSubclassCircleOfSpiritsHealingSpirit",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHealingSpiritTitle",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHealingSpiritDescription",
                                                  DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarAuraTruth.guiPresentation.SpriteReference,
                                                  max_healing,
                                                  death_saves_advantage
                                                  );


            var spirit_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.TerminateEffectsOnPowerUse>.createFeature("DruidSubclassCircleOfSpiritsSpiritWatcher",
                                                                                                                        "",
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_icon,
                                                                                                                        a =>
                                                                                                                        {
                                                                                                                            a.powers = new List<FeatureDefinitionPower> { hunt_spirit, healing_spirit, protection_spirit };
                                                                                                                            a.effectsToTerminate = a.powers.Cast<FeatureDefinition>().ToList();
                                                                                                                        }
                                                                                                                        );

            var base_effect = new EffectDescription();
            base_effect.Copy(DatabaseHelper.SpellDefinitions.Silence.effectDescription);
            base_effect.rangeParameter = 12;
            base_effect.durationParameter = 1;
            base_effect.effectForms.Clear();

            base_spirit = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerBundle>
                                                                        .createPower("DruidSubclassCircleOfSpiritsSpiritsBasePower",
                                                                                       "",
                                                                                       "Feature/&DruidSubclassCircleOfSpiritsSpiritsFeatureSetTitle",
                                                                                       "Feature/&DruidSubclassCircleOfSpiritsSpiritsFeatureSetDescription",
                                                                                       DatabaseHelper.FeatureDefinitionPowers.PowerSorcererCreateSorceryPoints.guiPresentation.spriteReference,
                                                                                       base_effect,
                                                                                       RuleDefinitions.ActivationTime.BonusAction,
                                                                                       2,
                                                                                       RuleDefinitions.UsesDetermination.Fixed,
                                                                                       RuleDefinitions.RechargeRate.ShortRest);
            base_spirit.linkedPower = DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape;
            base_spirit.addSubPower(healing_spirit);
            base_spirit.addSubPower(hunt_spirit);
            base_spirit.addSubPower(protection_spirit);

            spirits = Helpers.FeatureSetBuilder.createFeatureSet("DruidSubclassCircleOfSpiritsSpiritsFeatureSet",
                                                                "",
                                                                "Feature/&DruidSubclassCircleOfSpiritsSpiritsFeatureSetTitle",
                                                                "Feature/&DruidSubclassCircleOfSpiritsSpiritsFeatureSetDescription",
                                                                true,
                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                false,
                                                                base_spirit,
                                                                healing_spirit,
                                                                hunt_spirit,
                                                                protection_spirit,
                                                                spirit_watcher
                                                                );
        }


        static FeatureDefinitionPower createSpiritPower(string name, string title, string description, AssetReferenceSprite power_sprite, params FeatureDefinition[] features)
        {
            var condition = Helpers.ConditionBuilder.createCondition(name + "AreaCondition",
                                                                    "",
                                                                    title.Replace("Feature/&", "Rules/&Condition"),
                                                                    description.Replace("Feature/&", "Rules/&Condition"),
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed,
                                                                    features
                                                                    );
            condition.parentCondition = inside_spirit_area_condition;
            condition.allowMultipleInstances = false;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Silence.effectDescription);
            effect.rangeParameter = 12;
            effect.durationParameter = 1;
            effect.effectForms.Clear();
            var condition_form = new EffectForm();
            condition_form.formType = EffectForm.EffectFormType.Condition;
            condition_form.conditionForm = new ConditionForm();
            condition_form.createdByCharacter = true;
            condition_form.conditionForm.conditionDefinition = condition;
            effect.effectForms.Add(condition_form);
            var summon_form = new EffectForm();
            summon_form.formType = EffectForm.EffectFormType.Summon;
            summon_form.summonForm = new SummonForm();
            summon_form.createdByCharacter = true;
            summon_form.summonForm.summonType = SummonForm.Type.EffectProxy;
            summon_form.summonForm.effectProxyDefinitionName = spirit_proxy.name;
            summon_form.summonForm.number = 1;
            effect.effectForms.Add(summon_form);
            effect.targetSide = RuleDefinitions.Side.Ally;
            effect.canBePlacedOnCharacter = true;

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>.createPower(name + "Power",
                                                                                                   "",
                                                                                                   title,
                                                                                                   description,
                                                                                                   power_sprite,
                                                                                                   effect,
                                                                                                   RuleDefinitions.ActivationTime.BonusAction,
                                                                                                   2,
                                                                                                   RuleDefinitions.UsesDetermination.Fixed,
                                                                                                   RuleDefinitions.RechargeRate.ShortRest);
            power.linkedPower = DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape;
            return power;
        }


        static void createSpiritSummoner()
        {
            var title = "Feature/&DruidSubclassCircleOfSpiritsSpiritSummonerTitle";
            var description = "Feature/&DruidSubclassCircleOfSpiritsSpiritSummonerDescription";


            var magic_tag = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagIfHasFeature>.createFeature("DruidSubclassCircleOfSpiritsSpiritSummonerMagicAttacks",
                                                                                                                  "",
                                                                                                                  Common.common_no_title,
                                                                                                                  Common.common_no_title,
                                                                                                                  Common.common_no_icon,
                                                                                                                  a =>
                                                                                                                  {
                                                                                                                      a.requiredFeature = null;
                                                                                                                      a.tag = "Magical";
                                                                                                                  }
                                                                                                                  );

            var extra_hp = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseMonsterHitPointsToTargetOnConditionApplication>
                                                                                .createFeature("DruidSubclassCircleOfSpiritsExtraHP",
                                                                                               "",
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.requiredCondition = DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature;
                                                                                                   a.hdMultiplier = 2;
                                                                                               }
                                                                                               );

            var condition = Helpers.ConditionBuilder.createCondition("DruidSubclassCircleOfSpiritsSpiritSummonerCondition",
                                                                    "",
                                                                    title,
                                                                    "Rules/&DruidSubclassCircleOfSpiritsSpiritSummonerConditionDescription",
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance.guiPresentation.spriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed,
                                                                    magic_tag
                                                                    );

            var apply_condition = Helpers.FeatureBuilder<NewFeatureDefinitions.AddExtraConditionToTargetOnConditionApplication>
                                                                                .createFeature("DruidSubclassCircleOfSpiritsApplySpiritSummonerCondition",
                                                                                               "",
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.requiredCondition = DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature;
                                                                                                   a.extraCondition = condition;
                                                                                                   a.durationValue = 1;
                                                                                                   a.durationType = RuleDefinitions.DurationType.Permanent;
                                                                                               }
                                                                                               );

            spirit_summoner = Helpers.FeatureSetBuilder.createFeatureSet("DruidSubclassCircleOfSpiritsSpiritSummonerFeatureSet",
                                                                            "",
                                                                            title,
                                                                            description,
                                                                            false,
                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                            false,
                                                                            apply_condition,
                                                                            extra_hp
                                                                            );
        }


        static void createGuardianSpirits()
        {
            var title = "Feature/&DruidSubclassCircleOfSpiritsGuardianSpiritsTitle";
            var description = "Feature/&DruidSubclassCircleOfSpiritsGuardianSpiritsDescription";

            var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.HealAtTurnEndIfHasConditionBasedOnCasterLevel>
                                                                    .createFeature("DruidSubclassCircleOfSpiritsGuardianSpiritsFeature",
                                                                                   "",
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_icon,
                                                                                   a =>
                                                                                   {
                                                                                       a.casterCondition = inside_spirit_area_condition;
                                                                                       a.allowParentConditions = true;
                                                                                       a.characterClass = DatabaseHelper.CharacterClassDefinitions.Druid;
                                                                                       a.levelHealing = new List<(int, int)>
                                                                                       {
                                                                                           (9, 0),
                                                                                           (11, 5),
                                                                                           (13, 6),
                                                                                           (15, 7),
                                                                                           (17, 8),
                                                                                           (19, 9),
                                                                                           (20, 10)
                                                                                       };
                                                                                   }
                                                                                   );



            var condition = Helpers.ConditionBuilder.createCondition("DruidSubclassCircleOfSpiritsGuardianSpiritsCondition",
                                                                    "",
                                                                    title,
                                                                    "Rules/&DruidSubclassCircleOfSpiritsGuardianSpiritsConditionDescription",
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBarkskin.guiPresentation.spriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed,
                                                                    feature
                                                                    );

            guardian_spirits = Helpers.FeatureBuilder<NewFeatureDefinitions.AddExtraConditionToTargetOnConditionApplication>
                                                                                .createFeature("DruidSubclassCircleOfSpiritsGuardianSpiritsApplyCondition",
                                                                                               "",
                                                                                               title,
                                                                                               description,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.requiredCondition = DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature;
                                                                                                   a.extraCondition = condition;
                                                                                                   a.durationValue = 1;
                                                                                                   a.durationType = RuleDefinitions.DurationType.Permanent;
                                                                                               }
                                                                                               );
        }
    }
}
