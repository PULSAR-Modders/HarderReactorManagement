
namespace Harder_Reactor_Management
{
    internal class Util
    {
        public static bool modActive = false;
        public static bool engineerControl = true;

        public static bool isHost = false;
        public static bool isEngineer = false;
        public static bool engineerExists = false;
        public static bool engineerHasMod = false;

        private static int tick = 600;

        public static void Tick(bool instant = false)
        {
            tick--;
            if (tick < 0 || instant)
            {
                tick = 600;
                CheckUpdateVariables();
            }
        }

        private static void CheckUpdateVariables()
        {
            if (PLNetworkManager.Instance != null && PLServer.Instance != null)
            {
                if (PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer() != null)
                {
                    bool host = PhotonNetwork.isMasterClient;
                    if (host && !isHost)
                    {
                        modActive = Config.hostModActive.Value;
                        engineerControl = Config.engineerControl.Value;
                        Mod.Instance.SetEnabled(modActive);
                    }
                    isHost = host;
                }

                bool engineerExisted = engineerExists;
                engineerExists = PLServer.Instance.GetCachedFriendlyPlayerOfClass(4) != null && !PLServer.Instance.GetCachedFriendlyPlayerOfClass(4).IsBot;

                bool engineer = PLServer.Instance.GetCachedFriendlyPlayerOfClass(4) == PLNetworkManager.Instance.LocalPlayer;
                if (engineer != isEngineer)
                {
                    isEngineer = engineer;
                    if (isEngineer)
                    {
                        modActive = Config.engineerModActive.Value;
                        if (isHost && modActive != Mod.Instance.IsEnabled())
                        {
                            Mod.Instance.SetEnabled(modActive);
                            HarderReactorMessage.SendState(PhotonTargets.Others, modActive);
                        }
                    }
                    else
                    {
                        if (isHost)
                        {
                            modActive = Config.hostModActive.Value;
                            if (modActive != Mod.Instance.IsEnabled())
                            {
                                Mod.Instance.SetEnabled(modActive);
                                HarderReactorMessage.SendState(PhotonTargets.Others, modActive);
                            }
                        }
                    }
                }

                if (isHost && engineerExisted != engineerExists)
                {
                    engineerHasMod = false;
                    if (engineerExists)
                    {
                        HarderReactorMessage.RequestState();
                    }
                    else
                    {
                        if (Mod.Instance.IsEnabled() != modActive)
                        {
                            Mod.Instance.SetEnabled(modActive);
                            HarderReactorMessage.SendState(PhotonTargets.Others, modActive);
                        }
                    }
                }
            }
            else
            {
                isHost = false;
                isEngineer = false;
                engineerExists = false;
                engineerHasMod = false;
                Mod.Instance.Disable();
            }
        }
    }
}
