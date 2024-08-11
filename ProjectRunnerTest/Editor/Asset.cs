using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MonoEditorEndless.Editor
{
    public class Asset
    {
        public readonly string _name;
        public string _type;
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
        public Asset(string name, string type, bool isUsedInGame = false, bool isEditorAsset = false)
        {
            _name = name;
            _type = type;
            _isUsedInGame = isUsedInGame;
            _isEditorAsset = isEditorAsset;
        }
        protected void GenerateContentText()
        {
            _pathString = _isEditorAsset ? "Editor/" + _type + "/" + _name : _type + "/" + _name;
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
