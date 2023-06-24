using PulsarModLoader;

namespace Harder_Reactor_Management
{
    public class Mod : PulsarMod
    {
        public static Mod Instance { get; private set; }

        public Mod()
        {
            Instance = this;
            enabled = false;
        }

        public void SetEnabled(bool enabled, bool alwaysOuput = false)
        {
            if (alwaysOuput || (IsEnabled() != enabled))
            {
                PulsarModLoader.Utilities.Messaging.Notification($"{Name}: {(enabled ? "ON" : "OFF")}");
            }
            base.enabled = enabled;
        }

        public override string Version => "0.0.2";

        public override string Author => "18107";

        public override string ShortDescription => "Makes the reactor stability decrease if you try using too much power.";

        public override string LongDescription => "Makes the reactor lose stability if the power draw reaches the max power allowed. The position of the max power slider affects the rate of stability loss unless max temperature is reached.";

        public override string Name => "Harder Reactor Management";

        public override string ModID => "harderreactormanagement";

        public override string HarmonyIdentifier()
        {
            return "id107.harderreactormanagement";
        }
    }
}
