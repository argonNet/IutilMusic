using System;
using System.Runtime.InteropServices;

namespace IUtilMusic.Handlers
{
    /// <summary>
    /// Handlers for all keyboard events 
    /// </summary>
    public static class KeyboardHandler
    {
        #region DllImport

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        #endregion

        #region Consts
        /// <summary>
        /// Digit that determines that user has pressed down a key
        /// </summary>
        private const int KEYEVENTF_EXTENTEDKEY = 1;
        /// <summary>
        /// Byte code  Mute/Unmute sound
        /// </summary>
        private const int VK_VOLUME_MUTE = 0xAD;
        /// <summary>
        /// Byte code to volume down
        /// </summary>
        private const int VK_VOLUME_DOWN = 0xAE;
        /// <summary>
        /// Byte code to volume up
        /// </summary>
        private const int VK_VOLUME_UP = 0xAF;
        /// <summary>
        /// Byte code to jump to next track
        /// </summary>
        private const int VK_MEDIA_NEXT_TRACK = 0xB0;
        /// <summary>
        /// Byte code to stop a song
        /// </summary>
        private const int VK_MEDIA_STOP = 0xB2;
        /// <summary>
        /// Byte code to play or pause a song
        /// </summary>
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        /// <summary>
        /// Byte  jump to previous track
        /// </summary>
        private const int VK_MEDIA_PREV_TRACK = 0xB1; 
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Execute a key down event by using extern user32.dll method
        /// </summary>
        /// <param name="virtuaKey">Virtual code of the key that need to be pressed</param>
        private static void ExecuteKeyDownEvent(byte virtuaKey)
        {
            keybd_event(virtuaKey, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
        }
        #endregion

        #region Public
        /// <summary>
        /// Execute the Media Next Track key
        /// </summary>
        public static void DoNextTrack()
        {
            ExecuteKeyDownEvent(VK_MEDIA_NEXT_TRACK);
        }

        /// <summary>
        /// Execute the Media Previous Track key
        /// </summary>
        public static void DoPreviousTrack()
        {
            ExecuteKeyDownEvent(VK_MEDIA_PREV_TRACK);
        }

        /// <summary>
        /// Execute the Volume Up key
        /// </summary>
        public static void DoVolumeUp()
        {
            ExecuteKeyDownEvent(VK_VOLUME_UP);
        }

        /// <summary>
        /// Execute the Volume Down key
        /// </summary>
        public static void DoVolumeDown()
        {
            ExecuteKeyDownEvent(VK_VOLUME_DOWN);
        }

        /// <summary>
        /// Execute the Media play/pause key
        /// </summary>
        public static void DoMediaPlayPause()
        {
            ExecuteKeyDownEvent(VK_MEDIA_PLAY_PAUSE);
        }
        #endregion 
        #endregion
    }
}
