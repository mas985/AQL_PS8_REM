using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using Windows.Devices.PointOfService;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AQL_PS8_REM.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
                var mauiWindow = handler.VirtualView;
                var nativeWindow = handler.PlatformView;
 
                // allow Windows to draw a native titlebar which respects IsMaximizable/IsMinimizable
                nativeWindow.ExtendsContentIntoTitleBar = false;

                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                if (appWindow.Presenter is OverlappedPresenter p)
                {
                    p.IsResizable = true;

                    // these only have effect if XAML isn't responsible for drawing the titlebar.
                    p.IsMaximizable = true;
                    p.IsMinimizable = true;
                }

                int newWidth = Convert.ToInt32(Math.Min(420 * DeviceDisplay.Current.MainDisplayInfo.Density, DeviceDisplay.Current.MainDisplayInfo.Width));
                int newHeight = Convert.ToInt32(Math.Min(780 * DeviceDisplay.Current.MainDisplayInfo.Density, DeviceDisplay.Current.MainDisplayInfo.Height * 0.90));
                appWindow.Resize(new SizeInt32(newWidth, newHeight));
                appWindow.Move(new PointInt32(0, 0));
 
            });
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
