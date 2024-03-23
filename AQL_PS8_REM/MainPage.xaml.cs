using AQL_PS8_SKT;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;

namespace AQL_PS8_REM
{
    public partial class MainPage : TabbedPage
    {
        public static int WINUI_height { get; set; }
        public static int WINUI_width { get; set; }
        public MainPage()
        {
            InitializeComponent();

            LoadSettings();

            InitializeBackgroundWorker();

            App_Version.Text = AppInfo.VersionString;

            BindingContext = this;
        }
        protected void OnAppearing_TabbedPage(object sender, EventArgs e)
        {
#if WINDOWS
            WINUI_width = 428;
            WINUI_height = 780;

            double pageWidth = WINUI_width;
            double pageHeight = WINUI_height;
#else
            double pageWidth = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
            double pageHeight = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;
            LogLabel.Text = "";
            LogCheck.IsVisible = false;
#endif

            if (pageHeight < pageWidth)
            {
                (pageWidth, pageHeight) = (pageHeight, pageWidth);
            }
            double tdWidth = Math.Min(pageWidth - GRID1.Margin.HorizontalThickness, GRID1.MaximumWidthRequest) - TextDisplay.Margin.HorizontalThickness;

            //double tdHeight = Math.Min(tdWidth / 11 * 3, Math.Max(Aux1.FontSize * 3, 
            //    pageHeight * 0.84 - GRID1.Margin.VerticalThickness -
            //    (Aux1.MinimumHeightRequest + Aux1.Margin.VerticalThickness) * 10 -
            //    TextDisplay.Margin.VerticalThickness));

            TextDisplay.HeightRequest = tdWidth / 11 * 3; // tdHeight;
            TextDisplay.FontSize = tdWidth / 11; // Math.Min(tdWidth / 11, tdHeight / 3);

            // Set left/right button height

            LeftBtn.HeightRequest = MenuBtn.MinimumHeightRequest * 3 + MenuBtn.Margin.VerticalThickness * 2;
            RightBtn.HeightRequest = MenuBtn.MinimumHeightRequest * 3 + MenuBtn.Margin.VerticalThickness * 2;
        }
        protected void OnDisappearing_TabbedPage(object sender, EventArgs e)
        {
#if ANDROID
            //Application.Current.Quit(); // Called when changing system fonts which crashes application
#endif
        }
        protected void OnDisappearing_Labels(object sender, EventArgs e)
        {
            ValidateLabels();
            SaveSettings();
        }
        protected void OnDisappearing_Settings(object sender, EventArgs e)
        {
            UpdateIPPort();
            SaveSettings();
        }
        //protected void OnUnfocused_Entry(object sender, EventArgs e)
        //{
        //    Entry entry= (Entry)sender;
        //    if (!entry.StyleClass.Contains("_Edit")) { GetSettings(); }
        //    System.Diagnostics.Debug.WriteLine("Unfocused Event"); // Not Triggered
        //}
        string _key = "";
        protected void Button_Click(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            _key = button.StyleId;
            if (_key == "Reset")
            {
                //TextDisplay.Text = "Remote Device Reset...";
                TabPage.CurrentPage = TabPage.Children[0];
            }
        }
        private async void QSG_Click(object sender, EventArgs args)
        {
            try
            {
                Uri uri = new("https://docs.google.com/document/d/e/2PACX-1vSZmb9Bgn7S6KE8wlt1QiTWcziZFddjOOrk9HZb4LZOsaj4Rq5_vAn2Eb_FxFr8IvB3aYE6TfNOxuuz/pub");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception)
            {
                // An unexpected error occurred. No browser may be installed on the device.
            }
        }
 
