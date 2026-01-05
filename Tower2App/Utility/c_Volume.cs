
using System;
using System.Runtime.InteropServices;

namespace Edge.Tower2.UI
{
    class c_Volume
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint pdwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        const int WM_APPCOMMAND = 0x319;
        const int APPCOMMAND_VOLUME_MUTE = 0x80000;

        public static int getCurrentVolumeL()
        {
            uint volume;
            waveOutGetVolume(IntPtr.Zero, out volume);
            return (int)(volume & 0xFFFF);
            int right = (int)((volume >> 16) & 0xFFFF);
        }

        public static int getCurrentVolumeR()
        {
            uint volume;
            waveOutGetVolume(IntPtr.Zero, out volume);
            return (int)((volume >> 16) & 0xFFFF);
        }

        private void MuteButton_Click(object sender, EventArgs e)
        {
            //SendMessage(this.Handle, WM_APPCOMMAND, IntPtr.Zero, (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public static void SetVolume(int L ,int R)
        {
            uint volume = (uint)(L + (R << 16));
            waveOutSetVolume(IntPtr.Zero, volume);
        }
    }
}
