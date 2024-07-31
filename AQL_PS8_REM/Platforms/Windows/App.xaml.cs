using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;

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
                nativeWindow.Activate();

                // allow Windows to draw a native titlebar which respects IsMaximizable/IsMinimizable
                nativeWindow.ExtendsContentIntoTitleBar = false;

                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                int newWidth = 420;
                int newHeight =780;
                if (appWindow.Presenter is OverlappedPresenter p)
                {
                    p.IsResizable = true;

                    // these only have effect if XAML isn't responsible for drawing the titlebar.
                    p.IsMaximizable = true;
                    p.IsMinimizable = true;
                    
                    p.Maximize();
                    newWidth = Math.Min(Convert.ToInt32(newWidth * DeviceDisplay.Current.MainDisplayInfo.Density), appWindow.ClientSize.Width);
                    newHeight = Math.Min(Convert.ToInt32(newHeight * DeviceDisplay.Current.MainDisplayInfo.Density), appWindow.ClientSize.Height);
                    p.Restore();
                }

                appWindow.Resize(new SizeInt32(newWidth, newHeight));
                appWindow.Move(new PointInt32(0, 0));

            });
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
