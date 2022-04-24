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
            float h_AssignedAP = state.H_AssignedAP;
            float ct_AssignedAP = state.CT_AssignedAP;
            float lt_AssignedAP = state.LT_AssignedAP;
            float rt_AssignedAP = state.RT_AssignedAP;
            float la_AssignedAP = state.LA_AssignedAP;
            float ra_AssignedAP = state.RA_AssignedAP;
            float ll_AssignedAP = state.LL_AssignedAP;
            float rl_AssignedAP = state.RL_AssignedAP;
            float assignedPoints = h_AssignedAP + ct_AssignedAP + lt_AssignedAP + rt_AssignedAP + la_AssignedAP + ra_AssignedAP + ll_AssignedAP + rl_AssignedAP;

            logger.Log("canMaxArmor: " + canMaxArmor);
            logger.Log("maxArmorPoints: " + maxArmorPoints);
            logger.Log("availableArmor: " + availableArmor);
            logger.Log("assignedPoints: " + assignedPoints);
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
