using BattleTech;
using BattleTech.UI;
using Harmony;
using System;

namespace BrokenSalvagedMechs {

    [HarmonyPatch(typeof(SimGameState), "AddMechPart")]
    public static class SimGameState_AddMechPart {

        static bool Prefix(SimGameState __instance) {
            return false;
        }

        static void Postfix(SimGameState __instance, string id) {
            try {
                Settings settings = Helper.LoadSettings();

                int itemCount = __instance.GetItemCount(id, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY);
                int defaultMechPartMax = __instance.Constants.Story.DefaultMechPartMax;
                if (itemCount + 1 >= defaultMechPartMax) {
                    for (int i = 0; i < defaultMechPartMax - 1; i++) {
                        ReflectionHelper.InvokePrivateMethode(__instance, "RemoveItemStat", new object[] { id, "MECHPART", false });
                    }
                    MechDef mechDef = new MechDef(__instance.DataManager.MechDefs.Get(id), __instance.GenerateSimGameUID());
                    if (!settings.HeadRepaired) {
                        mechDef.Head.CurrentInternalStructure = 0f;
                    }
                    if (!settings.LeftArmRepaired) {
                        mechDef.LeftArm.CurrentInternalStructure = 0f;
                    }
                    if (!settings.RightArmRepaired) {
                        mechDef.RightArm.CurrentInternalStructure = 0f;
                    }
                    if (!settings.LeftLegRepaired) {
                        mechDef.LeftLeg.CurrentInternalStructure = 0f;
                    }
                    if (!settings.RightLegRepaired) {
                        mechDef.RightLeg.CurrentInternalStructure = 0f;
                    }
                    if (!settings.CentralTorsoRepaired) {
                        mechDef.CenterTorso.CurrentInternalStructure = 0f;
                    }
                    if (!settings.RightTorsoRepaired) {
                        mechDef.RightTorso.CurrentInternalStructure = 0f;
                    }
                    if (!settings.LeftTorsoRepaired) {
                        mechDef.LeftTorso.CurrentInternalStructure = 0f;
                    }
                    foreach (MechComponentRef comp in mechDef.Inventory) {
                        if (mechDef.IsLocationDestroyed(comp.MountedLocation) || settings.NoItems) {
                            comp.DamageLevel = ComponentDamageLevel.Destroyed;
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