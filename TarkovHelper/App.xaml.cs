using Avalonia;
using Avalonia.Markup.Xaml;

namespace TarkovHelper
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
