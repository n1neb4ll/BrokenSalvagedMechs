﻿using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdjustedMechAssembly {

    [HarmonyPatch(typeof(SimGameState), "AddMechPart")]
    public static class SimGameState_AddMechPart_Patch {

        static bool Prefix(SimGameState __instance) {
            return false;
        }

        static void Postfix(SimGameState __instance, string id) {
            try {
                Settings settings = Helper.LoadSettings();

                Dictionary<MechDef, int> possibleMechs = new Dictionary<MechDef, int>();
                int itemCount = 0;
                if (settings.AssembleVariants) {
                    foreach (KeyValuePair<string, MechDef> pair in __instance.DataManager.MechDefs) {
                        if (pair.Value.Chassis.PrefabBase.Equals(__instance.DataManager.MechDefs.Get(id).Chassis.PrefabBase)) {           
                            int numberOfParts = __instance.GetItemCount(pair.Value.Description.Id, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY);
                            if(numberOfParts > 0) {
                                itemCount += numberOfParts;
                                possibleMechs.Add(new MechDef(pair.Value, __instance.GenerateSimGameUID()), numberOfParts);
                            }
                        }
                    }
                }
                else {
                    itemCount = __instance.GetItemCount(id, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY);
                }
                int defaultMechPartMax = __instance.Constants.Story.DefaultMechPartMax;
                if (itemCount + 1 >= defaultMechPartMax) {
                    MechDef mechDef = null;
                    if (settings.AssembleVariants) {
                        Random rand = new Random();
                        bool found = false;
                        foreach(KeyValuePair<MechDef, int> mech in possibleMechs) {
                            if(rand.NextDouble() <= mech.Value) {
                                mechDef = mech.Key;
                                found = true;
                                break;
                            }
                        }
                        List<KeyValuePair<MechDef, int>> mechlist = possibleMechs.ToList();
                        mechlist = possibleMechs.OrderByDescending(o => o.Value).ToList();
                        if (!found) {                         
                            mechDef = mechlist[0].Key;
                        }
                        int j = 0;
                        int i = 0;
                        int currentPart = 1;
                        while (i < defaultMechPartMax -1) {
                            if(currentPart > mechlist[j].Value) {
                                j++;
                                currentPart = 1;
                            }
                            ReflectionHelper.InvokePrivateMethode(__instance, "RemoveItemStat", new object[] { mechlist[j].Key.Description.Id, "MECHPART", false });
                            currentPart++;
                            i++;
                        }
                    }
                    else {
                        for (int i = 0; i < defaultMechPartMax - 1; i++) {
                            ReflectionHelper.InvokePrivateMethode(__instance, "RemoveItemStat", new object[] { id, "MECHPART", false });
                        }
                        mechDef = new MechDef(__instance.DataManager.MechDefs.Get(id), __instance.GenerateSimGameUID());
                    }
                    Random rng = new Random();
                    if (!settings.HeadRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.Head.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.Head.CurrentInternalStructure = Math.Max(1f, mechDef.Head.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.LeftArmRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.LeftArm.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.LeftArm.CurrentInternalStructure = Math.Max(1f, mechDef.LeftArm.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.RightArmRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.RightArm.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.RightArm.CurrentInternalStructure = Math.Max(1f, mechDef.RightArm.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.LeftLegRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.LeftLeg.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.LeftLeg.CurrentInternalStructure = Math.Max(1f, mechDef.LeftLeg.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.RightLegRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.RightLeg.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.RightLeg.CurrentInternalStructure = Math.Max(1f, mechDef.RightLeg.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.CentralTorsoRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.CenterTorso.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.CenterTorso.CurrentInternalStructure = Math.Max(1f, mechDef.CenterTorso.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.RightTorsoRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.RightTorso.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.RightTorso.CurrentInternalStructure = Math.Max(1f, mechDef.RightTorso.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.LeftTorsoRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance)) {
                        mechDef.LeftTorso.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomStructureOnRepairedLimbs) {
                        mechDef.LeftTorso.CurrentInternalStructure = Math.Max(1f, mechDef.LeftTorso.CurrentInternalStructure * (float)rng.NextDouble());
                    }


                    // each component is checked and destroyed if the settings are set that way
                    // if RepairMechComponents is true it will variably make components either functional, nonfunctional, or destroyed based on settings
                    foreach (MechComponentRef mechComponent in mechDef.Inventory) {
                        if (!settings.RepairMechComponents || mechDef.IsLocationDestroyed(mechComponent.MountedLocation)) {
                            mechComponent.DamageLevel = ComponentDamageLevel.Destroyed;
                            continue;
                        }
                        if (settings.RepairMechComponents) {
                            double repairRoll = rng.NextDouble();
                            if (repairRoll <= settings.RepairComponentsFunctionalThreshold)
                                mechComponent.DamageLevel = ComponentDamageLevel.Functional;
                            else if (repairRoll <= settings.RepairComponentsNonFunctionalThreshold)
                                mechComponent.DamageLevel = ComponentDamageLevel.NonFunctional;
                            else mechComponent.DamageLevel = ComponentDamageLevel.Destroyed;
                        }
                    }

                    __instance.AddMech(0, mechDef, true, false, true, null);
                    SimGameInterruptManager interrupt = (SimGameInterruptManager)ReflectionHelper.GetPrivateField(__instance, "interruptQueue");
                    interrupt.DisplayIfAvailable();
                    __instance.MessageCenter.PublishMessage(new SimGameMechAddedMessage(mechDef, true));
                }
                else {
                    __instance.AddItemStat(id, "MECHPART", false);
                }
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}
