using HarmonyLib;

namespace Harder_Reactor_Management
{
    [HarmonyPatch(typeof(PLReactor), "ShipUpdate")]
    internal class Reactor
    {
        static void Postfix(PLShipInfoBase inShipInfo, PLReactor shipReactor)
        {
            //ReactorCoolingEnabled is core safety toggle
            if (Mod.Instance.IsEnabled() && PhotonNetwork.isMasterClient && inShipInfo.GetIsPlayerShip() && shipReactor != null && inShipInfo.ReactorCoolingEnabled && inShipInfo.CoreInstability >= 0.05f && !inShipInfo.IsReactorOverheated())
            {
                PLServer.Instance.ServerShipOverheat(inShipInfo.ShipID);
            }
        }
    }
}
