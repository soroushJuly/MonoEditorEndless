namespace MonoEditorEndless.Editor
{
    internal class AssetAudio : Asset
    {
        public AssetAudio(string name, bool isUsedInGame = false) : base(name, "Audio", isUsedInGame)
        {
            _importerDetails = "/importer:Mp3Importer\r\n/processor:SoundEffectProcessor\r\n/processorParam:Quality=Best\r\n";
            GenerateContentText();
        }
    }
}
