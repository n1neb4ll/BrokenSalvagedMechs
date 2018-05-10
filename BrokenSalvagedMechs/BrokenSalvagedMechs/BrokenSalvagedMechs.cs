using Harmony;
using System.Reflection;

namespace BrokenSalvagedMechs
{
    public class BrokenSalvagedMechs
    {
        public static void Init() {
            var harmony = HarmonyInstance.Create("de.morphyum.BrokenSalvagedMechs");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
