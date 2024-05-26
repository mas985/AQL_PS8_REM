using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using static AQL_PS8_SKT.SocketProcess;

namespace AQL_PS8_SKT
{
    class SocketProcess
    {
        public enum Keys
        {
            //"""Key events which can be sent to the unit"""
            //# Second word is the same on first down, 0000 every 100ms while holding
            RESET = 0x00000000,
            RIGHT = 0x00000001,
            MENU = 0x00000002,
            LEFT = 0x00000004,
            LRBTN = 0x00000005,
            SERVICE = 0x00000008,
            MINUS = 0x00000010,
            PLUS = 0x00000020,
            POOL_SPA = 0x00000040,
            FILTER = 0x00000080,
            LIGHTS = 0x00000100,
            AUX_1 = 0x00000200,
            AUX_2 = 0x00000400,
            AUX_3 = 0x00000800,
            AUX_4 = 0x00001000,
            AUX_5 = 0x00002000,
            AUX_6 = 0x00004000,
            AUX_7 = 0x00008000,
            VALVE_3 = 0x00010000,
            VALVE_4 = 0x00020000,
            HEATER_1 = 0x00040000,
            AUX_8 = 0x00080000,
            AUX_9 = 0x00100000,
            AUX_10 = 0x00200000,
            AUX_11 = 0x00400000,
            AUX_12 = 0x00800000,
            AUX_13 = 0x01000000,
            AUX_14 = 0x02000000,
        }

        [Flags]
        public enum States
        {
            HEATER_1 = 1 << 0,
            VALVE_3 = 1 << 1,
            CHECK_SYSTEM = 1 << 2,
            POOL = 1 << 3,
            SPA = 1 << 4,
            FILTER = 1 << 5,
            LIGHTS = 1 << 6,
            AUX_1 = 1 << 7,
            AUX_2 = 1 << 8,
            SERVICE = 1 << 9,
            AUX_3 = 1 << 10,
            AUX_4 = 1 << 11,
            AUX_5 = 1 << 12,
            AUX_6 = 1 << 13,
            VALVE_4 = 1 << 14,
            SPILLOVER = 1 << 15,
            SYSTEM_OFF = 1 << 16,
            AUX_7 = 1 << 17,
            AUX_8 = 1 << 18,
            AUX_9 = 1 << 19,
            AUX_10 = 1 << 20,
            AUX_11 = 1 << 21,
            AUX_12 = 1 << 22,
            AUX_13 = 1 << 23,
            AUX_14 = 1 << 24,
            SUPER_CHLORINATE = 1 << 25,
            HEATER_AUTO_MODE = 1 << 30, // This is a kludge for the heater auto mode
            FILTER_LOW_SPEED = 1 << 31  //This is a kludge for the low-speed filter
        }

        public class SocketData
        {
            public string DisplayText { get; set; }
            public States Status { get; set; }
            public States Blink { get; set; }
            public bool HasData { get; set; }
        }

        public bool Connected
        {
            get { return _tcpClient.Connected; }
        }

        private bool _menu_locked;

        private TcpClient _tcpClient = new();

        private const byte _FRAME_DLE = 0x10;
        private const byte _FRAME_STX = 0x02;
        private const byte _FRAME_ETX = 0x03;

        // private const byte _WIRELESS_KEY_EVENT = 0x83;
        private const byte _WIRED_LOCAL_KEY_EVENT = 0x02;
        //private const byte _WIRED_REMOTE_KEY_EVENT = 0x03;

        //public SocketProcess()
        //{
        //}

        public void Connect(string ipAddr, int portNum)
        {
            try
            {
                if (IPAddress.TryParse(ipAddr, out IPAddress ipAddress) && portNum > 0 && portNum < 65536)
                {
                    _tcpClient.Close();
                    _tcpClient = new()
                    {
                        NoDelay = true,
                        ReceiveTimeout = 5000,
                        SendTimeout = 1000
                    };
                    _tcpClient.Connect(ipAddr.Trim(), portNum);
                 }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Socket Connect: " + e.Message);
            }
        }

        //public void Close()
        //{
        //    _tcpClient.Close();
        //}

