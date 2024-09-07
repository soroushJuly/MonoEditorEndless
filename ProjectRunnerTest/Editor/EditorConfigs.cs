namespace MonoEditorEndless.Editor
{
    public class EditorConfigs
    {
        public float _spectateSensitivity;
        public float _spectateMoveSpeed;
        public bool _showInstructions;
        public bool _showCollisionBoxes;
        public int _selectedView;
        public bool _noInput;
        // HUD maker
        public bool _isShowFrame;
        public bool _isFromLeft;
        public bool _isFromTop;
        public bool _isScoreFromLeft;
        public bool _isScoreFromTop;
        // Menu Maker
        public bool _isTitleFromTop;
        public bool _isTitleFromLeft;
        public bool _isListFromLeft;
        public bool _isListFromTop;
        public bool _isControlsFromLeft;
        public bool _isControlsFromTop;

        public EditorConfigs()
        {
            // Set editor default values
            _spectateSensitivity = 0.1f;
            _spectateMoveSpeed = 1f ;
            _noInput = false;
            _showInstructions = true;
            _showCollisionBoxes = true;
            // show frame in editor
            _isShowFrame = true;
            _isFromTop = true;
            _isFromLeft = true;
            _isScoreFromLeft = true;
            _isScoreFromTop = true;
            _isTitleFromTop = true;
            _isTitleFromLeft = true;
            _isListFromLeft = true;
            _isListFromTop = true;
            _isControlsFromTop = true;
            _isControlsFromLeft = true;
        }
    }
}
