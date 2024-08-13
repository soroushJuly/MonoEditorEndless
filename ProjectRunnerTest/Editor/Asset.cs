using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Forms = System.Windows.Forms;


namespace MonoEditorEndless.Editor
{
    public enum AssetType
    {
        AUDIO,
        TEXTURE,
        MODEL,
        FONT
    }
    public abstract class Asset
    {
        public readonly string _name;
        public readonly string _nameWithoutExtenstion;
        public string _type;
        public AssetType _assetType;
        public string _format;
        public bool _isUsedInGame;
        public string _importerDetails;
        public string _mgcbText;
        public string _pathString;
        public string _contentPathString;
        public bool _isEditorAsset;

        public string GetContentText() { return _mgcbText; }
        public string GetPathString() { return _pathString; }
        public Asset()
        {
            _name = "";
            _type = "";
            _isUsedInGame = false;
        }
        public Asset(string name, string type, bool isUsedInGame = false, bool isEditorAsset = false)
        {
            _name = name;
            _nameWithoutExtenstion = Path.GetFileNameWithoutExtension(_name);
            _type = type;
            _isUsedInGame = isUsedInGame;
            _isEditorAsset = isEditorAsset;
        }
        protected void GenerateContentText()
        {
            _pathString = _isEditorAsset ? "Editor/" + _type + "/" + _name : _type + "/" + _name;
            _contentPathString = "Content/" + (_isEditorAsset ? "Editor/" + _type + "/" + _nameWithoutExtenstion : _type + "/" + _nameWithoutExtenstion);
            _mgcbText = "#begin " + _pathString + "\r\n" + _importerDetails +
                "/build:" + _pathString + "\r\n";
        }

        // TODO: If we want the members to be private
        //public XmlSchema GetSchema()
        //{
        //    return (null);
        //}
        //public void WriteXml(XmlWriter writer)
        //{
        //    writer.WriteAttributeString("name", _name);
        //    writer.WriteString(_type);
        //    writer.WriteString(_format);
        //    writer.WriteString(_isUsedInGame.ToString());
        //    writer.WriteString(_importerDetails);
        //    writer.WriteString(_mgcbText);
        //    //private bool _isUsedInGame;
        //    //protected string _importerDetails;
        //    //private string _mgcbText;
        //}

        //public void ReadXml(XmlReader reader)
        //{
        //    //personName = reader.ReadString();
        //}
    }
}
