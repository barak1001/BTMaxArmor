﻿using BattleTech.UI;
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
            bool inputChanged = false;
            var hk = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (hk)
            {
                Mod.Settings.HeadPointsUnChanged = !Mod.Settings.HeadPointsUnChanged;
                inputChanged = true;
            }
            ArmorState state = new(__instance.activeMechDef);
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
            if (state.CanMaxArmor)
            {
                float maxArmorPoints = state.MaxArmorPoints;
                float availableArmor = state.AvailableArmorPoints;
                float h_MaxAP = state.H_MaxAP;
                float ct_MaxAP = state.CT_MaxAP;
                float lt_MaxAP = state.LT_MaxAP;
                float rt_MaxAP = state.RT_MaxAP;
                float la_MaxAP = state.LA_MaxAP;
                float ra_MaxAP = state.RA_MaxAP;
                float ll_MaxAP = state.LL_MaxAP;
                float rl_MaxAP = state.RL_MaxAP;
                float h_AssignedAP = state.H_AssignedAP;
                float ct_AssignedAP = state.CT_AssignedAP;
                float lt_AssignedAP = state.LT_AssignedAP;
                float rt_AssignedAP = state.RT_AssignedAP;
                float la_AssignedAP = state.LA_AssignedAP;
                float ra_AssignedAP = state.RA_AssignedAP;
                float ll_AssignedAP = state.LL_AssignedAP;
                float rl_AssignedAP = state.RL_AssignedAP;
                float assignedPoints = h_AssignedAP + ct_AssignedAP + lt_AssignedAP + rt_AssignedAP + la_AssignedAP + ra_AssignedAP + ll_AssignedAP + rl_AssignedAP;
                float remainingPoints = availableArmor - assignedPoints;
                if (assignedPoints < availableArmor)
                {
                    bool pass = false;
                    float h_points = h_MaxAP - h_MaxAP;
                    float ct_points = ct_MaxAP - ct_AssignedAP;
                    float lt_points = lt_MaxAP - lt_AssignedAP;
                    float rt_points = rt_MaxAP - rt_AssignedAP;
                    float la_points = la_MaxAP - la_AssignedAP;
                    float ra_points = ra_MaxAP - ra_AssignedAP;
                    float ll_points = ll_MaxAP - ll_AssignedAP;
                    float rl_points = rl_MaxAP - rl_AssignedAP;
                    float torsoPoints = h_points + ct_points + lt_points + rt_points;
                    float extremityPoints = la_points + ra_points + ll_points + rl_points;
                    while (remainingPoints > 0)
                    {
                        if (torsoPoints > 0)
                        {
                            if (!Mod.Settings.HeadPointsUnChanged)
                            {
                                if (h_points > 0)
                                {
                                    logger.Log("Added Head");
                                    h_AssignedAP++;
                                    torsoPoints--;
                                    remainingPoints--;
                                    if (remainingPoints <= 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (ct_points > 0)
                            {
                                logger.Log("Added CenterTorso");
                                ct_AssignedAP++;
                                torsoPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }
                            if (lt_points > 0)
                            {
                                logger.Log("Added LeftTorso");
                                lt_AssignedAP++;
                                torsoPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }
                            if (rt_points > 0)
                            {
                                logger.Log("Added RightTorso");
                                rt_AssignedAP++;
                                torsoPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }
                        }
                        if (torsoPoints <= 0 && extremityPoints <= 0 && Mod.Settings.HeadPointsUnChanged)
                        {
                            logger.Log("Added Head");
                            h_AssignedAP++;
                            remainingPoints--;
                            if (remainingPoints <= 0)
                            {
                                break;
                            }
                        }
                        if (pass)
                        {
                            if (extremityPoints > 0)
                            {
                                if (la_points > 0)
                                {
                                    logger.Log("Added LeftArm");
                                    ll_AssignedAP++;
                                    extremityPoints--;
                                    remainingPoints--;
                                    if (remainingPoints <= 0)
                                    {
                                        break;
                                    }
                                }

                            }
                            if (extremityPoints > 0)
                            {
                                if (ra_points > 0)
                                {
                                    logger.Log("Added RightArm");
                                    ra_AssignedAP++;
                                    extremityPoints--;
                                    remainingPoints--;
                                    if (remainingPoints <= 0)
                                    {
                                        break;
                                    }
                                }

                            }
                            if (extremityPoints > 0)
                            {
                                if (ll_points > 0)
                                {
                                    logger.Log("Added LeftLeg");
                                    ll_AssignedAP++;
                                    extremityPoints--;
                                    remainingPoints--;
                                    if (remainingPoints <= 0)
                                    {
                                        break;
                                    }
                                }

                            }
                            if (extremityPoints > 0)
                            {
                                if (rl_points > 0)
                                {
                                    logger.Log("Added RightLeg");
                                    rl_AssignedAP++;
                                    extremityPoints--;
                                    remainingPoints--;
                                    if (remainingPoints <= 0)
                                    {
                                        break;
                                    }
                                }

                            }
                        }
                        pass = !pass;
                    }
                }
                float ct_Front = Mathf.Ceil(ct_AssignedAP * Mod.Settings.CenterTorsoRatio);
                float ct_Rear = ct_AssignedAP - ct_Front;
                float lt_Front = Mathf.Ceil(lt_AssignedAP * Mod.Settings.LeftTorsoRatio);
                float lt_Rear = lt_AssignedAP - lt_Front;
                float rt_Front = Mathf.Ceil(rt_AssignedAP * Mod.Settings.RightTorsoRatio);
                float rt_Rear = rt_AssignedAP - rt_Front;
                if (ct_Rear == 0)
                {
                    if (ct_Front > 1)
                    {
                        ct_Front--;
                        ct_Rear++;
                    }
                }
                if (lt_Rear == 0)
                {
                    if (lt_Front > 1)
                    {
                        lt_Front--;
                        lt_Rear++;
                    }
                }
                if (rt_Rear == 0)
                {
                    if (rt_Front > 1)
                    {
                        rt_Front--;
                        rt_Rear++;
                    }
                }
                assignedPoints = h_AssignedAP + ct_AssignedAP + lt_AssignedAP + rt_AssignedAP +la_AssignedAP + ra_AssignedAP + ll_AssignedAP + rl_AssignedAP;
                logger.Log("availableAP: " + availableArmor);
                logger.Log("assignedAP: " + assignedPoints);
                logger.Log("ct_assignedAP: " + ct_AssignedAP);
                logger.Log("lt_assignedAP: " + lt_AssignedAP);
                logger.Log("rt_assignedAP: " + rt_AssignedAP);
                logger.Log("h_assignedAP: " + h_AssignedAP);
                logger.Log("ct_Front: " + ct_Front);
                logger.Log("ct_Rear: " + ct_Rear);
                logger.Log("lt_Front: " + lt_Front);
                logger.Log("lt_Rear: " + lt_Rear);
                logger.Log("rt_Front: " + rt_Front);
                logger.Log("rt_Rear: " + rt_Rear);
                logger.Log("la_assignedAP: " + la_AssignedAP);
                logger.Log("ra_assignedAP: " + ra_AssignedAP);
                logger.Log("ll_assignedAP: " + ll_AssignedAP);
                logger.Log("rl_assignedAP: " + rl_AssignedAP);
                logger.Log("");

                ___mechInfoWidget.RefreshInfo(false);
                __instance.FlagAsModified();
                __instance.ValidateLoadout(false);
            }
            if(inputChanged == true)
            {
                Mod.Settings.HeadPointsUnChanged = !Mod.Settings.HeadPointsUnChanged;
            }
            return false;
        }
    }
}