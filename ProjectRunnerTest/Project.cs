using System;
using System.Collections.Generic;
using MonoEditorEndless.Editor;

namespace MonoEditorEndless
{
    public class AssetAdditionArgs : EventArgs
    {
        // address of config in gameconfigs
        public string role { get; }
        // name of the file
        public string name { get; }
        public AssetAdditionArgs(string role, string name)
        {
            this.role = role;
            this.name = name;
        }
    }
    /// <summary>
    /// This class contains the details of the project 
    /// </summary>
    public class Project
    {
        public List<Asset> _assets;
        public List<AssetAudio> _audioList;
        public List<AssetModel> _modelList;
        public List<AssetTexture> _textureList;
        public List<AssetFont> _fontList;
        // TODO: this should not be coupled here
        public GameConfigs _gameConfigs;
        public EditorConfigs _editorConfigs;
        public DateTime _lastSaved;

        public event EventHandler<AssetAdditionArgs> AssetAdded;
        public Project()
        {
            _assets = new List<Asset>();
            _audioList = new List<AssetAudio>();
            _modelList = new List<AssetModel>();
            _textureList = new List<AssetTexture>();
            _fontList = new List<AssetFont>();
            _gameConfigs = new GameConfigs();
            _editorConfigs = new EditorConfigs();
        }

        public void CreateDefault()
        {
            // Preoccupy the asset list with the current assets
            _assets.Add(new AssetAudio("mario_coin_sound.mp3"));
            _assets.Add(new AssetAudio("Titan.mp3"));
            _assets.Add(new AssetAudio("wall_hit.mp3"));
            _assets.Add(new AssetModel("bridge-straight.fbx"));
            _assets.Add(new AssetModel("Coin.fbx"));
            _assets.Add(new AssetModel("Ship.fbx"));
            _assets.Add(new AssetModel("rocks-small.fbx"));
            _assets.Add(new AssetModel("wall-corner.fbx"));
            _assets.Add(new AssetModel("wall-half.fbx"));
            _assets.Add(new AssetModel("wall.fbx"));
            _assets.Add(new AssetTexture("bg.jpg"));
            _assets.Add(new AssetTexture("top.bmp"));
            _assets.Add(new AssetTexture("right.bmp"));
            _assets.Add(new AssetTexture("left.bmp"));
            _assets.Add(new AssetTexture("front.bmp"));
            _assets.Add(new AssetTexture("bottom.bmp"));
            _assets.Add(new AssetTexture("back.bmp"));
            _assets.Add(new AssetTexture("colormap.png"));
            _assets.Add(new AssetTexture("Coin2_BaseColor.jpg"));
            _assets.Add(new AssetTexture("ShipDiffuse.tga"));
            _assets.Add(new AssetTexture("ShipDiffuse_0.tga"));
            _assets.Add(new AssetTexture("spaceship.png"));
            _assets.Add(new AssetTexture("grass.jpg"));
            _assets.Add(new AssetFont("File.spritefont"));
        }
        public List<Asset> GetAllAsset()
        {
            return _assets;
        }
        // TODO: better structure for these methods
        public void AddAssetAudio(AssetAudio asset, string role)
        {
            _assets.Add(asset);
            RaiseAssetAdded(role, asset._nameWithoutExtenstion);
        }
        public void AddAssetTexture(AssetTexture asset, string role)
        {
            _assets.Add(asset);
            RaiseAssetAdded(role, asset._nameWithoutExtenstion);
        }
        public void AddAssetModel(AssetModel asset, string role)
        {
            _assets.Add(asset);
            RaiseAssetAdded(role, asset._nameWithoutExtenstion);
        }
        public void AddAssetFont(AssetFont asset, string role)
        {
            _assets.Add(asset);
            RaiseAssetAdded(role, asset._nameWithoutExtenstion);
        }
        public void RemoveLastAsset()
        {
            _assets.RemoveAt(_assets.Count - 1);
        }
        public void RaiseAssetAdded(string role, string name)
        {
            AssetAdded?.Invoke(this, new AssetAdditionArgs(role, name));
        }
    }
}