        string _ipAddr;
        int _portNum;
        private void UpdateIPPort()
        {
            if (IPAddress.TryParse(IPaddr.Text, out IPAddress ipAddress))
            {
                _ipAddr = ipAddress.ToString();
            }
            else { IPaddr.Text = _ipAddr; }

            if (int.TryParse(PortNum.Text, out int pNum))
            {
                _portNum = pNum;
            }
            else { PortNum.Text = _portNum.ToString(); }

        }
        public void ValidateLabels()
        {
            if (Aux1_Edit.Text.Length == 0) { Aux1_Edit.Text = "Aux1"; }
            if (Aux2_Edit.Text.Length == 0) { Aux2_Edit.Text = "Aux2"; }
            if (Aux3_Edit.Text.Length == 0) { Aux3_Edit.Text = "Aux3"; }
            if (Aux4_Edit.Text.Length == 0) { Aux4_Edit.Text = "Aux4"; }
            if (Aux5_Edit.Text.Length == 0) { Aux5_Edit.Text = "Aux5"; }
            if (Aux6_Edit.Text.Length == 0) { Aux6_Edit.Text = "Aux6"; }
            if (Valve3_Edit.Text.Length == 0) { Valve3_Edit.Text = "Valve3"; }
            if (Valve4_Edit.Text.Length == 0) { Valve4_Edit.Text = "Valve4"; }
        }
        public void LoadSettings()
        {
            Aux1_Edit.Text = Preferences.Get(Aux1_Edit.StyleId, "Aux1");
            Aux2_Edit.Text = Preferences.Get(Aux2_Edit.StyleId, "Aux2");
            Aux3_Edit.Text = Preferences.Get(Aux3_Edit.StyleId, "Aux3");
            Aux4_Edit.Text = Preferences.Get(Aux4_Edit.StyleId, "Aux4");
            Aux5_Edit.Text = Preferences.Get(Aux5_Edit.StyleId, "Aux5");
            Aux6_Edit.Text = Preferences.Get(Aux6_Edit.StyleId, "Aux6");
            Valve3_Edit.Text = Preferences.Get(Valve3_Edit.StyleId, "Valve3");
            Valve4_Edit.Text = Preferences.Get(Valve4_Edit.StyleId, "Valve4");

            IPaddr.Text = Preferences.Get(IPaddr.StyleId, "192.168.0.15");
            PortNum.Text = Preferences.Get(PortNum.StyleId, "8899");

            UpdateIPPort();

        }
        public void SaveSettings()
        {
            Preferences.Set(Aux1_Edit.StyleId, Aux1_Edit.Text);
            Preferences.Set(Aux2_Edit.StyleId, Aux2_Edit.Text);
            Preferences.Set(Aux3_Edit.StyleId, Aux3_Edit.Text);
            Preferences.Set(Aux4_Edit.StyleId, Aux4_Edit.Text);
            Preferences.Set(Aux5_Edit.StyleId, Aux5_Edit.Text);
            Preferences.Set(Aux6_Edit.StyleId, Aux6_Edit.Text);
            Preferences.Set(Valve3_Edit.StyleId, Valve3_Edit.Text);
            Preferences.Set(Valve4_Edit.StyleId, Valve4_Edit.Text);

            Preferences.Set(IPaddr.StyleId, IPaddr.Text);
            Preferences.Set(PortNum.StyleId, PortNum.Text);
        }

        // UI Update

