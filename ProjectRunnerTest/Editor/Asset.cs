using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Editor
{
    internal class Asset
    {
        protected readonly string _name;
        private string _type;
        private string _format;
        private bool _isUsedInGame;
        protected string _importerDetails;
        private string _mgcbText;

        public string GetContentText() { return _mgcbText; }
        public Asset(string name, string type, bool isUsedInGame = false)
        {
            _name = name;
            _type = type;
            _isUsedInGame = isUsedInGame;
        }
        protected void GenerateContentText()
        {
            _mgcbText = "#begin " + _type + "/" + _name + "\r\n" + _importerDetails +
                "/build:" + _type + "/" + _name + "\r\n\r\n";
        }
    }
}
