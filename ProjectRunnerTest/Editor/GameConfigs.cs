
namespace MonoEditorEndless.Editor
{
    public class GameConfigs
    {
        // Game title
        public string _title;
        // Audio
        public string menuMusic;
        public string gameplayMusic;
        public string itemCollectionSound;
        public string itemCollectionSoundInstance;
        // Models in the game
        public string _characterModel;

        // Font

        // Textures

        // Camera setting
        public float distanceFromCharacter;
        public float cameraLookDistance;
        public float cameraHeight;
        // Gameplay
        public float characterMinSpeed;
        public bool characterHasMaxSpeed;
        public float characterMaxSpeed;
        public int characterHealth;
        public float characterMoveSensitivity;
        public int itemValue;
        // 



        public GameConfigs()
        {
            _title = "Untitled";
            menuMusic = null;
            gameplayMusic = null;
            itemCollectionSound = null;
            distanceFromCharacter = 250f;
            cameraLookDistance = 100f;
            cameraHeight = 100f;
            characterMinSpeed = 160f;
            characterHasMaxSpeed = true;
            characterMaxSpeed = 400f;
            characterHealth = 4;
            characterMoveSensitivity = 1f;
            itemValue = 100;

        }
    }
}
