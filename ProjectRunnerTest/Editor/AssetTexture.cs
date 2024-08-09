namespace MonoEditorEndless.Editor
{
    internal class AssetTexture : Asset
    {
        public AssetTexture(string name, bool isUsedInGame = false) : base(name, "Texture", isUsedInGame)
        {
            _importerDetails = "/importer:TextureImporter\r\n/processor:TextureProcessor\r\n/processorParam:ColorKeyColor=255,0,255,255\r\n/processorParam:ColorKeyEnabled=True\r\n/processorParam:GenerateMipmaps=False\r\n/processorParam:PremultiplyAlpha=True\r\n/processorParam:ResizeToPowerOfTwo=False\r\n/processorParam:MakeSquare=False\r\n/processorParam:TextureFormat=Color\r\n";
            GenerateContentText();
        }
    }
}
