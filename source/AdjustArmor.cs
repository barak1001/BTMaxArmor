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
            var state = new ArmorState(mechDef);
            float maxpoints = ArmorUtils.MaxArmorPoints(mechDef);

            var availableAP = state.AvailableArmorPoints;
            var armorWeight = state.ArmorWeight;
            var armorFactor = state.ArmorFactor;
            var maxTonnage = mechDef.Chassis.Tonnage;
            float freeTonnage = state.FreeTonnage;
        }
    }
}
