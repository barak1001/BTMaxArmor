using BattleTech;
using CustomComponents;
using System.Linq;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Misc;
using UnityEngine;

namespace BTMaxArmor
{
    public static class ArmorUtils
    {
        public static float CalculateArmorFactor(this MechDef mechDef)
        {
            if (mechDef?.Inventory == null)
            {
                return 0;
            }

            var armorFactor = mechDef.Inventory
                .Select(r => r.Def?.GetComponent<Weights>())
                .Where(w => w != null)
                .Sum(weights => weights.ArmorFactor - 1);
            armorFactor = armorFactor + 1;
            return armorFactor;
        }
        public static float CalcFreeTonnage(this MechDef mechDef)
        {
            float currentTotalTonnage = StoredTonnage.UnRoundedTonnage;
            float freeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;
            return freeTonnage;
        }
        public static float AdjustedTonPerPoint(this MechDef mechDef)
        {
            float tonPerPoint = UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;
            float armorFactor = CalculateArmorFactor(mechDef);
            float adjustedTonPerPoint = tonPerPoint * armorFactor;
            return adjustedTonPerPoint;
        }
        public static float CalcArmorWeight(this MechDef mechDef)
        {
            float armorPoints = mechDef.MechDefAssignedArmor;
            float tonPerPoint = mechDef.AdjustedTonPerPoint();
            float armorWeight = armorPoints * tonPerPoint;
            return armorWeight;
        }
        public static float CalcMaxArmor(this MechDef mechDef)
        {
            float armorWeight = CalcArmorWeight(mechDef);
            float freeWeight = CalcFreeTonnage(mechDef);
            float adjustedWeight = armorWeight + freeWeight;
            if (adjustedWeight <= 0)
            {
                return 0;
            }
            return adjustedWeight;
        }

        public static bool CanMaxArmor(this MechDef mechDef)
        {
            if (CalcMaxArmor(mechDef) < 1)
            {
                return false;
            }
            return true;
        }
        public static float CalcAvailableAP(this MechDef mechDef)
        {
            var logger = HBS.Logging.Logger.GetLogger("Sysinfo");
            float maxAP = mechDef.MaxArmorPoints();
            float maxArmorWeight = mechDef.CalcMaxArmor();
            float adjustedTPP = mechDef.AdjustedTonPerPoint();
            float availableAP = maxArmorWeight / adjustedTPP;
            availableAP = Mathf.Floor(availableAP);
            logger.Log("maxAP: " + maxAP);
            logger.Log("maxArmorWeight: " + maxArmorWeight);
            logger.Log("adjustedTPP: " + adjustedTPP);

            if(availableAP > maxAP)
            {
                availableAP = maxAP;
            }
            logger.Log("availableAP: " + availableAP);
            logger.Log("");
            return availableAP;
        }
        public static float MaxArmorPoints(this MechDef mechDef)
        {
            var maxPoints = Mod.Settings.HeadPoints +
                            (mechDef.Chassis.CenterTorso.InternalStructure * 2) +
                            (mechDef.Chassis.LeftTorso.InternalStructure * 2) +
                            (mechDef.Chassis.RightTorso.InternalStructure * 2) +
                            (mechDef.Chassis.LeftArm.InternalStructure * 2) +
                            (mechDef.Chassis.RightArm.InternalStructure * 2) +
                            (mechDef.Chassis.LeftLeg.InternalStructure * 2) +
                            (mechDef.Chassis.RightLeg.InternalStructure * 2);
            return maxPoints;
        }
    }

    class ArmorState
    {
        internal static bool CanMaxArmor;
        internal readonly float ArmorWeight;
        internal readonly float ArmorFactor;
        internal readonly float CurrentArmorPoints;
        internal readonly float AvailableArmorPoints;
        internal readonly float TonPerPoint;
        internal readonly float FreeTonnage;
        internal ArmorState(MechDef mechDef)
        {
            CanMaxArmor = mechDef.CanMaxArmor();
            CurrentArmorPoints = mechDef.MechDefAssignedArmor;
            AvailableArmorPoints = mechDef.CalcAvailableAP();
            ArmorFactor = mechDef.CalculateArmorFactor();
            TonPerPoint = mechDef.AdjustedTonPerPoint();
            ArmorWeight = mechDef.CalcArmorWeight();
            FreeTonnage = mechDef.CalcFreeTonnage();
        }
    }
}
