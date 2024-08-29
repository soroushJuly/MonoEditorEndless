using System;

namespace MonoEditorEndless.Editor
{
    [Serializable]
    public class AssetAudio : Asset
    {

        public static string[] _allowedExtentions = new string[] { ".mp3" };
        // Argument-less constructor to make the serialization possible
        public AssetAudio() : base()
        {
        }
        public AssetAudio(string name, bool isUsedInGame = false) : base(name, "Audio", isUsedInGame)
        {
            _importerDetails = "/importer:Mp3Importer\r\n/processor:SoundEffectProcessor\r\n/processorParam:Quality=Best\r\n";
            GenerateContentText();
            _assetType = AssetType.AUDIO;
        }
    }
}
