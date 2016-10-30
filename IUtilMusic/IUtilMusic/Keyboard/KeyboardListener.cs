using System;
using System.Runtime.InteropServices;

namespace IUtilMusic.Keyboard
{

    /// <summary>
    /// Listener for all keyboard events 
    /// </summary>
    public class KeyboardListener
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

        #region Events
        /// <summary>
        /// Send informations of the key that triggered the event
        /// </summary>
        public event KeyboardCustomEvent.KeyboardEventHandler OnKeyDownInformation;
        #endregion

        #region Methods
        #region Private
        /// <summary>
        /// Execute a key down event by using extern user32.dll method and
        /// send some informations about the key
        /// </summary>
        /// <param name="virtuaKey">Virtual code of the key that need to be pressed</param>
        /// <param name="keyInfo">Informations concerning the key</param>
        private void ExecuteKeyDownEvent(byte virtuaKey, int keyType, string keyInfo)
        {
            keybd_event(virtuaKey, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
            if (OnKeyDownInformation != null) OnKeyDownInformation(this, new KeyboardCustomEvent.KeyboardArgs(keyType, keyInfo));
        }
        #endregion

        #region Public
        /// <summary>
        /// Execute the Media Next Track key
        /// </summary>
        public void DoNextTrack()
        {
            ExecuteKeyDownEvent(VK_MEDIA_NEXT_TRACK, 1, "Media next track key");
        }

        /// <summary>
        /// Execute the Media Previous Track key
        /// </summary>
        public void DoPreviousTrack()
        {
            ExecuteKeyDownEvent(VK_MEDIA_PREV_TRACK, 2, "Media previous track key");
        }

        /// <summary>
        /// Execute the Volume Up key
        /// </summary>
        public void DoVolumeUp()
        {
            ExecuteKeyDownEvent(VK_VOLUME_UP, 3, "Media volume up key");
        }

        /// <summary>
        /// Execute the Volume Down key
        /// </summary>
        public void DoVolumeDown()
        {
            ExecuteKeyDownEvent(VK_VOLUME_DOWN, 4, "Media volume down key");
        }

        /// <summary>
        /// Execute the Media play/pause key
        /// </summary>
        public void DoMediaPlayPause()
        {
            ExecuteKeyDownEvent(VK_MEDIA_PLAY_PAUSE, 5, "Media play/pause key");
        }
        #endregion
        #endregion
    }
}
