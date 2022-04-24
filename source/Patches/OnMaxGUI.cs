using BattleTech.UI;
using Harmony;
using UnityEngine;
using System;

namespace BTMaxArmor.Patches
{

    [HarmonyPatch(typeof(MechLabPanel), "OnMaxArmor")]
    static class MechLabPanel_OnMaxArmor_Patch
    {
        static bool Prefix(MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
        {
            var logger = HBS.Logging.Logger.GetLogger("Sysinfo");
            var hk = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            ArmorState state = new(__instance.activeMechDef);
            if (hk)
            {
                if (state.CanMaxArmor)
                {
                    AdjustArmor.MaxArmor(__instance.activeMechDef);
                    ___mechInfoWidget.RefreshInfo(false);
                    __instance.FlagAsModified();
                    __instance.ValidateLoadout(false);
                }
            }
            if (!__instance.Initialized)
            {
                return false;
            }
            if (___dragItem != null)
            {
                return false;
            }
            if (__instance.headWidget.IsDestroyed || __instance.centerTorsoWidget.IsDestroyed || __instance.leftTorsoWidget.IsDestroyed || __instance.rightTorsoWidget.IsDestroyed || __instance.leftArmWidget.IsDestroyed || __instance.rightArmWidget.IsDestroyed || __instance.leftLegWidget.IsDestroyed || __instance.rightLegWidget.IsDestroyed)
            {
                return false;
            }
            return false;
        }
    }
}