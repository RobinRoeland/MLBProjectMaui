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
        private IAudioPlayer _audioPlayer;

        public SoundManager()
        {
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
            if (_audioPlayer == null)
            {
                var audioStream = await FileSystem.OpenAppPackageFileAsync(mp3filename);
                _audioPlayer = _audioManager.CreatePlayer(audioStream);
            }

            _audioPlayer.Play();
            _audioPlayer = null;
        }
    }
}
