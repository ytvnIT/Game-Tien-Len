using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class playSound
    {
        private string PlayCommand;
        private bool isOpen;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr oCallback);

        public void ClosePlayer()
        {
            PlayCommand = "Close MediaFile";
            mciSendString(PlayCommand, null, 0, IntPtr.Zero);
            isOpen = false;
        }

        public void OpenMediaFile(string strFileName)
        {
            PlayCommand = "Open \"" + strFileName + "\" alias MediaFile";
            mciSendString(PlayCommand, null, 0, IntPtr.Zero);
            isOpen = true;
        }

        public void PlayMediaFile(bool loop)
        {
            if (isOpen)
            {
                PlayCommand = "Play MediaFile";
                if (loop)
                    PlayCommand += " REPEAT";
                mciSendString(PlayCommand, null, 0, IntPtr.Zero);
            }
        }

    }
}
