using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Urovo
{
    /// <summary>
    /// Provides access to the Urovo Wince 60 serial handheld terminals.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// 802.1X authentication mode 
        /// </summary>
        public enum AuthMode
        {
            /// <summary>
            /// Specifies IEEE 802.11 Open System authentication mode 
            /// </summary>
            Open,

            /// <summary>
            /// Specifies IEEE 802.11 Shared Key authentication mode 
            /// </summary>
            Shared,

            /// <summary>
            /// Specifies WPA version 1 security for infrastructure mode
            /// </summary>
            WPA,

            /// <summary>
            /// Specifies WPA version 1 security (pre shared key) for infrastructure mode 
            /// </summary>
            WPAPSK,

            /// <summary>
            /// Specifies WPA version 1 security for ad hoc mode
            /// </summary>
            WPANone,

            /// <summary>
            /// Specifies WPA version 2 security for infrastructure mode 
            /// </summary>
            WPA2,

            /// <summary>
            /// Specifies WPA version 2 security (pre shared key) for infrastructure mode 
            /// </summary>
            WPA2PSK
        }

        /// <summary>
        /// 802.1X encryption mode 
        /// </summary>
        public enum EncryptMode
        {
            /// <summary>
            /// No WEP encryption 
            /// </summary>
            Disabled,

            /// <summary>
            /// WEP encryption enabled 
            /// </summary>
            WEP,

            /// <summary>
            /// TKIP encryption enabled  
            /// </summary>
            TKIP,

            /// <summary>
            /// AES encryption enabled 
            /// </summary>
            AES
        }

        /// <summary>
        /// 802.1X extensible authentication protocol type 
        /// </summary>
        public enum EapType
        {
            /// <summary>
            /// EAP-TLS authentication 
            /// </summary>
            TLS,

            /// <summary>
            /// PEAP authentication 
            /// </summary>
            PEAP,

            /// <summary>
            /// MD5 authentication 
            /// </summary>
            MD5
        }

        /// <summary>
        /// Encapsulates information for a Wireless Local Area Network (WLAN) access point
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class WlanInfo
        {
            /// <summary>
            /// Specifies a media access control (MAC) address. Each access point has a unique MAC address that is the same as the BSSID. 
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MacAddress;

            /// <summary>
            /// The length of the Ssid member
            /// </summary>
            public uint SsidLength;

            /// <summary>
            /// The SSID
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Ssid;

            /// <summary>
            /// The received signal strength indication (RSSI) in dBm
            /// </summary>
            public int Rssi;
        }

        /// <summary>
        /// This structure stores a list of WlanInfo structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class WlanInfoList
        {
            /// <summary>
            /// Number of items in the list of WlanInfo structures.
            /// </summary>
            public uint count;

            /// <summary>
            /// Point to an array of WlanInfo structures.
            /// </summary>
            public IntPtr pWlanInfo;
        }

        /// <summary>
        /// The type of the device information that is being requested
        /// </summary>
        public enum DeviceInfoType : uint
        {
            /// <summary>
            /// Determines whether a cold boot occurred. The pvParam parameter must point to a int variable that receives 0 if a cold boot occurred, or 1 if a warm boot occured.
            /// </summary>
            BootType = 1,
        }

        /// <summary>
        /// The triggering mode of the scanner.
        /// </summary>
        public enum TriggerMode
        {
            /// <summary>
            /// The laser is always on and decoding.
            /// </summary>
            Normal = 0,

            /// <summary>
            /// The laser is on when user press the "scan" key.
            /// </summary>
            Continuous
        }

        /// <summary>
        /// The type of the sms notification event.
        /// </summary>
        public enum SmsNotifyType : byte
        {
            /// <summary>
            /// Indicates that new message has been received
            /// </summary>
            NewSms = 0x10,

            /// <summary>
            /// Indicates that rssi has been received
            /// </summary>
            Rssi = 0x11,

            /// <summary>
            /// Indicates that the network status has changed
            /// </summary>
            RegistrationState = 0x12
        }

        /// <summary>
        /// This function is used to power on the gsm module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableGsmModule();


        /// <summary>
        /// This function is used to power off the gsm module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisableGsmModule();


        /// <summary>
        /// This function is used to get the power status of the gsm mudule
        /// </summary>
        /// <returns>1 when gsm module is power on, 0 when power off, -1 when unknown</returns>
        [DllImport("Device.dll")]
        public static extern int GetGsmPowerStatus();

        /// <summary>
        /// This function is used to power on the 3G module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Enable3GModule();


        /// <summary>
        /// This function is used to power off the 3G module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Disable3GModule();


        /// <summary>
        /// This function is used to get the power status of the 3G mudule
        /// </summary>
        /// <returns>1 when gsm module is power on, 0 when power off, -1 when unknown</returns>
        [DllImport("Device.dll")]
        public static extern int Get3GPowerStatus();

        /// <summary>
        /// This function is used to request that the gsm module return the received signal strength indication (rssi)
        /// </summary>
        /// <returns>the rssi value. please see the remarks</returns>
        /// <remarks>
        /// <list type="table">
        /// <item>
        /// <term>0</term>
        /// <description>-113 dBm or less</description>
        /// </item>
        /// <item>
        /// <term>1</term>
        /// <description>-111 dBm</description>
        /// </item>
        /// <item>
        /// <term>2..30</term>
        /// <description>-109...-53 dBm</description>
        /// </item>
        /// <item>
        /// <term>31</term>
        /// <description>-51 dBm or greater</description>
        /// </item>
        /// <item>
        /// <term>99</term>
        /// <description>not known or not detectable</description>
        /// </item>
        /// </list>
        /// </remarks>
        [DllImport("Device.dll")]
        public static extern int GetGsmSignalStrength();

        /// <summary>
        /// This function is used to request that the gsm module return the received signal strength indication (rssi)
        /// </summary>
        /// <returns>the rssi value. please see the remarks</returns>
        /// <remarks>
        /// <list type="table">
        /// <item>
        /// <term>0</term>
        /// <description>-113 dBm or less</description>
        /// </item>
        /// <item>
        /// <term>1</term>
        /// <description>-111 dBm</description>
        /// </item>
        /// <item>
        /// <term>2..30</term>
        /// <description>-109...-53 dBm</description>
        /// </item>
        /// <item>
        /// <term>31</term>
        /// <description>-51 dBm or greater</description>
        /// </item>
        /// <item>
        /// <term>99</term>
        /// <description>not known or not detectable</description>
        /// </item>
        /// </list>
        /// </remarks>
        [DllImport("Device.dll")]
        public static extern int Get3GSignalStrength();


        /// <summary>
        /// This function is used to get the IMSI of the sim card.
        /// </summary>
        /// <param name="imsi">the buffer that is filled in with the imsi</param>
        /// <param name="length">Specifies the length, in characters, of the imsi buffer, must be large than 15.</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSimIMSI(StringBuilder imsi, uint length);


        /// <summary>
        /// This function is used to power on the wlan module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWlanModule();


        /// <summary>
        /// This function is used to power off the wlan module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisableWlanModule();


        /// <summary>
        /// This function is used to get the power status of the wlan module
        /// </summary>
        /// <returns>1 when wlan module is power on, 0 when power off, -1 when unknown</returns>
        [DllImport("Device.dll")]
        public static extern int GetWlanPowerStatus();


        /// <summary>
        /// This function is used to request that the wlan driver return the received signal strength indication (RSSI).
        /// </summary>
        /// <returns>the RSSI value in dBm. the normal values for the RSSI value are between -10 and -200</returns>
        [DllImport("Device.dll")]
        public static extern int GetWlanSignalStrength();


        /// <summary>
        /// This function is used to power on the bluetooth module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableBthModule();


        /// <summary>
        /// This function is used to power off the bluetooth module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisableBthModule();

        /// <summary>
        /// get the power status of the Bluetooth module
        /// </summary>
        /// <returns>1 when bluetooth module is power on, 0 when power off, -1 when unknown</returns>
        [DllImport("Device.dll")]
        public static extern int GetBthPowerStatus();

        /// <summary>
        /// This function is used to power on the GPS module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableGpsModule();


        /// <summary>
        /// This function is used to power off the GPS module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisableGpsModule();


        /// <summary>
        /// This function returns the power status of the GPS module
        /// </summary>
        /// <returns>1 when GPS module is power on, 0 when power off, -1 when unknown</returns>
        [DllImport("Device.dll")]
        public static extern int GetGpsPowerStatus();

   
        /// <summary>
        /// This function is used to power on the vibrate module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableVibrateModule();


        /// <summary>
        /// This function is used to power off the vibrate module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisableVibrateModule();


        /// <summary>
        /// This function is used to set the background light value
        /// </summary>
        /// <param name="level">the backlight value ,range is 1~20</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetBackLightLevel(int level);


        /// <summary>
        /// This function is used to get the background light value
        /// </summary>
        /// <returns>Returns the backlight value. between 1-20. -1 indicates failed.</returns>
        [DllImport("Device.dll")]
        public static extern int GetBackLightLevel();


        /// <summary>
        /// This function is used to Check that the device is connected to the gateway
        /// </summary>
        /// <returns>true when the device has connected to the gateway, false when not connected</returns>
        /// <remarks>this function does not distinguish between network type(such as gprs, wireless lan, usb)</remarks>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckNetworkStat();


        /// <summary>
        /// This function is used to establishes a gprs connection
        /// </summary>
        /// <param name="connName">a ras phone book entry name</param>
        /// <param name="timeout">timeout value, in seconds</param>
        /// <param name="errorCode">the Zero indicates success. A nonzero error value, either from the set listed in the RAS header file or ERROR_NOT_ENOUGH_MEMORY, indicates failure.</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConnectGprs([MarshalAs(UnmanagedType.LPWStr)]string connName, uint timeout, out uint errorCode);


        /// <summary>
        /// This function is used to check the gprs connection status
        /// </summary>
        /// <param name="connName">a ras phone book entry name</param>
        /// <returns>true when active, false when disactive</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetGprsStatus([MarshalAs(UnmanagedType.LPWStr)]string connName);


        /// <summary>
        /// This function is used to terminate the gprs connection
        /// </summary>
        /// <param name="connName">a ras phone book entry name</param>
        [DllImport("Device.dll")]
        public static extern void DisConnectGprs([MarshalAs(UnmanagedType.LPWStr)]string connName);


        /// <summary>
        /// This function creates a new phone-book entry for gprs
        /// </summary>
        /// <param name="connName">the string that contains an phone entry name</param>
        /// <param name="apn">the string that contains an acess point name</param>
        /// <param name="phoneNumber">the string that contains a telephone number </param>
        /// <param name="userName">the string that contains the user's user name. This string is used to authenticate the user's access to the remote access server.</param>
        /// <param name="password">the string that contains the user's password. This string is used to authenticate the user's access to the remote access server.</param>
        /// <param name="domain">string that contains the domain on which authentication is to occur. An empty string ("") specifies the domain in which the remote access server is a member. An asterisk specifies the domain stored in the phone book for the entry. </param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateGprsEntry([MarshalAs(UnmanagedType.LPWStr)]string connName, [MarshalAs(UnmanagedType.LPWStr)]string apn, [MarshalAs(UnmanagedType.LPWStr)]string phoneNumber, [MarshalAs(UnmanagedType.LPWStr)]string userName, [MarshalAs(UnmanagedType.LPWStr)]string password, [MarshalAs(UnmanagedType.LPWStr)]string domain);

        /// <summary>
        /// This function creates a new phone-book entry for gprs
        /// </summary>
        /// <param name="connName">the string that contains an phone entry name</param>
        /// <param name="apn">the string that contains an acess point name</param>
        /// <param name="phoneNumber">the string that contains a telephone number </param>
        /// <param name="userName">the string that contains the user's user name. This string is used to authenticate the user's access to the remote access server.</param>
        /// <param name="password">the string that contains the user's password. This string is used to authenticate the user's access to the remote access server.</param>
        /// <param name="domain">string that contains the domain on which authentication is to occur. An empty string ("") specifies the domain in which the remote access server is a member. An asterisk specifies the domain stored in the phone book for the entry. </param>
        /// <param name="cid">PDP Context Identifier, can be 1(default) or 2</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateGprsEntryEx([MarshalAs(UnmanagedType.LPWStr)]string connName, [MarshalAs(UnmanagedType.LPWStr)]string apn, [MarshalAs(UnmanagedType.LPWStr)]string phoneNumber, [MarshalAs(UnmanagedType.LPWStr)]string userName, [MarshalAs(UnmanagedType.LPWStr)]string password, [MarshalAs(UnmanagedType.LPWStr)]string domain, int cid);


        /// <summary>
        /// This function provides detailed information for the Wireless NIC as they are cached in the driver .
        /// </summary>
        /// <param name="currentMac">the media access control (MAC) address of the access point associated with the NIC.</param>
        /// <param name="pAvailableList">The list of the SSIDs detected by the NIC.</param>
        /// <param name="pPreferredList">The list of preferred wireless zero configurations</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryWlanInformation(byte[] currentMac, WlanInfoList pAvailableList, WlanInfoList pPreferredList);


        /// <summary>
        /// This function is used to free the memory that was allocated by a call to QueryWlanInformation.
        /// </summary>
        [DllImport("Device.dll")]
        public static extern void FreeWlanInformation();


        /// <summary>
        /// This function adds a new wirelesslan config entry to the "preferred network list"
        /// </summary>
        /// <param name="szSSID">the name of wireless network to connect</param>
        /// <param name="authMode">802.1x authentication mode</param>
        /// <param name="encryptMode">802.1x encryption  mode</param>
        /// <param name="szKey">
        /// for WEP-key, use 'key-index/key-value' format<br/>
        /// 'key-index' is WEP key index(1-4), 'key-value' is WEP key value (40-bit or 104-bit).<br/>
        /// 40-bit is either '10-digit hex numbers' (ex: "0x1234567890") or '5-char ASCII string' (ex: "zxcvb")<br/>
        /// 104-bit is either '26-digit hex numbers' (ex: "0x12345678901234567890123") or '13-char ASCII string' (ex: "abcdefghijklm")<br/>
        /// for TKIP-key, use 'key-value' format. (no key index)<br/>
        /// TKIP-key can be 8-63 char ASCII string (ex: "asdfghjk")
        /// </param>
        /// <param name="eapType">802.1x EAP extension type. both AP and STA will get keys automatically after the successful EAP.</param>
        /// <param name="bAdhoc">indicates whether or not connecting to adhoc net</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddToWlanPreferredList([MarshalAs(UnmanagedType.LPWStr)]string szSSID, AuthMode authMode, EncryptMode encryptMode, [MarshalAs(UnmanagedType.LPWStr)]string szKey, EapType eapType, [MarshalAs(UnmanagedType.Bool)]bool bAdhoc);


        /// <summary>
        /// This function clear the "preferred networks list".
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        /// <remarks>After a succ call, wireless card will disconnect if it was connected</remarks>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ResetWlanPreferredList();


        /// <summary>
        /// This function forces the wireless card to reconnect "preferred networks list"
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RefreshWlanPreferredList();


        /// <summary>
        /// This function is used to get the universally unique identifier of the device. the uuid is the 14 characters string.
        /// </summary>
        /// <param name="deviceId">the buffer that is filled in with the uuid of the device</param>
        /// <param name="length">Specifies the length, in characters, of the deviceId buffer, must be large than 14.</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDeviceID(StringBuilder deviceId, uint length);


        /// <summary>
        /// This function is used to receive the value of one of the device infomations
        /// </summary>
        /// <param name="deviceInfoType">The type of the device information that is being requested</param>
        /// <param name="pvParam">Depends on the device information being queried. For more information, see the DeviceInfoType enum</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDeviceInfo(DeviceInfoType deviceInfoType, out int pvParam);


        /// <summary>
        /// This function is used to set the mode of the keyboard.
        /// </summary>
        /// <param name="uiMode">0,numpad mode; 1,lowercase mode; 2,uppercase mode</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetKeyboardMode(uint uiMode);


        /// <summary>
        /// This function is used to query the mode of the keyboard.
        /// </summary>
        /// <returns>0 when numpad mode, 1 when lowercase mode, 2 when uppercase mode</returns>
        [DllImport("Device.dll")]
        public static extern int GetKeyboardMode();

        /// <summary>
        /// This function is used to display the system cursor.
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisplayCursor();


        /// <summary>
        /// This function is used to hide the system cursor.
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool HideCursor();




        /// <summary>
        /// 
        /// </summary>
        /// <param name="uKey"></param>
        /// <param name="lpszKeyName"></param>
        /// <returns></returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetScreenLockHotkey(uint uKey, [MarshalAs(UnmanagedType.LPWStr)]string lpszKeyName);

        /// <summary>
        /// This function is used to enable the screen auto-lock.  After call this method, the screen will be locked after a specified period of time expires. ther user can poress Fn+5 to unlock the screen.
        /// </summary>
        /// <param name="idleTime">inteval time, in seconds</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartScreenLock(uint idleTime);   //in seconds

        /// <summary>
        /// This function resets the timer that controls whether or not the screen will auto-lock.
        /// </summary>
        [DllImport("Device.dll")]
        public static extern void ScreenLockTimerReset();


        /// <summary>
        /// This function is used to enable the screen auto-lock.
        /// </summary>
        [DllImport("Device.dll")]
        public static extern void StopScreenLock();


        /// <summary>
        /// This function is used to associate a event handler with the screenlock event.
        /// </summary>
        /// <param name="hLockEvent">an event that was created with CreateEvent. if hLockEvent is null, the association is removed</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetScreenLockEvent(IntPtr hLockEvent);


        /// <summary>
        /// This function is used to check the screen status
        /// </summary>
        /// <returns>true when locked, false when not</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsScreenLocked();


        /// <summary>
        /// This function is used to power on the scanner module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_EnableModule();

        /// <summary>
        /// This function is used to power off the scanner module
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_DisableModule();

        /// <summary>
        /// This function is used to get the power status of the scanner module
        /// </summary>
        /// <returns>1 when power on, 0 when power off, -1 when unknown</returns>
        [DllImport("Device.dll")]
        public static extern int SCA_GetPowerStatus();


        /// <summary>
        /// This function is used to register for scan notification events.
        /// </summary>
        /// <param name="hMsgQ">Handle to the application's message queue created with CreateMsgQueue</param>
        /// <returns>Nonzero indicates success. Zero indicates failure</returns>
        /// <remarks>Scan notification events are issued through message queues.<br/>
        /// The developer can use ReadMsgQueue to read a single message from the message queue.<br/>
        /// The first byte in each message indicates the barcode type. the second byte in each message indicates the length of the barcode.
        /// </remarks>
        [DllImport("Device.dll")]
        public static extern IntPtr SCA_RegisterNotification(IntPtr hMsgQ);


        /// <summary>
        /// This function is used to stop receiving scan notification events. 
        /// </summary>
        /// <param name="hNotify">The handle returned from SCA_RegisterNotification</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_UnRegisterNotification(IntPtr hNotify);


        /// <summary>
        /// This function is used to trigger the laser
        /// </summary>
        /// <param name="state">true for laser on, false for laser off</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_SetTriggerState([MarshalAs(UnmanagedType.Bool)]bool state);


        /// <summary>
        /// This function is used to lock the hardware trigger
        /// </summary>
        /// <returns></returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_LockTrigger();


        /// <summary>
        /// This function is used to unlock the hardware trigger
        /// </summary>
        /// <returns></returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_UnlockTrigger();


        /// <summary>
        /// This function is used to set the trigger mode
        /// </summary>
        /// <param name="mode">the supported trigger mode are listed in the TriggerMode enumeration</param>
        /// <returns>true indicates success, false indicates failure</returns>
        /// <remarks>the setting of the triggermode will be lost when power removed.<br/>
        /// This function is only available on symbol scanner.
        /// </remarks>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_SetTriggerMode(TriggerMode mode);


        /// <summary>
        /// This functions is used to send commands to the scanner.
        /// </summary>
        /// <param name="pParam">a buffer the contain the data of the commands</param>
        /// <param name="dwSize">the size of the buffer, in bytes</param>
        /// <param name="bPermanent">Specifies whether or not the commands is lost when power down</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_SendParam(byte[] pParam, uint dwSize, [MarshalAs(UnmanagedType.Bool)]bool bPermanent);



        /// <summary>
        /// This functions is used to request parameters of the scanner
        /// </summary>
        /// <param name="pParam">a buffer the contain the parameters name </param>
        /// <param name="dwSize">size of the pParam, in bytes</param>
        /// <param name="pParamVal">a buffer that used to receive the parameter value</param>
        /// <param name="dwValSize">size of the pParamVal, in bytes</param>
        /// <returns>true indicates success, false indicates failure</returns>
        /// <remarks>This function is only available on symbol scanner.</remarks>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_RequestParam(byte[] pParam, uint dwSize, byte[] pParamVal, ref uint dwValSize);


        /// <summary>
        /// This function is used to set all parameters to their factory default settings.
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SCA_ResetScannerParams();



        /// <summary>
        /// This function is used to open the sms function of the device
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_Open();


        /// <summary>
        /// This function is used to close the sms function of the device
        /// </summary>
        [DllImport("Device.dll")]
        public static extern void SMS_Close();


        /// <summary>
        /// This function is used to determine whether the sms function is opened.
        /// </summary>
        /// <returns>true indicates the sms function is opened. false indicates not</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_IsOpened();


        /// <summary>
        /// This function is used to register for sms notification events.
        /// </summary>
        /// <param name="hMsgQ">Handle to the application's message queue created with CreateMsgQueue</param>
        /// <returns>Nonzero indicates success. Zero indicates failure</returns>
        /// <remarks>sms notification events are issued through message queues.<br/>
        /// The developer can use ReadMsgQueue to read a single message from the message queue.<br/>
        /// The first byte in each message indicates the notify type. the second byte in each message contain the valid data.
        /// </remarks>
        [DllImport("Device.dll")]
        public static extern IntPtr SMS_RegisterNotification(IntPtr hMsgQ);


        /// <summary>
        /// This function is used to stop receiving the sms notification events.
        /// </summary>
        /// <param name="hNotify">The handle returned from SMS_UnRegisterNotification</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_UnRegisterNotification(IntPtr hNotify);


        /// <summary>
        /// This function is used to send a SMS message.New applications should call SMS_SendSMSEx instead of this function. 
        /// </summary>
        /// <param name="szRecipient">The phone number of the recipient</param>
        /// <param name="szSmsc">The phone number of the short message service senter, this param can be empty</param>
        /// <param name="szMsg">The content of the short message. it can be up to 160 characters (7 bit coded ) or 140 characters (8 bit coded) or 70 characters(unicode)</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_SendSMS([MarshalAs(UnmanagedType.LPWStr)]string szRecipient, [MarshalAs(UnmanagedType.LPWStr)]string szSmsc, [MarshalAs(UnmanagedType.LPWStr)]string szMsg);


        /// <summary>
        /// This function is used to send a SMS message.
        /// </summary>
        /// <param name="szRecipient">The phone number of the recipient</param>
        /// <param name="szSmsc">The phone number of the short message service senter, this param can be empty</param>
        /// <param name="szMsg">The content of the short message. it can be up to 160 characters (7 bit coded ) or 140 characters (8 bit coded) or 70 characters(unicode)</param>
        /// <param name="wID">The reference number of the enhanced concatenated short message</param>
        /// <param name="byTotalNum">The maximum number of short messages in the enhanced concatenated short message.</param>
        /// <param name="byCurrentNum">The sequence number of the current short message.</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_SendSMSEx([MarshalAs(UnmanagedType.LPWStr)]string szRecipient, [MarshalAs(UnmanagedType.LPWStr)]string szSmsc, [MarshalAs(UnmanagedType.LPWStr)]string szMsg, ushort wID, byte byTotalNum, byte byCurrentNum);


        /// <summary>
        /// This function is used to Read a SMS messages from preferred store.New applications should call SMS_ReadSMSEx instead of this function. 
        /// </summary>
        /// <param name="iIndex">Integer type; value in the range of location numbers supported by the associated memory</param>
        /// <param name="szRecipient">a buffer to receive the phone number of the recipient</param>
        /// <param name="dwRecpLen">the size of the recipient buffer, in characters</param>
        /// <param name="szMsg">a buffer to receive the content of the message</param>
        /// <param name="dwMsgLen">the size of the message content buffer, in characters</param>
        /// <param name="szTime">a buffer to receive the time of the message</param>
        /// <param name="dwTimeLen">the size of the time buffer, in characters</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_ReadSMS(int iIndex, StringBuilder szRecipient, uint dwRecpLen, StringBuilder szMsg, uint dwMsgLen, StringBuilder szTime, uint dwTimeLen);


        /// <summary>
        /// This function is used to Read a SMS messages from preferred store.
        /// </summary>
        /// <param name="iIndex">Integer type; value in the range of location numbers supported by the associated memory</param>
        /// <param name="szRecipient">a buffer to receive the phone number of the recipient</param>
        /// <param name="dwRecpLen">the size of the recipient buffer, in characters</param>
        /// <param name="szMsg">a buffer to receive the content of the message</param>
        /// <param name="dwMsgLen">the size of the message content buffer, in characters</param>
        /// <param name="szTime">a buffer to receive the time of the message</param>
        /// <param name="dwTimeLen">the size of the time buffer, in characters</param>
        /// <param name="pwID">The reference number of the enhanced concatenated short message</param>
        /// <param name="pbyTotalNum">The maximum number of short messages in the enhanced concatenated short message.</param>
        /// <param name="pbyCurrentNum">The sequence number of the current short message.</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_ReadSMSEx(int iIndex, StringBuilder szRecipient, uint dwRecpLen, StringBuilder szMsg, uint dwMsgLen, StringBuilder szTime, uint dwTimeLen, ref ushort pwID, ref byte pbyTotalNum, ref byte pbyCurrentNum);


  
        /// <summary>
        /// This function is used to delete a SMS messages from preferred store.
        /// </summary>
        /// <param name="iIndex">Integer type; value in the range of location numbers supported by the associated memory</param>
        /// <returns>true indicates success, false indicates failure</returns>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_DeleteSMS(int iIndex);



        /// <summary>
        /// This function is used to list SMS messages from preferred store.
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        /// <remarks>sms notification events are issued through message queues.<br/>
        /// The first byte in each message indicates the notify type. the second byte in each message contain the sms index.
        /// </remarks>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_ListSMS();


        /// <summary>
        /// This function is used to request the rssi.
        /// </summary>
        /// <returns>true indicates success, false indicates failure</returns>
        /// <remarks>sms notification events are issued through message queues.<br/>
        /// The first byte in each message indicates the notify type. the second byte in each message contain the rssi.
        /// <list type="table">
        /// <item>
        /// <term>0</term>
        /// <description>-113 dBm or less</description>
        /// </item>
        /// <item>
        /// <term>1</term>
        /// <description>-111 dBm</description>
        /// </item>
        /// <item>
        /// <term>2..30</term>
        /// <description>-109...-53 dBm</description>
        /// </item>
        /// <item>
        /// <term>31</term>
        /// <description>-51 dBm or greater</description>
        /// </item>
        /// <item>
        /// <term>99</term>
        /// <description>not known or not detectable</description>
        /// </item>
        /// </list>
        /// </remarks>
        [DllImport("Device.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SMS_GetSignalStrength();
        

        /// <summary>
        /// This function is used to get the network registration status.
        /// </summary>
        /// <returns>The network registration status ,please see the remarks</returns>
        /// <remarks>
        /// <list type="table">
        /// <item>
        /// <term>0</term>
        /// <description>Not registered, device is currently not searching for new operator.</description>
        /// </item>
        /// <item>
        /// <term>1</term>
        /// <description>Registered to home network</description>
        /// </item>
        /// <item>
        /// <term>2</term>
        /// <description>Not registered, but device is currently searching for a new operator.</description>
        /// </item>
        /// <item>
        /// <term>3</term>
        /// <description>Registration denied</description>
        /// </item>
        /// <item>
        /// <term>4</term>
        /// <description>Unknown</description>
        /// </item>
        /// <item>
        /// <term>5</term>
        /// <description>Registered, roaming</description>
        /// </item>
        /// </list>
        /// </remarks>
        [DllImport("Device.dll")]
        public static extern int SMS_GetRegistrationState();

    }
}