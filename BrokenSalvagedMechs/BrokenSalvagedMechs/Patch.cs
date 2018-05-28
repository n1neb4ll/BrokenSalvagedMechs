using BattleTech;
using BattleTech.UI;
using Harmony;
using System;

namespace BrokenSalvagedMechs
{

    [HarmonyPatch(typeof(SimGameState), "AddMechPart")]
    public static class SimGameState_AddMechPart
    {

        static bool Prefix(SimGameState __instance)
        {
            return false;
        }

        static void Postfix(SimGameState __instance, string id)
        {
            try
            {
                Settings settings = Helper.LoadSettings();

                int itemCount = __instance.GetItemCount(id, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY);
                int defaultMechPartMax = __instance.Constants.Story.DefaultMechPartMax;
                if (itemCount + 1 >= defaultMechPartMax)
                {
                    for (int i = 0; i < defaultMechPartMax - 1; i++)
                    {
                        ReflectionHelper.InvokePrivateMethode(__instance, "RemoveItemStat", new object[] { id, "MECHPART", false });
                    }
                    MechDef mechDef = new MechDef(__instance.DataManager.MechDefs.Get(id), __instance.GenerateSimGameUID());
                    Random rng = new Random();
                    if (!settings.HeadRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.Head.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.Head.CurrentInternalStructure = Math.Max(1f, mechDef.Head.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.LeftArmRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.LeftArm.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.LeftArm.CurrentInternalStructure = Math.Max(1f, mechDef.LeftArm.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.RightArmRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.RightArm.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.RightArm.CurrentInternalStructure = Math.Max(1f, mechDef.RightArm.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.LeftLegRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.LeftLeg.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.LeftLeg.CurrentInternalStructure = Math.Max(1f, mechDef.LeftLeg.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.RightLegRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.RightLeg.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.RightLeg.CurrentInternalStructure = Math.Max(1f, mechDef.RightLeg.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.CentralTorsoRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.CenterTorso.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.CenterTorso.CurrentInternalStructure = Math.Max(1f, mechDef.CenterTorso.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.RightTorsoRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.RightTorso.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.RightTorso.CurrentInternalStructure = Math.Max(1f, mechDef.RightTorso.CurrentInternalStructure * (float)rng.NextDouble());
                    }
                    if (!settings.LeftTorsoRepaired && (!settings.RepairMechLimbs || rng.NextDouble() > settings.RepairMechLimbsChance))
                    {
                        mechDef.LeftTorso.CurrentInternalStructure = 0f;
                    }
                    else if (settings.RandomDamageOnRepaired)
                    {
                        mechDef.LeftTorso.CurrentInternalStructure = Math.Max(1f, mechDef.LeftTorso.CurrentInternalStructure * (float)rng.NextDouble());
                    }

                    // each component is checked and destroyed if the settings are set that way
                    // if RepairMechComponents is true it will variably make components NonFunctional instead of destroyed
                    // the next effect is that some limbs survive, with some structure, and some weapons/components
                    // the settings control the frequency (default 0.33333 for components and 0.5 for limbs
                    foreach (MechComponentRef mechComponent in mechDef.Inventory)
                    {
                        if (!settings.RepairMechComponents || mechDef.IsLocationDestroyed(mechComponent.MountedLocation))
                            mechComponent.DamageLevel = ComponentDamageLevel.Destroyed;
                        if (settings.RepairMechComponents && !mechDef.IsLocationDestroyed(mechComponent.MountedLocation))
                            if (rng.NextDouble() >= settings.RepairComponentsChance)
                                mechComponent.DamageLevel = ComponentDamageLevel.NonFunctional;
                    }

                    __instance.AddMech(0, mechDef, true, false, true, null);
                    SimGameInterruptManager interrupt = (SimGameInterruptManager)ReflectionHelper.GetPrivateField(__instance, "interruptQueue");
                    interrupt.DisplayIfAvailable();
                    __instance.MessageCenter.PublishMessage(new SimGameMechAddedMessage(mechDef, true));
                }
                else
                {
                    __instance.AddItemStat(id, "MECHPART", false);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
