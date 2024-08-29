/// <summary>
/// Routes here are relative to the Debug folder
/// </summary>
namespace MonoEditorEndless
{
    internal class Routes
    {
        // Singleton Pattern
        Routes _routes;
        public const string CONTENT_FILE = @"..\..\..\Content\Content.mgcb";
        public const string CONTENT_DIRECTORY = @"..\..\..\Content";
        public const string ROOT_DIRECTORY = @"..\..\..\..";
        public const string SAVED_PROJECTS = @"..\..\..\Editor\Saved";
        public const string FRAMEWORK_TARGET = "net6.0-windows";
    }
}
