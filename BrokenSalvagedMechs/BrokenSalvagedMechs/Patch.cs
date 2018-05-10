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
                int itemCount = __instance.GetItemCount(id, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY);
                int defaultMechPartMax = __instance.Constants.Story.DefaultMechPartMax;
                if (itemCount + 1 >= defaultMechPartMax) {
                    for (int i = 0; i < defaultMechPartMax - 1; i++) {
                        ReflectionHelper.InvokePrivateMethode(__instance, "RemoveItemStat", new object[] { id, "MECHPART", false });
                    }
                    MechDef mechDef = new MechDef(__instance.DataManager.MechDefs.Get(id), __instance.GenerateSimGameUID());
                    mechDef.Head.CurrentInternalStructure = 0f;
                    mechDef.LeftArm.CurrentInternalStructure = 0f;
                    mechDef.RightArm.CurrentInternalStructure = 0f;
                    mechDef.LeftLeg.CurrentInternalStructure = 0f;
                    mechDef.RightLeg.CurrentInternalStructure = 0f;
                    mechDef.CenterTorso.CurrentInternalStructure = 0f;
                    mechDef.RightTorso.CurrentInternalStructure = 0f;
                    mechDef.LeftTorso.CurrentInternalStructure = 0f;
                    __instance.AddMech(0, mechDef, true, false, true, null);
                    SimGameInterruptManager interrupt = (SimGameInterruptManager)ReflectionHelper.GetPrivateField(__instance, "interruptQueue");
                    interrupt.DisplayIfAvailable();
                    __instance.MessageCenter.PublishMessage(new SimGameMechAddedMessage(mechDef, true));
                    return;
                }
                __instance.AddItemStat(id, "MECHPART", false);
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}