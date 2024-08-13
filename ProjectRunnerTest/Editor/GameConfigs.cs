using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoEditorEndless.Editor
{
    public class GameConfigs
    {
        // Game title
        public string _title = "Asghar game";
        // Audio
        public string menuMusic;
        public string gameplayMusic;
        public string itemCollectionSound;
        public string itemCollectionSoundInstance;
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
