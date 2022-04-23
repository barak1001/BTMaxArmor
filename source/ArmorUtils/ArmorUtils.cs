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
            armorFactor++;
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
        public static float UsableWeight(this MechDef mechDef)
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
            float buffer = 12f;
            float adjustedTPP = mechDef.AdjustedTonPerPoint();
            float headArmor = mechDef.Head.AssignedArmor;
            float minFree = (headArmor + buffer) * adjustedTPP;
            if (UsableWeight(mechDef) < minFree)
            {
                return false;
            }
            return true;
        }
        public static float AvailableAP(this MechDef mechDef)
        {
            var logger = HBS.Logging.Logger.GetLogger("Sysinfo");
            float maxAP = mechDef.MaxArmorPoints();
            float usableWeight = mechDef.UsableWeight();
            float adjustedTPP = mechDef.AdjustedTonPerPoint();
            float availableAP = usableWeight / adjustedTPP;
            availableAP = Mathf.Floor(availableAP);

            if(availableAP > maxAP)
            {
                availableAP = maxAP;
            }
            if (Mod.Settings.HeadPointsUnChanged)
            {
                availableAP -= mechDef.Head.AssignedArmor;
            }
            return availableAP;
        }
        public static float MaxArmorPoints(this MechDef mechDef)
        {
            float headValue = mechDef.HeadMax();
            float maxPoints = headValue +
                              mechDef.Chassis.CenterTorso.InternalStructure*2 +
                              mechDef.Chassis.LeftTorso.InternalStructure*2 +
                              mechDef.Chassis.RightTorso.InternalStructure*2 +
                              mechDef.Chassis.LeftArm.InternalStructure*2 +
                              mechDef.Chassis.RightArm.InternalStructure*2 +
                              mechDef.Chassis.LeftLeg.InternalStructure*2 +
                              mechDef.Chassis.RightLeg.InternalStructure*2;
            return maxPoints;
        }
        public static float AdjustedMaxArmor(this MechDef mechDef)
        {
            float adjustedArmor = mechDef.MaxArmorPoints() - mechDef.Head.AssignedArmor;
            return adjustedArmor;
        }
        public static float HeadMax(this MechDef mechDef)
        {
            float headMax = mechDef.Chassis.Head.MaxArmor;
            return headMax;
        }

        public static float LocationGetMax(LocationDef location)
        {
            float armor = location.InternalStructure * 2;
            return armor;
        }
        public static float LocationMult(this MechDef mechDef, LocationLoadoutDef location)
        {
            var logger = HBS.Logging.Logger.GetLogger("Sysinfo");
            float maxArmor = mechDef.MaxArmorPoints();
            float adjustedArmor = mechDef.AdjustedMaxArmor();
            float mult = 0.0f;
            if (Mod.Settings.HeadPointsUnChanged)
            {
                if (location == mechDef.Head)
                {
                    mult = location.AssignedArmor/maxArmor;
                }
                if (location == mechDef.CenterTorso)
                {
                    mult = mechDef.Chassis.CenterTorso.InternalStructure*2 / adjustedArmor;
                }
                if (location == mechDef.LeftTorso)
                {
                    mult = mechDef.Chassis.LeftTorso.InternalStructure * 2 / adjustedArmor;
                }
                if (location == mechDef.RightTorso)
                {
                    mult = mechDef.Chassis.RightTorso.InternalStructure * 2 / adjustedArmor;
                }
                if (location == mechDef.LeftArm)
                {
                    mult = mechDef.Chassis.LeftArm.InternalStructure * 2 / adjustedArmor;
                }
                if (location == mechDef.RightArm)
                {
                    mult = mechDef.Chassis.RightArm.InternalStructure * 2 / adjustedArmor;
                }
                logger.Log(location + ": " + mult);
                return mult;
            }
            if (location == mechDef.Head)
            {
                mult = mechDef.HeadMax()/maxArmor;
            }
            if (location == mechDef.CenterTorso)
            {
                mult = mechDef.Chassis.CenterTorso.InternalStructure * 2 / maxArmor;
            }
            if (location == mechDef.LeftTorso)
            {
                mult = mechDef.Chassis.LeftTorso.InternalStructure * 2 / maxArmor;
            }
            if (location == mechDef.RightTorso)
            {
                mult = mechDef.Chassis.RightTorso.InternalStructure * 2 / maxArmor;
            }
            if (location == mechDef.LeftArm)
            {
                mult = mechDef.Chassis.LeftArm.InternalStructure * 2 / maxArmor;
            }
            if (location == mechDef.RightArm)
            {
                mult = mechDef.Chassis.RightArm.InternalStructure * 2 / maxArmor;
            }
            logger.Log(location + ": " + mult);
            return mult;
        }

    }

    class ArmorState
    {
        internal readonly bool CanMaxArmor;
        internal readonly float ArmorWeight;
        internal readonly float ArmorFactor;
        internal readonly float MaxArmorPoints;
        internal readonly float CurrentArmorPoints;
        internal readonly float AvailableArmorPoints;
        internal readonly float TonPerPoint;
        internal readonly float FreeTonnage;
        internal readonly float UsableWeight;
        internal readonly float H_MaxArmor;
        internal readonly float CT_MaxArmor;
        internal readonly float LT_MaxArmor;
        internal readonly float RT_MaxArmor;
        internal readonly float LA_MaxArmor;
        internal readonly float RA_MaxArmor;
        internal readonly float LL_MaxArmor;
        internal readonly float RL_MaxArmor;
        internal readonly float H_Multiplier;
        internal readonly float CT_Multiplier;
        internal readonly float LT_Multiplier;
        internal readonly float RT_Multiplier;
        internal readonly float LA_Multiplier;
        internal readonly float RA_Multiplier;
        internal ArmorState(MechDef mechDef)
        {
            CanMaxArmor = mechDef.CanMaxArmor();
            MaxArmorPoints = mechDef.MaxArmorPoints();
            CurrentArmorPoints = mechDef.MechDefAssignedArmor;
            AvailableArmorPoints = mechDef.AvailableAP();
            ArmorFactor = mechDef.CalculateArmorFactor();
            TonPerPoint = mechDef.AdjustedTonPerPoint();
            ArmorWeight = mechDef.CalcArmorWeight();
            FreeTonnage = mechDef.CalcFreeTonnage();
            UsableWeight = mechDef.UsableWeight();
            H_MaxArmor = mechDef.HeadMax();
            CT_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.CenterTorso);
            LT_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.LeftTorso);
            RT_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.RightTorso);
            LA_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.LeftArm);
            RA_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.RightArm);
            LL_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.LeftLeg);
            RL_MaxArmor = ArmorUtils.LocationGetMax(mechDef.Chassis.RightLeg);
            H_Multiplier = ArmorUtils.LocationMult(mechDef, mechDef.Head);
            CT_Multiplier = ArmorUtils.LocationMult(mechDef, mechDef.CenterTorso);
            LT_Multiplier = ArmorUtils.LocationMult(mechDef, mechDef.LeftTorso);
            RT_Multiplier = ArmorUtils.LocationMult(mechDef, mechDef.RightTorso);
            LA_Multiplier = ArmorUtils.LocationMult(mechDef, mechDef.LeftArm);
            RA_Multiplier = ArmorUtils.LocationMult(mechDef, mechDef.RightArm);
        }
    }

}
