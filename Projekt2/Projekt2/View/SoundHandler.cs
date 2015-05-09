using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace View
{
    /// <summary>
    ///  Class for musik and sound playback
    /// </summary>
    class SoundHandler
    {
        private View.InputHandler _inputHandler;
        private SoundEffect[] _soundEffects;
        private Song[] _soundTracks;
        private Track _activeSong;

        /// <summary>
        /// Enum for indexing of soundeffects
        /// </summary>
        public enum Effect
        {
            MENU_BUTTON_HOVER = 0,
            MENU_BUTTON_SELECT = 1,
            MENU_BUTTON_SELECT_B = 2
        }

        /// <summary>
        /// Enum for indexing of sound tracks
        /// </summary>
        public enum Track
        {
            THEME = 0,
            WORLD = 1,
            DUNGEON = 2,
            TOWN = 3
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inputHandler">Instance of InputHandler</param>
        public SoundHandler(View.InputHandler inputHandler)
        {
            this._inputHandler = inputHandler;
        }

        /// <summary>
        /// Loads all soundeffects and soundtracks
        /// </summary>
        /// <param name="content">Instance of ContentManager</param>
        internal void LoadContent(ContentManager content)
        {
            _soundTracks = new Song[4] {content.Load<Song>("Sound/Music/HeroicDemise"),
                                         content.Load<Song>("Sound/Music/Soliloquy"),
                                         content.Load<Song>("Sound/Music/DesertTrauma"),
                                         content.Load<Song>("Sound/Music/Caketown")};

            _soundEffects = new SoundEffect[3] {content.Load<SoundEffect>("Sound/Effects/hover"),
                                                  content.Load<SoundEffect>("Sound/Effects/menuSelect"),
                                                   content.Load<SoundEffect>("Sound/Effects/menuSelect2")};
        }

        /// <summary>
        /// Method for soundeffect playback
        /// </summary>
        /// <param name="sound">Sound effect array index</param>
        /// <param name="volume">Sound volume</param>
        internal void PlaySound(Effect sound, float volume)
        {
            if(!_inputHandler.SoundDisabled)
                _soundEffects[Convert.ToInt32(sound)].Play(volume, 0f, 0f);
        }

        /// <summary>
        /// Checks wheter the given song is allready playing
        /// </summary>
        /// <param name="track">Track array index</param>
        public bool IsPlayingSong(Track track)
        {
            return MediaPlayer.State == MediaState.Playing && _activeSong == track;
        }

        /// <summary>
        /// Stops the current playback
        /// </summary>
        internal void StopTrack()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Checks wheter the given song is allready playing
        /// </summary>
        /// <param name="track">Track array index</param>
        internal void PlaySoundTrack(Track track)
        {
            if (_inputHandler.MusicDisabled)
                StopTrack();
            else if (!IsPlayingSong(track))
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = 0.4f;
                MediaPlayer.Play(_soundTracks[Convert.ToInt32(track)]);
                _activeSong = track;
            }
        }
    }
}
