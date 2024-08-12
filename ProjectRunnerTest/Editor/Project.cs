using System;
using System.Collections.Generic;

namespace MonoEditorEndless.Editor
{
    public class Project
    {

        public List<AssetAudio> _audioList;
        public List<AssetModel> _modelList;
        public List<AssetTexture> _textureList;
        public List<AssetFont> _fontList;
        // TODO: this should not be coupled here
        public GameConfigs _gameConfigs;
        public EditorConfigs _editorConfigs;
        public DateTime _lastSaved;

        public event EventHandler AssetAdded;
        public Project()
        {
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
            _audioList.Add(new AssetAudio("mario_coin_sound.mp3"));
            _audioList.Add(new AssetAudio("Titan.mp3"));
            _modelList.Add(new AssetModel("bridge-straight.fbx"));
            _modelList.Add(new AssetModel("Coin.fbx"));
            _modelList.Add(new AssetModel("Ship.fbx"));
            _modelList.Add(new AssetModel("rocks-small.fbx"));
            _modelList.Add(new AssetModel("wall-corner.fbx"));
            _modelList.Add(new AssetModel("wall-half.fbx"));
            _modelList.Add(new AssetModel("wall.fbx"));
            _textureList.Add(new AssetTexture("bg.jpg"));
            _textureList.Add(new AssetTexture("top.bmp"));
            _textureList.Add(new AssetTexture("right.bmp"));
            _textureList.Add(new AssetTexture("left.bmp"));
            _textureList.Add(new AssetTexture("front.bmp"));
            _textureList.Add(new AssetTexture("bottom.bmp"));
            _textureList.Add(new AssetTexture("back.bmp"));
            _textureList.Add(new AssetTexture("colormap.png"));
            _textureList.Add(new AssetTexture("Coin2_BaseColor.jpg"));
            _textureList.Add(new AssetTexture("ShipDiffuse.tga"));
            _textureList.Add(new AssetTexture("ShipDiffuse_0.tga"));
            _textureList.Add(new AssetTexture("heart.png"));
            _textureList.Add(new AssetTexture("grass.jpg"));
            _fontList.Add(new AssetFont("File.spritefont"));
        }
        public List<Asset> GetAllAsset()
        {
            List<Asset> assets = new List<Asset>();
            assets.AddRange(_audioList);
            assets.AddRange(_textureList);
            assets.AddRange(_modelList);
            assets.AddRange(_fontList);
            return assets;
        }
        // TODO: better structure for these methods
        public void AddAssetAudio(AssetAudio asset)
        {
            _audioList.Add(asset);
            AssetAdded(this, EventArgs.Empty);
        }
        public void AddAssetTexture(AssetTexture asset)
        {
            _textureList.Add(asset);
            AssetAdded(this, EventArgs.Empty);
        }
        public void AddAssetModel(AssetModel asset)
        {
            _modelList.Add(asset);
            AssetAdded(this, EventArgs.Empty);
        }
        public void AddAssetFont(AssetFont asset)
        {
            _fontList.Add(asset);
            AssetAdded(this, EventArgs.Empty);
        }
    }
}
