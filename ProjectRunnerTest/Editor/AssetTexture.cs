using System.Linq;

namespace MonoEditorEndless.Editor
{
    public class AssetTexture : Asset
    {
        public static string[] _allowedExtentions = new string[] { ".bmp", ".png", ".jpg" };
        // Argument-less constructor to make the serialization possible
        public AssetTexture() : base()
        {
        }
        public AssetTexture(string name, bool isUsedInGame = false, bool isEditor = false) :
            base(name, "Texture", isUsedInGame, isEditor)
        {

            _importerDetails = "/importer:TextureImporter\r\n/processor:TextureProcessor\r\n/processorParam:ColorKeyColor=255,0,255,255\r\n/processorParam:ColorKeyEnabled=True\r\n/processorParam:GenerateMipmaps=False\r\n/processorParam:PremultiplyAlpha=True\r\n/processorParam:ResizeToPowerOfTwo=False\r\n/processorParam:MakeSquare=False\r\n/processorParam:TextureFormat=Color\r\n";
            GenerateContentText();

        }
    }
}
