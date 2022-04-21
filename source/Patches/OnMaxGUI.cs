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
            var hk = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (hk)
            {
                if (ArmorUtils.CanMaxArmor(__instance.activeMechDef))
                {
                    AdjustArmor.MaxArmor(__instance.activeMechDef);
                    //               __instance.headWidget.StripArmor();
                    //               __instance.centerTorsoWidget.StripArmor();
                    //               __instance.leftTorsoWidget.StripArmor();
                    //               __instance.rightTorsoWidget.StripArmor();
                    //               __instance.leftArmWidget.StripArmor();
                    //               __instance.rightArmWidget.StripArmor();
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