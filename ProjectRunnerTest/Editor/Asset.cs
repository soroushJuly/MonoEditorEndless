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
    public class Asset
    {
        public readonly string _name;
        public string _type;
        public AssetType _assetType;
        public string[] _allowedExtentions;
        public string _format;
        public bool _isUsedInGame;
        public string _importerDetails;
        public string _mgcbText;
        public string _pathString;
        public bool _isEditorAsset;

        public string GetContentText() { return _mgcbText; }
        public string GetPathString() { return _pathString; }
        public Asset()
        {
            _name = "";
            _type = "";
            _isUsedInGame = false;
        }
        public Asset(string name, string type, string[] extensions, bool isUsedInGame = false, bool isEditorAsset = false)
        {
            _name = name;
            _type = type;
            _isUsedInGame = isUsedInGame;
            _isEditorAsset = isEditorAsset;
            _allowedExtentions = extensions;
        }
        protected void GenerateContentText()
        {
            _pathString = _isEditorAsset ? "Editor/" + _type + "/" + _name : _type + "/" + _name;
            _mgcbText = "#begin " + _pathString + "\r\n" + _importerDetails +
                "/build:" + _pathString + "\r\n";
        }
        public bool ValidityCheck(string name)
        {
            bool status = false;
            string fileExtention = Path.GetExtension(name);

            foreach (var allowedExtension in _allowedExtentions)
            {
                if (allowedExtension == fileExtention)
                {
                    status = true;
                    break;
                }
            }
            if (!status)
            {
                string allowedExt = "";
                foreach (var allowedExtension in _allowedExtentions)
                {
                    allowedExt += allowedExtension + " ";
                }
                Forms.MessageBox.Show("Please select a proper file type\n\r" + allowedExt);
            }

            return status;
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
