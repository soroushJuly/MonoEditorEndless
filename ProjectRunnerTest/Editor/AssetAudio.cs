using System;

namespace MonoEditorEndless.Editor
{
    [Serializable]
    public class AssetAudio : Asset
    {
        // Argument-less constructor to make the serialization possible
        public AssetAudio() : base()
        {
        }
        public AssetAudio(string name, bool isUsedInGame = false) : base(name, "Audio", new string[] { ".mp3" }, isUsedInGame)
        {
            if (ValidityCheck(name))
            {
                _importerDetails = "/importer:Mp3Importer\r\n/processor:SoundEffectProcessor\r\n/processorParam:Quality=Best\r\n";
                GenerateContentText();
            }
        }
    }
}