        public static Keys GetKey(string key)
        {
            return key switch
            {
                "Service" => Keys.SERVICE,
                "Pool" => Keys.POOL_SPA,
                "Spa" => Keys.POOL_SPA,
                "Spillover" => Keys.POOL_SPA,
                "Filter" => Keys.FILTER,
                "Lights" => Keys.LIGHTS,
                "Heater1" => Keys.HEATER_1,
                "Valve3" => Keys.VALVE_3,
                "Valve4" => Keys.VALVE_4,
                "Aux1" => Keys.AUX_1,
                "Aux2" => Keys.AUX_2,
                "Aux3" => Keys.AUX_3,
                "Aux4" => Keys.AUX_4,
                "Aux5" => Keys.AUX_5,
                "Aux6" => Keys.AUX_6,
                "Aux7" => Keys.AUX_7,
                "Aux8" => Keys.AUX_8,
                "Aux9" => Keys.AUX_9,
                "Aux10" => Keys.AUX_10,
                "Aux11" => Keys.AUX_11,
                "Aux12" => Keys.AUX_12,
                "Aux13" => Keys.AUX_13,
                "Aux14" => Keys.AUX_14,
                "MenuBtn" => Keys.MENU,
                "LeftBtn" => Keys.LEFT,
                "RightBtn" => Keys.RIGHT,
                "LRBtn" => Keys.LRBTN,
                "PlusBtn" => Keys.PLUS,
                "MinusBtn" => Keys.MINUS,
                "Reset" => Keys.RESET,
                _ => 0,
            };
        }
        public bool QueueKey(string key, bool isP4)
        {
            if (_tcpClient.Connected)
            {
                if (_menu_locked && key == "RightBtn")
                {
                    SendKey("LRBtn", isP4);
                    return true;
                }
                else
                {
                    SendKey(key, isP4);
                    return false;
                }
            }
            return false;
        }

