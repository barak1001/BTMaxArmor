using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;

namespace BTMaxArmor
{
    public class AdjustArmor
    {
        public static void MaxArmor(MechDef mechDef)
        {
            var logger = HBS.Logging.Logger.GetLogger("Sysinfo");
            ArmorState state = new(mechDef);
            bool canMaxArmor = state.CanMaxArmor;
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
                    if(torsoPoints <= 0 && extremityPoints <= 0 && Mod.Settings.HeadPointsUnChanged)
                    {
                        logger.Log("Added Head");
                        h_AssignedAP++;
                        remainingPoints--;
                        if (remainingPoints <= 0)
                        {
                            break;
                        }
                    }
                    if(pass)
                    {
                        if(extremityPoints > 0)
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
            logger.Log("canMaxArmor: " + canMaxArmor);
            logger.Log("maxArmorPoints: " + maxArmorPoints);
            logger.Log("availableArmor: " + availableArmor);
            logger.Log("assignedPoints: " + assignedPoints);
            float newPoints = h_AssignedAP + ct_AssignedAP + lt_AssignedAP + rt_AssignedAP + la_AssignedAP + ra_AssignedAP + ll_AssignedAP + rl_AssignedAP;
            logger.Log("newPoints: " + newPoints);
            logger.Log("h_AssignedAP: " +  h_AssignedAP);
            logger.Log("ct_AssignedAP: " + ct_AssignedAP);
            logger.Log("lt_AssignedAP: " + lt_AssignedAP);
            logger.Log("rt_AssignedAP: " + rt_AssignedAP);
            logger.Log("la_AssignedAP: " + la_AssignedAP);
            logger.Log("ra_AssignedAP: " + ra_AssignedAP);
            logger.Log("ll_AssignedAP: " + ll_AssignedAP);
            logger.Log("rl_AssignedAP: " + rl_AssignedAP);

            logger.Log(" ");
        }
    }
}
