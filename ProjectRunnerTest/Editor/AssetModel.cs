namespace MonoEditorEndless.Editor
{
    public class AssetModel : Asset
    {
        public static string[] _allowedExtentions = new string[] { ".fbx" };
        // Argument-less constructor to make the serialization possible
        public AssetModel() : base()
        {
        }
        public AssetModel(string name, bool isUsedInGame = false, bool isInEditor = false) : base(name, "Model", isUsedInGame, isInEditor)
        {
            _importerDetails = "/importer:FbxImporter\r\n/processor:ModelProcessor\r\n/processorParam:ColorKeyColor=0,0,0,0\r\n/processorParam:ColorKeyEnabled=True\r\n/processorParam:DefaultEffect=BasicEffect\r\n/processorParam:GenerateMipmaps=True\r\n/processorParam:GenerateTangentFrames=False\r\n/processorParam:PremultiplyTextureAlpha=True\r\n/processorParam:PremultiplyVertexColors=True\r\n/processorParam:ResizeTexturesToPowerOfTwo=False\r\n/processorParam:RotationX=0\r\n/processorParam:RotationY=0\r\n/processorParam:RotationZ=0\r\n/processorParam:Scale=1\r\n/processorParam:SwapWindingOrder=False\r\n/processorParam:TextureFormat=Compressed\r\n";
            GenerateContentText();
        }
    }
}
