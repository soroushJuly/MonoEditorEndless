using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Editor
{
    public class EditorConfigs
    {
        public float _spectateSensitivity;
        public float _spectateMoveSpeed;
        public bool _showInstructions;
        public int _selectedView;

        public EditorConfigs()
        {
            // Set editor default values
            _spectateSensitivity = 0.1f;
            _spectateMoveSpeed = 1f ;
            _showInstructions = true;
        }
    }
}
