using System;

using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Urovo
{
    public class DecodeEventArgs : EventArgs
    {
        private string barcode;
        private byte type;

        public DecodeEventArgs(string barcodeData, byte typeData)
        {
            barcode = barcodeData;
            type = typeData;
        }

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        public byte Type
        {
            get { return type; }
            set { type = value; }
        }

    }

    class Scanner
    {
        public event EventHandler<DecodeEventArgs> DecodeEvent;

        private bool needClose = false;

        private IntPtr[] hEvent = new IntPtr[2];
        private IntPtr hNotify = IntPtr.Zero;

        private Thread scanThread;

        private bool isContinuousMode;
        public bool IsContinuousMode
        {
            get { return isContinuousMode; }
            set { isContinuousMode = value; }
        }

        public bool Start()
        {

            if (Device.SCA_GetPowerStatus() == 0)
            {
                Device.SCA_EnableModule();
                needClose = true;
            }

            Win32.MSGQUEUEOPTIONS options = new Win32.MSGQUEUEOPTIONS();
            options.dwSize = 20;
            options.dwFlags = 2;
            options.dwMaxMessages = 3;
            options.cbMaxMessage = 252;
            options.bReadAccess = true;

            IntPtr hMsgQ = Win32.CreateMsgQueue(null, options);
            if (hMsgQ == IntPtr.Zero)
            {
                return false;
            }

            hNotify = Device.SCA_RegisterNotification(hMsgQ);
            if (hNotify == IntPtr.Zero)
            {
                return false;
            }

            hEvent[0] = Win32.CreateEvent(IntPtr.Zero, false, false, null);
            hEvent[1] = hMsgQ;

            scanThread = new Thread(new ThreadStart(this.ScanThreadPorc));
            scanThread.Start();

            return true;

        }

        public void Stop()
        {
            try
            {
                Win32.EventModify(hEvent[0], Win32.EVENT_SET);
                if (!scanThread.Join(3000))
                {
                    scanThread.Abort();
                }
            }
            catch (Exception ex)
            {
            }
            Win32.CloseHandle(hEvent[0]);

            Device.SCA_UnRegisterNotification(hNotify);
            Win32.CloseHandle(hEvent[1]);

            if (needClose)
            {
                Device.SCA_DisableModule();
            }
            else  if (isContinuousMode)
            {
                Device.SCA_SetTriggerMode(Device.TriggerMode.Normal);
            }

        }

        public bool SwitchTriggerMode()
        {
            if (isContinuousMode)
            {
                if (Device.SCA_SetTriggerMode(Device.TriggerMode.Normal))
                {
                    isContinuousMode = false;
                    return true;
                }
            }
            else
            {
                if (Device.SCA_SetTriggerMode(Device.TriggerMode.Continuous))
                {
                    isContinuousMode = true;
                    return true;
                }

            }

            return false;
        }


        private void ScanThreadPorc()
        {
            while (true)
            {
                uint evt = Win32.WaitForMultipleObjects(2, hEvent, false, Win32.INFINITE);
                switch (evt)
                {
                    case 0://return thread
                        return;
                    case 1://disable network 
                        uint bytesRead;
                        uint flags;

                        byte[] buf = new byte[256];

                        if (Win32.ReadMsgQueue(hEvent[1], buf, 256, out bytesRead, Win32.INFINITE, out flags))
                        {
                             EventHandler<DecodeEventArgs> temp = DecodeEvent;
                             if (temp != null)
                             {
                                 temp(this, new DecodeEventArgs(Encoding.Default.GetString(buf, 2, (int)bytesRead), buf[1]));
                             }
                        }
                        break;
                }
            }
        }

    }
}
