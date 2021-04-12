﻿using HarmonyLib;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Utilities;

namespace TabletopTweaks.MechanicsChanges {
	public enum ExtraModifierDescriptor : int {
		NaturalArmorBonus = ModifierDescriptor.NaturalArmor,
		NaturalArmorSize = 50,
		NaturalArmorStackable = 51
	}

	public class AdditionalModifierDescriptors {

		[PostPatchInitialize]
		static void Update_ModifiableValueArmorClass_FilterIsArmor(){
			Func<ModifiableValue.Modifier, bool> newFilterIsArmor = delegate (ModifiableValue.Modifier m) {
				ModifierDescriptor modDescriptor = m.ModDescriptor;
				return
					modDescriptor == ModifierDescriptor.Armor ||
					modDescriptor == ModifierDescriptor.ArmorEnhancement ||
					modDescriptor == ModifierDescriptor.ArmorFocus ||
					modDescriptor == (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorBonus ||
					modDescriptor == (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorSize ||
					modDescriptor == (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorStackable ||
					modDescriptor == ModifierDescriptor.NaturalArmorEnhancement;
			};

			var FilterIsArmor = AccessTools.Field(typeof(ModifiableValueArmorClass), "FilterIsArmor");
			FilterIsArmor.SetValue(null, newFilterIsArmor);
		}
		[PostPatchInitialize]
		static void Update_ModifierDescriptorComparer_Instance() {
			ModifierDescriptorComparer.Instance = new ModifierDescriptorComparer();
		}

		[HarmonyPatch(typeof(ModifierDescriptorComparer), MethodType.Constructor)]
		static class ModifierDescriptorComparer_Patch {
			static void Postfix(ModifierDescriptorComparer __instance) {
				Helpers.SetField(__instance, "m_Order", new int[Enum.GetNames(typeof(ModifierDescriptor)).Length + Enum.GetNames(typeof(ExtraModifierDescriptor)).Length]);
				for (int i = 0; i < __instance.m_Order.Length; i++) {
					__instance.m_Order[i] = -1;
				}
				List<ModifierDescriptor> list = new List<ModifierDescriptor>(new ModifierDescriptor[] {
					ModifierDescriptor.None,
					ModifierDescriptor.Racial,
					ModifierDescriptor.Inherent,
					ModifierDescriptor.Trait,
					ModifierDescriptor.Size,
					ModifierDescriptor.Insight,
					ModifierDescriptor.Profane,
					ModifierDescriptor.Sacred,
					ModifierDescriptor.Luck,
					ModifierDescriptor.Circumstance,
					ModifierDescriptor.Enhancement,
					ModifierDescriptor.Morale,
					ModifierDescriptor.Competence,
					ModifierDescriptor.Resistance,
					ModifierDescriptor.Cooking,
					ModifierDescriptor.Feat,
					ModifierDescriptor.FavoredEnemy,
					ModifierDescriptor.UntypedStackable,
					ModifierDescriptor.ConstitutionBonus,
					ModifierDescriptor.Difficulty,
					ModifierDescriptor.Polymorph,
					ModifierDescriptor.Other,
					ModifierDescriptor.DexterityBonus,
					ModifierDescriptor.Deflection,
					ModifierDescriptor.Armor,
					ModifierDescriptor.ArmorEnhancement,
					ModifierDescriptor.ArmorFocus,
					ModifierDescriptor.Shield,
					ModifierDescriptor.ShieldEnhancement,
					ModifierDescriptor.Focus,
					(ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorStackable,
					(ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorBonus,
					(ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorSize,
					ModifierDescriptor.NaturalArmorEnhancement,
					ModifierDescriptor.Dodge,
					ModifierDescriptor.Encumbrance,
					ModifierDescriptor.Penalty,
					ModifierDescriptor.StatDamage,
					ModifierDescriptor.StatDrain,
					ModifierDescriptor.Fatigued,
					ModifierDescriptor.Crippled,
					ModifierDescriptor.FearPenalty,
					ModifierDescriptor.NegativeEnergyPenalty,
					ModifierDescriptor.Helpless,
					ModifierDescriptor.Prone
				});
				int j;
				for (j = 0; j < list.Count; j++) {
					__instance.m_Order[(int)list[j]] = j;
				}
				foreach (ModifierDescriptor modifierDescriptor in EnumUtils.GetValues<ModifierDescriptor>().Concat(EnumUtils.GetValues<ExtraModifierDescriptor>().Select(e => (ModifierDescriptor)e))) {
					if (__instance.m_Order[(int)modifierDescriptor] < 0) {
						j++;
						__instance.m_Order[(int)modifierDescriptor] = j;
						list.Add(modifierDescriptor);
					}
				}
				ModifierDescriptorComparer.SortedValues = list.ToArray();
				for (int i = 0; i < ModifierDescriptorComparer.SortedValues.Length; i++) {
					Main.Log($"SortedValues - {i}:{ModifierDescriptorComparer.SortedValues[i]}");
				}
			}
		}

		[HarmonyPatch(typeof(AbilityModifiersStrings), "GetName", new Type[] { typeof(ModifierDescriptor) })]
		static class AbilityModifierStrings_GetName_Patch {
			static void Postfix(ModifierDescriptor descriptor, ref string __result) {
				switch (descriptor) {
					case (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorBonus:
						__result = "Natrual armor bonus";
						break;
					case (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorSize:
						__result = "Natrual armor size";
						break;
					case (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorStackable:
						__result = "Natrual armor";
						break;
				}
			}
		}

		[HarmonyPatch(typeof(AbilityModifiersStrings), "GetDescription", new Type[] { typeof(ModifierDescriptor) })]
		static class AbilityModifierStrings_GetDescription_Patch {
			static void Postfix(ModifierDescriptor descriptor, ref string __result) {
				switch (descriptor) {
					case (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorBonus:
						__result = "Natrual armor bonus";
						break;
					case (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorSize:
						__result = "Natrual armor size";
						break;
					case (ModifierDescriptor)ExtraModifierDescriptor.NaturalArmorStackable:
						__result = "Natrual armor stackable";
						break;
				}
			}
		}
	}
}