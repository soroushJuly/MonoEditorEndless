using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoEditorEndless.Editor
{
    public class GameConfigs
    {
        // Game title
        public string _title;
        // Audio
        public Song menuMusic;
        public Song gameplayMusic;
        public SoundEffect itemCollectionSound;
        public SoundEffectInstance itemCollectionSoundInstance;
        // Models in the game
        public string _characterModel;

        // Font

        // Textures 
        


        public GameConfigs()
        {
            _title = "";
            menuMusic = null;
            gameplayMusic = null;
            itemCollectionSound = null;
        }
    }
}
