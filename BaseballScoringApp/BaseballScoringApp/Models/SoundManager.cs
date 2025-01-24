using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    //used a singleton instance !!

    public class SoundManager
    {
        public static SoundManager _soundManager = null;

        private readonly IAudioManager _audioManager; //set from outside

        private List<IAudioPlayer> _audioPlayerList;
        public SoundManager()
        {
            _audioPlayerList = new List<IAudioPlayer>();
            _audioManager = MauiProgram.ServiceProvider.GetRequiredService<IAudioManager>();
        }
        //singleton instande datarepository
        public static SoundManager getInstance()
        {
            if (_soundManager == null)
            {
                _soundManager = new SoundManager();
            }
            return _soundManager;
        }
        public async void PlaySound(string mp3filename)
        {
            IAudioPlayer aNewPlayer = null;
            var audioStream = await FileSystem.OpenAppPackageFileAsync(mp3filename);
            aNewPlayer = _audioManager.CreatePlayer(audioStream);
            _audioPlayerList.Add(aNewPlayer);

            if (aNewPlayer != null)
            {
                aNewPlayer.Play();

                // Optionally, subscribe to an event or use a delay to ensure the player is retained
                aNewPlayer.PlaybackEnded += (sender, e) =>
                {
                    //when the play of sound is ended, remove it from the list and dispose
                    if(_audioPlayerList.Contains(aNewPlayer))
                        _audioPlayerList.Remove(aNewPlayer);
                    //aNewPlayer.Dispose();
                    aNewPlayer  = null;
                };
            }
        }
    }
}
