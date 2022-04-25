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
        //Calculates the ArmorFactor depending on what items are equipped on the Mech.
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
        //Calculates the free weight available on the Mech.  This depends on MechEngineer having StoredTonnage as a class.
        public static float CalcFreeTonnage(this MechDef mechDef)
        {
            float currentTotalTonnage = StoredTonnage.UnRoundedTonnage;
            float freeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;
            return freeTonnage;
        }
        //Takes TONNAGE_PER_ARMOR_POINT and multiplies it by the ArmorFactor provided by equipped items.
        public static float TonPerPoint(this MechDef mechDef)
        {
            float tonPerPoint = UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;
            float armorFactor = CalculateArmorFactor(mechDef);
            float adjustedTonPerPoint = tonPerPoint * armorFactor;
            return adjustedTonPerPoint;
        }
        //The weight of the armor that is currently equipped on the Mech.
        public static float CalcArmorWeight(this MechDef mechDef)
        {
            float armorPoints = mechDef.MechDefAssignedArmor;
            float tonPerPoint = mechDef.TonPerPoint();
            float armorWeight = armorPoints * tonPerPoint;
            return armorWeight;
        }
        //Armor weight + free tonnage.  This is the amount of possible armor in tons that can be assigned.
        public static float UsableWeight(this MechDef mechDef)
        {
            float weight = CalcArmorWeight(mechDef);
            weight += CalcFreeTonnage(mechDef);
            if (weight <= 0)
            {
                return 0;
            }
            return weight;
        }
        //Max total armor points that the Mech can have based on CBT rules as per MechEngineer.
        public static float MaxArmorPoints(this MechDef mechDef)
        {
            float headValue = mechDef.Chassis.Head.InternalStructure*3;
            float headMax = mechDef.Chassis.Head.MaxArmor;
            if (headValue > headMax)
            {
                headValue = headMax;
            }
            float maxPoints = headValue +
                              mechDef.Chassis.CenterTorso.InternalStructure * 2 +
                              mechDef.Chassis.LeftTorso.InternalStructure * 2 +
                              mechDef.Chassis.RightTorso.InternalStructure * 2 +
                              mechDef.Chassis.LeftArm.InternalStructure * 2 +
                              mechDef.Chassis.RightArm.InternalStructure * 2 +
                              mechDef.Chassis.LeftLeg.InternalStructure * 2 +
                              mechDef.Chassis.RightLeg.InternalStructure * 2;
            return maxPoints;
        }
        //Calculates available armor points based usable weight.
        public static float AvailableAP(this MechDef mechDef)
        {
            float maxAP = mechDef.MaxArmorPoints();
            float availableAP = mechDef.UsableWeight();
            availableAP /= mechDef.TonPerPoint();
            availableAP = Mathf.Floor(availableAP);
            if (availableAP > mechDef.MaxArmorPoints())
            {
                return maxAP;
            }
            return availableAP;
        }
        //Percentage equal to available armor points divided by max armor points.
        public static float ArmorMultiplier(this MechDef mechDef)
        {
            float headPoints = mechDef.Head.AssignedArmor;
            float availablePoints = mechDef.AvailableAP();
            float maxArmor = mechDef.MaxArmorPoints();
            if(Mod.Settings.HeadPointsUnChanged)
            {
                maxArmor -= headPoints;
                availablePoints -= headPoints;
            }
            float multiplier = availablePoints / maxArmor;
            return multiplier;
        }
        //Max AP by location
        public static float CalcMaxAPbyLocation(this MechDef mechDef, LocationLoadoutDef location, LocationDef locationDef)
        {
            float maxAP = locationDef.InternalStructure * 2;
            if (location == mechDef.Head)
            {
                maxAP = locationDef.InternalStructure * 3;
                float maxArmor = mechDef.Chassis.Head.MaxArmor;
                if(maxArmor < maxAP)
                {
                    maxAP = maxArmor;
                }
            }
            return maxAP;
        }
        public static float AssignAPbyLocation(this MechDef mechDef, LocationLoadoutDef location, LocationDef locationDef)
        {
            float maxAP = locationDef.InternalStructure * 2;
            float availableAP = mechDef.CalcMaxAPbyLocation(location, locationDef);
            availableAP *= mechDef.ArmorMultiplier();
            availableAP = Mathf.Floor(availableAP);

            if (location == mechDef.Head)
            {
                maxAP = locationDef.MaxArmor;
                if (Mod.Settings.HeadPointsUnChanged)
                {
                    availableAP = location.AssignedArmor;
                    if(availableAP > maxAP)
                    {
                        availableAP = maxAP;
                    }
                }
            }
            if(availableAP > maxAP)
            {
                availableAP = maxAP;
            }
            return availableAP;
        }

        public static float CurrentArmorPoints(this MechDef mechDef)
        {
            float currentArmorPoints = mechDef.MechDefAssignedArmor;
            return currentArmorPoints;

        }
        public static bool CanMaxArmor(this MechDef mechDef)
        {
            float buffer = 15.0f;
            float adjustedTPP = mechDef.TonPerPoint();
            float headArmor = mechDef.Head.AssignedArmor;
            float minFree = (headArmor + buffer) * adjustedTPP;
            if (UsableWeight(mechDef) < minFree)
            {
                return false;
            }
            return true;
        }
    }

    class ArmorState
    {
        internal readonly bool CanMaxArmor;
        internal readonly float MaxArmorPoints;
        internal readonly float CurrentArmorPoints;
        internal readonly float AvailableArmorPoints;
        internal readonly float H_MaxAP;
        internal readonly float H_AssignedAP;
        internal readonly float CT_MaxAP;
        internal readonly float CT_AssignedAP;
        internal readonly float LT_MaxAP;
        internal readonly float LT_AssignedAP;
        internal readonly float RT_MaxAP;
        internal readonly float RT_AssignedAP;
        internal readonly float LA_MaxAP;
        internal readonly float LA_AssignedAP;
        internal readonly float RA_MaxAP;
        internal readonly float RA_AssignedAP;
        internal readonly float LL_MaxAP;
        internal readonly float LL_AssignedAP;
        internal readonly float RL_MaxAP;
        internal readonly float RL_AssignedAP;
        internal ArmorState(MechDef mechDef)
        {
            CanMaxArmor = mechDef.CanMaxArmor();
            MaxArmorPoints = mechDef.MaxArmorPoints(); 
            CurrentArmorPoints = mechDef.CurrentArmorPoints();
            AvailableArmorPoints = mechDef.AvailableAP();
            H_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.Head, mechDef.Chassis.Head);
            H_AssignedAP = mechDef.AssignAPbyLocation(mechDef.Head, mechDef.Chassis.Head);
            CT_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.CenterTorso, mechDef.Chassis.CenterTorso);
            CT_AssignedAP = mechDef.AssignAPbyLocation(mechDef.CenterTorso, mechDef.Chassis.CenterTorso);
            LT_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.LeftTorso, mechDef.Chassis.LeftTorso);
            LT_AssignedAP = mechDef.AssignAPbyLocation(mechDef.LeftTorso, mechDef.Chassis.LeftTorso);
            RT_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.RightTorso, mechDef.Chassis.RightTorso);
            RT_AssignedAP = mechDef.AssignAPbyLocation(mechDef.RightTorso, mechDef.Chassis.RightTorso);
            LA_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.LeftArm, mechDef.Chassis.LeftArm);
            LA_AssignedAP = mechDef.AssignAPbyLocation(mechDef.LeftArm, mechDef.Chassis.LeftArm);
            RA_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.RightArm, mechDef.Chassis.RightArm);
            RA_AssignedAP = mechDef.AssignAPbyLocation(mechDef.RightArm, mechDef.Chassis.RightArm);
            LL_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.LeftLeg, mechDef.Chassis.LeftLeg);
            LL_AssignedAP = mechDef.AssignAPbyLocation(mechDef.LeftLeg, mechDef.Chassis.LeftLeg);
            RL_MaxAP = mechDef.CalcMaxAPbyLocation(mechDef.RightLeg, mechDef.Chassis.RightLeg);
            RL_AssignedAP = mechDef.AssignAPbyLocation(mechDef.RightLeg, mechDef.Chassis.RightLeg);
        }
    }
}
