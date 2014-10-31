using Capabilities.Host;

namespace CompoundUI.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            HostService.Use<CompundUiHost>();
        }
    }
}