        private void SendKey(string key, bool isP4)
        {
            try
            {
                if (_tcpClient.Connected)
                {
                    Keys bKey = GetKey(key);

                    if (!isP4 || (int)bKey < 0xFFFF) {
                        List<byte> queData = [];

                        queData.Add(_FRAME_DLE);
                        queData.Add(_FRAME_STX);
                        queData.Add(0x00);
                        queData.Add(_WIRED_LOCAL_KEY_EVENT);

                        //byte[] aBytes = BitConverter.GetBytes((int)bKey); // Reversed bytes

                        byte[] aBytes;
                        if (isP4)
                        {
                            aBytes = BitConverter.GetBytes((short)bKey);
                        }
                        else
                        {
                            aBytes = BitConverter.GetBytes((int)bKey);
                        }

                        queData.AddRange([.. aBytes]);
                        queData.AddRange([.. aBytes]);

                        short crc = 0;
                        foreach (byte aB in queData) { crc += aB; }
                        queData.AddRange(BitConverter.GetBytes(crc).Reverse().ToArray());

                        for (int i = queData.Count - 1; i > 1; i--)
                        {
                            if (queData[i] == 0x10) { queData.Insert(i + 1, 0x00); }
                        }

                        queData.Add(_FRAME_DLE);
                        queData.Add(_FRAME_ETX);

                        //System.Diagnostics.Debug.WriteLine(string.Format("{0,10}    {1}", key, BitConverter.ToString([.. queData])));

                        // Send key

                        _tcpClient.GetStream().Write([.. queData], 0, queData.Count);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

        }

        private int _airT;
        private int _poolT = -9999;
        private int _spaT = -9999;
        private string _temp = "";
        private string _pTemp = "";
        public SocketData Update(bool logCheck)
        {
            SocketData socketData = new();
            try
            {
                byte[] kaBytes = [0x10, 0x02, 0x01, 0x01, 0x00, 0x14, 0x10, 0x03]; // Keep Alive Sequence
                byte[] frBytes = [0x00, 0xe0, 0x00, 0xe6, 0x18, 0x1e, 0xe0];       // Frame indicator between 2 keep alive
                List<byte> recData = [];
                long nCRC = 0;
                while (_tcpClient.Connected && _tcpClient.Available > 6)
                {
                    byte pByte = 0;
                    byte aByte = 0;
                    recData.Clear();

                    // read segment

                    while (_tcpClient.Available > 0)
                    {
                        pByte = aByte;
                        aByte = (byte)_tcpClient.GetStream().ReadByte();

                        if (aByte != 0x00 || pByte != 0x10)
                        {
                            recData.Add(aByte);
                        }

                        if ((aByte == 0x03 && pByte == 0x10) || (aByte == 0xE0 && pByte == 0x1E))
                        {
                            break;
                        }
                    }
                    byte[] bytes = [.. recData];

//#if WINDOWS
//                    if (logCheck)
//                    {
//                        WriteString("AQL_PS8_CODE.CSV", (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString() + "," + BitConverter.ToString(bytes), false);
//                    }
//#endif
                    // process segment

                    if (bytes.SequenceEqual(frBytes)) //Frame
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", "FRAME:", BitConverter.ToString(bytes)));
                    }
                    else if (bytes.SequenceEqual(kaBytes)) //Keep Alive
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", "KEEP ALIVE:", BitConverter.ToString(bytes)));
                     }
                    else if (bytes.Length > 6)
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("{0}", BitConverter.ToString(bytes)));

                        // Calculate CRC

                        short crc = 0;
                        for (int i = 0; i < bytes.Length - 4; i++)
                        {
                            crc += bytes[i];
                        }
                        int sCRC = (bytes[^4] * 256) + bytes[^3];

                        if (sCRC == crc)
                        {
                            if (bytes[2] == 0x01 && bytes[3] == 0x02) // LEDs
                            {
                                socketData.Status = (States)BitConverter.ToInt32(bytes, 4);
                                socketData.Blink = (States)BitConverter.ToInt32(bytes, 8);
                                socketData.HasData = true;
                            }
                            else if (bytes[2] == 0x01 && bytes[3] == 0x03) // Display
                            {
                                string disp = Byte2string(bytes, 4, bytes.Length - 9); // 4 head + 20 line1 + 20 line2 + 5 trail bytes
                                //System.Diagnostics.Debug.WriteLine(string.Format("{0} :: {1}", BitConverter.ToString(bytes), disp));
                                if (disp.Contains("Air Temp"))
                                {
                                    _airT = GetTemp(disp);
                                    disp = disp.Replace(" Temp ", " Temp\n");
                                    
                                    _pTemp = _temp;
                                    _temp = _airT.ToString() + "," + _poolT.ToString() + "," + _spaT.ToString();
                                    if (_pTemp != _temp && logCheck)
                                    {
                                        WriteString("AQL_PS8_TEMP.CSV", _temp, true);
                                    }
                                }
                                else if (disp.Contains("Pool Temp"))
                                {
                                    _poolT = GetTemp(disp);
                                    disp = disp.Replace(" Temp ", " Temp\n");
                                }
                                else if (disp.Contains("Spa Temp"))
                                {
                                    _spaT = GetTemp(disp);
                                    disp = disp.Replace(" Temp ", " Temp\n");
                                }
                                else if (disp.Contains("Display Light"))
                                {
                                    disp = disp.Replace("Display Light", "Display\nLight");
                                }
                                _menu_locked = disp.Contains("Menu-Locked");

                                socketData.DisplayText = disp;
                                socketData.HasData = true;
                            }
                            else
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", "OTHER:", BitConverter.ToString(bytes)));
                            }
                            nCRC = 0;
                        }
                        else
                        {
                            nCRC++;
                            if (nCRC > 10)
                            {
                                socketData.DisplayText = "CRC Error";
                                socketData.HasData = true;
                            }
                            //System.Diagnostics.Debug.WriteLine(string.Format("{0}   {1}", "CRC Error", BitConverter.ToString(bytes)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                socketData.DisplayText = "Socket Error";
                socketData.HasData = true;
            }
            return socketData;
        }

        private static int GetTemp(string str)
        {
            if (str.Contains("Temp"))
            {
                _ = Int32.TryParse(str.AsSpan(str.Length - 4, 2), out int num);
                return num;
            }
            return 0;
        }
        private static string Byte2string(byte[] bytes, int istr, int slen)
        {
            string tStr = "";
            string bStr = "";
            int isplt = istr + slen / 2;
            for (int i = istr; i < istr + slen; i++)
            {
                if (bytes[i] == 0) { break; }
                string cc = bytes[i] < 128 ? Convert.ToChar(bytes[i]).ToString() : Convert.ToChar(bytes[i] - 128).ToString();

                if (i < isplt)
                {
                    tStr += cc.ToString();
                }
                else
                {
                    if (bytes[i] != 186 && bytes[i - 1] != 186)
                    {
                        if (bytes[i] > 127 && bytes[i - 1] < 128)
                        {
                            bStr += "[";
                        }
                        else if (bytes[i] < 128 && bytes[i - 1] > 127)
                        {
                            bStr += "]";
                        }
                    }
                    bStr += cc.ToString();
                }
            }
            if (bStr.Contains('[') && !bStr.Contains(']')) { bStr += "]"; }
            string str = tStr.Trim() + "\n" + bStr.Trim();
            return str.Replace("  ", " ").Replace("  ", " ").Replace("_", "°").Replace(" :", ":").Replace("[ ", "[").Replace(" ]", "]").Trim();
        }

        private static void WriteString(string name, string str, bool head)
        {
#if WINDOWS
            string fPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), name);
            using StreamWriter file = new(fPath, append: true);
            if (!File.Exists(fPath) && head) // Write header
            {
                file.WriteLine("Time,Air T,Pool T,Spa T");
            }
            file.WriteLine(DateTime.Now.ToString() + "," + str);
#endif
        }

        //public static long PingUART(string ipAddr)
        //{
        //    Ping pingSender = new();
        //    IPAddress ipAddress = IPAddress.Parse(ipAddr);
        //    PingReply reply = pingSender.Send(ipAddress);
        //    if (reply.Status == IPStatus.Success)
        //    {
        //        //System.Diagnostics.Debug.WriteLine("Address: {0}", reply.Address.ToString());
        //        //System.Diagnostics.Debug.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
        //        //System.Diagnostics.Debug.WriteLine("Time to live: {0}", reply.Options.Ttl);
        //        //System.Diagnostics.Debug.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
        //        //System.Diagnostics.Debug.WriteLine("Buffer size: {0}", reply.Buffer.Length);
        //        return reply.RoundtripTime;
        //    }
        //    else
        //    {
        //        return -1;
        //    }
        //}
    }
}
