using System;
using System.Windows.Forms;  
using System.Runtime.InteropServices;

namespace ShadowEngine
{
    public enum ScreenFormat
    {Normal3X4, WideScreen16X9};

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE1
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;

        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    };

    class User_32
    {
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
    }

   public class Resolution
    {
        int width, height;
        static int screenHeight, screenWidth;

        public static int ScreenWidth
        {
            get { return Resolution.screenWidth; }
        }

        public static int ScreenHeight
        {
            get { return Resolution.screenHeight; }
        }

        public static void SetCurrentRes()
        {
            Screen screen = Screen.PrimaryScreen;
            screenWidth = screen.Bounds.Width;
            screenHeight = screen.Bounds.Height;
        }

        public static ScreenFormat GetScreenFormat()
        {
            if (Math.Abs(screenWidth / (float)screenHeight - 4 / (float)3) < 0.05f)
            {
                return ScreenFormat.Normal3X4;
            }
            return ScreenFormat.WideScreen16X9;  
        }

        public Resolution(int a, int b)
        {
            Screen screen = Screen.PrimaryScreen;
            width = screen.Bounds.Width;
            height = screen.Bounds.Height;
            if (a != width && b != height)
            {
                ChangeResolution(a, b);
            }
        }

        public void ChangeResolution(int a, int b)
        {
            int iWidth = a;
            int iHeight = b;


            DEVMODE1 dm = new DEVMODE1();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (0 != User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm))
            {
                dm.dmPelsWidth = iWidth;
                dm.dmPelsHeight = iHeight;

                int iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_TEST);

                if (iRet == User_32.DISP_CHANGE_FAILED)
                {
                    MessageBox.Show("Unable to process your request");
                    MessageBox.Show("Description: Unable To Process Your Request. Sorry For This Inconvenience.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_UPDATEREGISTRY);

                    switch (iRet)
                    {
                        case User_32.DISP_CHANGE_SUCCESSFUL:
                            {
                                break;
                            }
                        case User_32.DISP_CHANGE_RESTART:
                            {
                                MessageBox.Show("Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        default:
                            {
                                MessageBox.Show("Description: Failed To Change The Resolution.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                    }
                }
            }
        }

        public void RestoreResolution()
        {
            ChangeResolution(width, height);   
        }
    }
}
