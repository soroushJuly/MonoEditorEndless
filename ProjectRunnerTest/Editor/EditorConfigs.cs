namespace MonoEditorEndless.Editor
{
    public class EditorConfigs
    {
        public float _spectateSensitivity;
        public float _spectateMoveSpeed;
        public bool _showInstructions;
        public int _selectedView;
        // HUD maker
        public bool _isShowFrame;
        public bool _isFromLeft;
        public bool _isFromTop;
        public bool _isScoreFromLeft;
        public bool _isScoreFromTop;

        public EditorConfigs()
        {
            // Set editor default values
            _spectateSensitivity = 0.1f;
            _spectateMoveSpeed = 1f ;
            _showInstructions = true;
            // show frame in editor
            _isShowFrame = true;
            _isFromTop = true;
            _isFromLeft = true;
            _isScoreFromLeft = true;
            _isScoreFromTop = true;
        }
    }
}
