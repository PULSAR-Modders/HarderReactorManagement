using static UnityEngine.GUILayout;
using PulsarModLoader.CustomGUI;

namespace Harder_Reactor_Management
{
    internal class GUI : ModSettingsMenu
    {
        public override string Name()
        {
            return Mod.Instance.Name;
        }

        public override void Draw()
        {
            if (Util.isHost && Util.isEngineer)
            {
                bool modOn = Toggle(Util.modActive, "Mod Active");

                if (modOn != Util.modActive)
                {
                    Util.modActive = modOn;
                    Config.engineerModActive.Value = Util.modActive;
                    Mod.Instance.SetEnabled(Util.modActive);
                    HarderReactorMessage.SendState(PhotonTargets.Others, Util.modActive);
                }
            }
            else if (Util.isHost) //&& !Util.isEngineer
            {
                bool modOn = Toggle(Util.modActive, "Mod Active");
                bool engControl = Toggle(Util.engineerControl, "Allow Engineer Control");

                if (engControl != Util.engineerControl)
                {
                    Util.engineerControl = engControl;
                    Config.engineerControl.Value = Util.engineerControl;
                    if (Util.engineerControl)
                    {
                        HarderReactorMessage.RequestState();
                    }
                    else
                    {
                        if (Util.modActive != Mod.Instance.IsEnabled())
                        {
                            Mod.Instance.SetEnabled(Util.modActive);
                            HarderReactorMessage.SendState(PhotonTargets.Others, Util.modActive);
                        }
                    }
                }

                if (modOn != Util.modActive)
                {
                    Util.modActive = modOn;
                    Config.hostModActive.Value = Util.modActive;
                    if (!Util.engineerControl || !Util.engineerHasMod)
                    {
                        Mod.Instance.SetEnabled(Util.modActive);
                        HarderReactorMessage.SendState(PhotonTargets.Others, Util.modActive);
                    }
                }
            }
            else if (Util.isEngineer) //&& !Util.isHost
            {
                bool modOn = Toggle(Util.modActive, "Mod Active");

                if (modOn != Util.modActive)
                {
                    Util.modActive = modOn;
                    Config.engineerModActive.Value = Util.modActive;
                    HarderReactorMessage.SendState(PhotonTargets.MasterClient, Util.modActive);
                }
            }
            //else no options
        }
    }
}
