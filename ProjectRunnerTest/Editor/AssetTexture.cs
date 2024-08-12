using System.Linq;

namespace MonoEditorEndless.Editor
{
    public class AssetTexture : Asset
    {
        // Argument-less constructor to make the serialization possible
        public AssetTexture() : base()
        {
        }
        public AssetTexture(string name, bool isUsedInGame = false, bool isEditor = false) :
            base(name, "Texture", new string[] { ".bmp", ".png", ".jpg" }, isUsedInGame, isEditor)
        {
            if (ValidityCheck(name))
            {
                _importerDetails = "/importer:TextureImporter\r\n/processor:TextureProcessor\r\n/processorParam:ColorKeyColor=255,0,255,255\r\n/processorParam:ColorKeyEnabled=True\r\n/processorParam:GenerateMipmaps=False\r\n/processorParam:PremultiplyAlpha=True\r\n/processorParam:ResizeToPowerOfTwo=False\r\n/processorParam:MakeSquare=False\r\n/processorParam:TextureFormat=Color\r\n";
                GenerateContentText();
            }
        }
    }
}
