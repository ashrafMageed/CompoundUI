using Capabilities.Host;

namespace CompoundUI.CompositionHost
{
    class Program
    {
        static void Main(string[] args)
        {
            HostService.Use<CompositionHost>();
        }
    }
}
