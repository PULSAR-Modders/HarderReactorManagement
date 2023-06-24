using PulsarModLoader;

namespace Harder_Reactor_Management
{
    internal class HarderReactorMessage : ModMessage
    {
        public static readonly int versionNumber = 0;

        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            Util.Tick(true);
            if (Util.isHost && Util.engineerControl && sender.sender == PLServer.Instance?.GetCachedFriendlyPlayerOfClass(4)?.GetPhotonPlayer())
            {
                int version = (int)arguments[0];
                if (version == versionNumber)
                {
                    bool active = (bool)arguments[1];
                    Mod.Instance.SetEnabled(active);
                    SendState(PhotonTargets.Others, active);
                    Util.engineerHasMod = true;
                }
                else
                {
                    PulsarModLoader.Utilities.Messaging.Notification("Engineer is using an incompatible version of \"Harder Reactor Management\"");
                }
            }
            else if (!Util.isHost && sender.sender == PhotonNetwork.masterClient)
            {
                int version = (int)arguments[0];
                if (version == versionNumber)
                {
                    if (Util.isEngineer)
                    {
                        //If host is requesting the active state
                        if (arguments.Length == 1)
                        {
                            Util.Tick(true); //Make sure Util.modActive is up to date
                            SendState(PhotonTargets.MasterClient, Util.modActive); //Send the state to host
                            return;
                        }
                    }

                    bool active = (bool)arguments[1];
                    Mod.Instance.SetEnabled(active);
                }
                else
                {
                    PulsarModLoader.Utilities.Messaging.Notification("Host is using an incompatible version of \"Harder Reactor Management\"");
                }
            }
        }

        public static void RequestState()
        {
            PhotonPlayer engineer = PLServer.Instance?.GetCachedFriendlyPlayerOfClass(4)?.GetPhotonPlayer();
            if (engineer != null)
            {
                SendRPC(Mod.Instance.HarmonyIdentifier(), $"{nameof(Harder_Reactor_Management)}.{nameof(HarderReactorMessage)}", engineer, new object[] { versionNumber });
            }
        }

        public static void SendState(PhotonTargets target, bool active)
        {
            SendRPC(Mod.Instance.HarmonyIdentifier(), $"{nameof(Harder_Reactor_Management)}.{nameof(HarderReactorMessage)}", target, new object[] { versionNumber, active });
        }
    }
}