          private void UpdateDisplay(SocketProcess.SocketData socketData)
        {
            try
            {
                if (socketData.DisplayText != null)
                {
                    TextDisplay.Text = socketData.DisplayText;
                    //TextDisplay.Text = "Wednesday [12]:45[A]";
                }

                if (socketData.Status != 0)
                {
                    SetStatus(Pool, socketData.Status, socketData.Blink, SocketProcess.States.POOL);
                    SetStatus(Spa, socketData.Status, socketData.Blink, SocketProcess.States.SPA);
                    SetStatus(Spillover, socketData.Status, socketData.Blink, SocketProcess.States.SPILLOVER);
                    SetStatus(Filter, socketData.Status, socketData.Blink, SocketProcess.States.FILTER);
                    SetStatus(Lights, socketData.Status, socketData.Blink, SocketProcess.States.LIGHTS);
                    SetStatus(Heater1, socketData.Status, socketData.Blink, SocketProcess.States.HEATER_1);
                    SetStatus(Valve3, socketData.Status, socketData.Blink, SocketProcess.States.VALVE_3);
                    SetStatus(Valve4, socketData.Status, socketData.Blink, SocketProcess.States.VALVE_4);
                    SetStatus(Aux1, socketData.Status, socketData.Blink, SocketProcess.States.AUX_1);
                    SetStatus(Aux2, socketData.Status, socketData.Blink, SocketProcess.States.AUX_2);
                    SetStatus(Aux3, socketData.Status, socketData.Blink, SocketProcess.States.AUX_3);
                    SetStatus(Aux4, socketData.Status, socketData.Blink, SocketProcess.States.AUX_4);
                    SetStatus(Aux5, socketData.Status, socketData.Blink, SocketProcess.States.AUX_5);
                    SetStatus(Aux6, socketData.Status, socketData.Blink, SocketProcess.States.AUX_6);
                    SetStatus(Service, socketData.Status, socketData.Blink, SocketProcess.States.SERVICE);
                }

#if WINDOWS
                if (LogCheck.IsChecked && socketData.LogText != null)
                {
                    string fPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AQL_PS8_LOG.csv");
                    if (!File.Exists(fPath)) // Write header
                    {
                        File.WriteAllText(fPath, "Time,Air T,Pool T,Spa T\n");
                    }
                    using StreamWriter file = new(fPath, append: true);
                    file.WriteLine(DateTime.Now.ToString() + "," + socketData.LogText);

                }
#endif

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        private static void SetStatus(Button button, SocketProcess.States status, SocketProcess.States blink, SocketProcess.States state)
        {
            button.FontAttributes = (status.HasFlag(state) ? FontAttributes.Bold : FontAttributes.None) |
                    (blink.HasFlag(state) ? FontAttributes.Italic : FontAttributes.None);
        }

        // BackgroundWorker

        readonly BackgroundWorker _backgroundWorker = new();
        private void InitializeBackgroundWorker()
        {
            if (!_backgroundWorker.IsBusy) 
            { 
                TextDisplay.Text = "Connecting...";

                _backgroundWorker.WorkerReportsProgress = true;
                _backgroundWorker.WorkerSupportsCancellation = true;
                _backgroundWorker.DoWork +=
                    new DoWorkEventHandler(BackgroundWorker_DoWork);
                _backgroundWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(
                    BackgroundWorker_RunWorkerCompleted);
                _backgroundWorker.ProgressChanged +=
                        new ProgressChangedEventHandler(
                    BackgroundWorker_ProgressChanged);
                _backgroundWorker.RunWorkerAsync();
            }
        }
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int nCnt = 0;
            SocketProcess socketProcess = new();
            while (!_backgroundWorker.CancellationPending)
            {
                Thread.Sleep(100);

                if (!socketProcess.Connected || nCnt > 50)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0:HH:mm:ss} {1:HH:mm:ss} {2} {3}", DateTime.Now, nTime, socketProcess.Connected, "Reset Socket"));
                    socketProcess.Connect(_ipAddr, _portNum);
                    nCnt = 0;
                    _key = "";
                }
                else
                {
                    SocketProcess.SocketData socketData = socketProcess.Update();

                    if (socketData.HasData)
                    {
                        _backgroundWorker.ReportProgress(0, socketData);
                        nCnt = 0;
                        //System.Diagnostics.Debug.WriteLine(string.Format("{0:HH:mm:ss} {1}", nTime, "Read Data"));
                    }
                    else
                    {
                        nCnt++;
                    }

                    if (_key != "")
                    {
                        if (socketProcess.QueueKey(_key))
                        {
                            socketData.HasData = true;
                            socketData.DisplayText = "Please Wait...";
                            _backgroundWorker.ReportProgress(0, socketData);
                        }
                        else if (_key == "Reset")
                        {
                            //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", DateTime.Now, "Reset Device"));
                            socketData.HasData = true;
                            socketData.DisplayText = "Remote Device Reset...";
                            _backgroundWorker.ReportProgress(0, socketData);
                        }
                        _key = "";
                    }
                }
            }
        }
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SocketProcess.SocketData socketData = (SocketProcess.SocketData)e.UserState;
            
            if (socketData.HasData)
            {
                UpdateDisplay(socketData);
            }
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
    }
}
