using PulsarModLoader;

namespace Harder_Reactor_Management
{
    internal class Config
    {
        public static SaveValue<bool> hostModActive = new SaveValue<bool>("host mod active", false);
        public static SaveValue<bool> engineerModActive = new SaveValue<bool>("engineer mod active", true);
        public static SaveValue<bool> engineerControl = new SaveValue<bool>("engineer control", true);
    }
}
