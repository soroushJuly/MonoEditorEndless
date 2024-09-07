
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;

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
        public string characterModel;
        public float characterScale;
        public float characterRotateY;
        public string collectableModel;
        public float collectableScale;
        public float collectableOffset;
        public string obstacleModel;
        public float obstacleScale;
        public string blockStraightModel;
        public float blockStraightScale;
        public string blockTurnModel;
        public float blockTurnScale;
        // Font
        // TODO: Should be dynamic later
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
        public int obstacleChance;
        public int collectableChance;
        public bool isCollectableRotating;
        public float gameAcceleration;
        public int obstacleBehavior;
        // Light
        public Vector3 sunDiffuseColor;
        public Vector3 sunSpecularColor;
        public Vector3 sunDirection;
        // Fog
        public bool fogEnable;
        public Vector3 fogColor;
        public float fogStartDistance;
        public float fogEndDistance;
        // Sky
        public string skyTop;
        public string skyBottom;
        public string skyRight;
        public string skyLeft;
        public string skyFront;
        public string skyBack;
        // Plane
        public string planeTexture;
        // Audio
        public string audioBackground;
        public float audioBackgroundVolume;
        public string audioCollected;
        public float audioCollectedVolume;
        public string audioCollided;
        public float audioCollidedVolume;
        // HUD
        public string healthIcon;
        public float healthScale;
        public Vector2 healthPosition;
        public Vector2 scorePosition;
        // Menus
        public string mainMenuBackground;
        public Vector2 buttonSize;
        public Vector3 buttonColor;
        public Vector3 titleColor;
        public float titleSize;
        public Vector2 titlePosition;
        public Vector2 listPosition;
        public float listPadding;
        public Vector2 controlsPosition;
        public float controlsPadding;
        public Vector3 controlsColor;


        public GameConfigs()
        {
            _title = "Untitled";
            menuMusic = null;
            gameplayMusic = null;
            itemCollectionSound = null;
            distanceFromCharacter = 250f;
            cameraLookDistance = 100f;
            cameraHeight = 100f;
            characterMinSpeed = 180f;
            characterHasMaxSpeed = true;
            characterMaxSpeed = 350f;
            characterHealth = 4;
            characterMoveSensitivity = 1f;
            itemValue = 100;
            obstacleChance = 10;
            collectableChance = 20;
            isCollectableRotating = false;
            gameAcceleration = 0.001f;
            obstacleBehavior = 0;
            sunDiffuseColor = new Vector3(1, .8f, .6f); // Warm Orange
            sunSpecularColor = new Vector3(1, 1, 1); // White
            // Approximation of 3 p.m.
            sunDirection = new Vector3(0.5f, -0.75f, -0.25f);
            // Fog
            fogEnable = false;
            fogColor = new Vector3(.6f);
            fogStartDistance = 900.75f;
            fogEndDistance = 1000.25f;
            // Models
            collectableModel = "Coin";
            collectableScale = .1f;
            collectableOffset = 1f;
            obstacleModel = "rocks-small";
            obstacleScale = 0.15f;
            characterModel = "Ship";
            characterScale = .012f;
            characterRotateY = -90f;
            blockStraightModel = "wall";
            blockStraightScale = 1f;
            blockTurnModel = "wall-corner";
            blockTurnScale = 1f;
            // Sky
            skyTop = "top";
            skyBottom = "bottom";
            skyRight = "right";
            skyLeft = "left";
            skyFront = "front";
            skyBack = "back";
            // Plane
            planeTexture = "grass";
            // Audio
            audioBackground = "Titan";
            audioBackgroundVolume = 1f;
            audioCollected = "mario_coin_sound";
            audioCollectedVolume = .3f;
            audioCollided = "wall_hit";
            audioCollidedVolume = 1f;
            // HUD
            healthIcon = "heart";
            healthScale = 1f;
            healthPosition = new Vector2(200, 100);
            scorePosition = new Vector2(200, 50);
            // Menu
            mainMenuBackground = "bg";
            buttonSize = new Vector2(150, 30);
            buttonColor = new Vector3(.3f, .3f, .3f);
            titleColor = new Vector3(.3f);
            titleSize = 2f;
            titlePosition = new Vector2(50f);
            listPosition = new Vector2(50f,180f);
            listPadding = 50f;
            controlsPosition = new Vector2(50f, 290f);
            controlsColor = new Vector3(1f);
            controlsPadding = 40f;
        }
    }
}
