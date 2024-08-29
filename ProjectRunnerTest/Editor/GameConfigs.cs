
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
        public string _characterModel;
        public string collectableModel;
        public float collectableScale;
        public float collectableOffset;
        public string obstacleModel;
        public float obstacleScale;

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
        public int obstacleChance;
        public int collectableChance;
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
            characterMaxSpeed = 250f;
            characterHealth = 4;
            characterMoveSensitivity = 1f;
            itemValue = 100;
            obstacleChance = 10;
            collectableChance = 20;
            gameAcceleration = 0.001f;
            obstacleBehavior = 0;
            sunDiffuseColor = new Vector3(1, .8f, .6f); // Warm Orange
            sunSpecularColor = new Vector3(1, 1, 1); // White
            // Approximation of 3 p.m.
            sunDirection = new Vector3(0.5f, -0.75f, -0.25f);
            // Fog
            fogEnable = true;
            fogColor = new Vector3(.6f);
            fogStartDistance = 900.75f;
            fogEndDistance = 1000.25f;
            // Models
            collectableModel = "coin.fbx";
            collectableScale = .1f;
            collectableOffset = 0f;
            obstacleModel = "rocks-small.fbx";
            obstacleScale = 0.15f;
        }
    }
}
