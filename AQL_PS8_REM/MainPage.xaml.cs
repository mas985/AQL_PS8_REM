﻿using AQL_PS8_REM;
using System.ComponentModel;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace AQL_PS8_REM
{
    public partial class MainPage : TabbedPage
    {
         public MainPage()
        {
            InitializeComponent();

            LoadSettings();

            InitializeBackgroundWorker();

            App_Version.Text = AppInfo.VersionString;

            BindingContext = this;
        }
        protected void OnLoaded_TabbedPage(object sender, EventArgs e)
        {
            FormatTextDisplay();
        }
        protected void OnDisappearing_Labels(object sender, EventArgs e)
        {
            ValidateLabels();
            SaveSettings();
        }
        protected void OnDisappearing_Setup(object sender, EventArgs e)
        {
            UpdateIPPort();
            SaveSettings();
        }
        string _key = "";
        protected void Button_Click(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            _key = button.StyleId;
            if (_key == "Reset")
            {
                 TabPage.CurrentPage = TabPage.Children[0];
            }
        }
        private async void Info_Click(object sender, EventArgs args)
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
        private void FormatTextDisplay()
        {
            TextDisplay.FontAutoScalingEnabled = false;
#if WINDOWS
            TextDisplay.FontSize = 34;

#else
            LogLabel.Text = "";
            LogLabel.MinimumHeightRequest = 0;
            LogLabel.HeightRequest = 0;

            LogCheck.IsEnabled = false;
            LogCheck.IsVisible = false;

            double dispWidth = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
            double dispHeight = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;

            if (dispHeight < dispWidth)
            {
                (dispWidth, dispHeight) = (dispHeight, dispWidth);
            }
            double tdWidth = Math.Min(dispWidth - GRID1.Margin.HorizontalThickness, GRID1.MaximumWidthRequest) - TextDisplay.Margin.HorizontalThickness;
            TextDisplay.FontSize = tdWidth / 11; // 22 characters max line, 2:1 H:W ave ratio
#endif
            TextDisplay.MinimumHeightRequest = TextDisplay.FontSize * 3;
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
            P4Mode.IsChecked = Preferences.Get(P4Mode.StyleId, false);

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
            Preferences.Set(P4Mode.StyleId, P4Mode.IsChecked);
        }

        private int _pmin = -999;

        // UI Update
        private void UpdateDisplay(SocketProcess.SocketData socketData)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1} {2} {3}", Aux1.FontSize, Aux1.Height, TextDisplay.FontSize, TextDisplay.Height));;
                if (socketData.DisplayText != null)
                {
                    string disp = socketData.DisplayText;
                    //TextDisplay.Text = "Aux4 Group\nSuperChlr:[Unaffected]";

                    if (disp.Contains("Air Temp"))
                    {
                        disp = disp.Replace(" Temp ", " Temp\n");
                        AirTemp.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Pool Temp"))
                    {
                        disp = disp.Replace(" Temp ", " Temp\n");
                        PoolTemp.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Spa Temp"))
                    {
                        disp = disp.Replace(" Temp ", " Temp\n");
                        SpaTemp.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Salt Level"))
                    {
                        SaltLevel.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Pool Chlorinator"))
                    {
                        PoolChlor.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Spa Chlorinator"))
                    {
                        SpaChlor.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Filter Speed"))
                    {
                        FilterSpeed.Text = disp.Split('\n').Last();
                    }
                    else if (disp.Contains("Display Light"))
                    {
                        disp = disp.Replace("Display Light", "Display\nLight");
                    }
                    PumpWatts.Text = SocketProcess.PumpWatts;
                    TextDisplay.Text = disp;
                }

                // Write Windows Log Data

                if (LogCheck.IsChecked)
                {
                    int min = DateTime.Now.Minute;
                    if (min != _pmin)
                    {
                        _pmin = min;
                        WriteString("AQL_PS8_TEMP.CSV",AirTemp.Text.Split('°').First() + "," + PoolTemp.Text.Split('°').First() + "," + SpaTemp.Text.Split('°').First(), true);
                    }
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
                TextDisplay.Text = "Connection Setup\n- Please Wait -";

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
                        if (socketProcess.QueueKey(_key, P4Mode.IsChecked))
                        {
                            socketData.HasData = true;
                            socketData.DisplayText = "Unlocking Menu\n- Please Wait -";
                            _backgroundWorker.ReportProgress(0, socketData);
                        }
                        else if (_key == "Reset")
                        {
                            //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", DateTime.Now, "Reset Device"));
                            socketData.HasData = true;
                            socketData.DisplayText = "Connection Reset\n- Please Wait -";
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

        private static void WriteString(string name, string str, bool head)
        {
#if WINDOWS
            string fPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), name);
            if (!File.Exists(fPath) && head) // Write header
            {
                File.WriteAllText(fPath, "Time,Air T,Pool T,Spa T\n");
            }
            using StreamWriter file = new(fPath, append: true);
            file.WriteLine(DateTime.Now.ToString() + "," + str);
#endif
        }

    }
}
