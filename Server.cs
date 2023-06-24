using HarmonyLib;

namespace Harder_Reactor_Management
{
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class Server
    {
        static void Postfix()
        {
            Util.Tick();
        }
    }
}
