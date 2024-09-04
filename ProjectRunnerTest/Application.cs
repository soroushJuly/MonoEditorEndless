using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless;
using MonoEditorEndless.Editor;
using MonoEditorEndless.Editor.Components;
using MonoEditorEndless.Editor.Layouts;
using MonoEditorEndless.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using System.Threading;
using Forms = System.Windows.Forms;
using Num = System.Numerics;

namespace ProjectRunnerTest
{
    /// <summary>
    /// Simple FNA + ImGui example
    /// </summary>
    public class Application : Game
    {
        public static GraphicsDeviceManager _graphics;
        // To manage and communicate with the game
        private GameHandle _gameHandle;
        // To manage and communicate with the editor
        private static EditorHandle _editorHandle;

        // Current project class being used
        public static Project _project;

        private string _platform = "win-x64";

        // Current build mode
        public static bool _isDebug = false;

        // encapsulate event Controls for the application
        ControlsAggregator aggregator = new ControlsAggregator();

        public Application()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferMultiSampling = true;

            //_graphics.IsFullScreen = true;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // If in debug Mode Change the flag
            // Debug mode is editor mode
#if DEBUG
            _isDebug = true;
#endif
            // Initialize file handler
            FileHandler fileHandler = new FileHandler();

            // Load the project on initialize
            _project = new Project();
            string recentProjectName = null;
            // Find and load the most recent Project
            recentProjectName = fileHandler.LoadClassXml(recentProjectName, Path.Combine(Routes.SAVED_PROJECTS, "recent_project.xml"));
            if (recentProjectName != null)
            {
                // Load the most recent project
                _project = fileHandler.LoadClassXml(_project, Path.Combine(Routes.SAVED_PROJECTS, recentProjectName));
            }
            else
            {
                // If no recent project Create the default project
                _project.CreateDefault();
                // Then save the new project
                if (fileHandler.SaveXml<Project>(_project, "default_project.xml", Routes.SAVED_PROJECTS))
                    // Update recent project
                    fileHandler.SaveXml<string>(new string("default_project.xml"), "recent_project.xml", Routes.SAVED_PROJECTS);
            }

            _project.AssetAdded += (object sender, AssetAdditionArgs e) =>
            {
                string ContentFile = File.ReadAllText(Routes.CONTENT_FILE);
                UpdateContent(_project.GetAllAsset());
                // If build fails
                if (!BuildContent())
                {
                    // Revert change if the build process wasn't successful
                    _project.RemoveLastAsset();
                    // Change the content file to what is was before
                    File.WriteAllText(Routes.CONTENT_FILE, ContentFile);
                }
                // If content build is successful we change the field in game config project
                else
                {
                    // Using system reflection to dynamically change a member value
                    FieldInfo field = _project._gameConfigs.GetType().GetField(e.role);
                    if (field != null)
                    {
                        field.SetValue(Application._project._gameConfigs, e.name);
                    }
                    // Refresh the game
                    _gameHandle?.Refresh();

                    ModalLoading.Instance.Stop();
                }
            };

            _editorHandle = new EditorHandle(this, _graphics, aggregator);


            // Update the content file
            UpdateContent(_project.GetAllAsset());
            // Build the newly updated Content file
            BuildContent();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load editor contents
            _editorHandle.LoadContent(Content);
            // Initialize the game handle
            _gameHandle = new GameHandle(Content, GraphicsDevice);
            _gameHandle.RaisePause += (object sender, EventArgs e) => { aggregator.RaisePausePressed(); };
            aggregator.PlayPressed += (object sender, EventArgs e) => { _gameHandle.Start(); };
            aggregator.PausePressed += (object sender, EventArgs e) => { _gameHandle.Stop(); };
            aggregator.PlayFromStartPressed += (object sender, EventArgs e) => { _gameHandle.Start(true); };
            aggregator.RestartPressed += (object sender, ReplayEventArgs e) => { _gameHandle.Restart(e.IsFromStart()); };
            aggregator.MenuMakerPressed += (object sender, EventArgs e) => { _gameHandle.MenuMaker(); };
            aggregator.HUDMakerPressed += (object sender, EventArgs e) => { _gameHandle.HUDMaker(); };
            aggregator.RefreshSpectate += (object sender, EventArgs e) => { _gameHandle.Refresh(); };
            aggregator.SpectatePressed += (object sender, EventArgs e) => { _gameHandle.Spectate(); };


            base.LoadContent();
        }
        private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            // Render the game
            _gameHandle.Draw(GraphicsDevice);
            // Render editor on top of the game
            _editorHandle.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            _editorHandle.Update(gameTime);
            _gameHandle.Update(gameTime);
        }
        public static bool BuildContent()
        {
            string contentProjectPath = Routes.CONTENT_FILE; // Path to content.mgbc project

            string result = "";
            string error = null;
            int exitCode = 0;

            Thread thread = new Thread(() =>
            {
                string CurrentDirectory = Environment.CurrentDirectory;
                string buildMode = (_isDebug ? "Debug" : "Release");
                string platform = (_isDebug ? "" : "/win-x64");
                //System.Runtime.
                // Going back to root
                CurrentDirectory = Path.GetFullPath(Path.Combine(CurrentDirectory, "..", "..", ".."));
                string Intermediate = Path.GetFullPath(Path.Combine(CurrentDirectory, "Intermediate"));
                // Ensure the intermediate directory exists
                if (!Directory.Exists(Intermediate))
                {
                    Directory.CreateDirectory(Intermediate);
                }

                var processInfo = new ProcessStartInfo("dotnet")
                {
                    //Arguments = "",
                    Arguments = $"mgcb /@:{contentProjectPath} /platform:DesktopGL /outputDir:bin/{buildMode}/{Routes.FRAMEWORK_TARGET} /intermediateDir:\"{Intermediate}\" /quiet",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                    //WorkingDirectory = CurrentDirectory,
                    //CreateNoWindow = true
                };
                using (var process = Process.Start(processInfo))
                {
                    // Capture the output and error streams
                    using (var reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                        int lastLineIndex = result.LastIndexOf("\r\n");
                        result = result.Substring(lastLineIndex);
                        Console.WriteLine("Build Content: " + result);
                    }
                    using (var reader = process.StandardError)
                    {
                        error = reader.ReadToEnd();
                    }
                    exitCode = process.ExitCode;
                }
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error content build:");
                Console.WriteLine(error);
            }
            if (exitCode != 0)
            {
                Forms.MessageBox.Show("Building new content failed, File is not compatible.\r\n" + result);
                ModalLoading.Instance.Stop();
                return false;
            }

            return true;
        }
        /// <summary>
        /// Updates the content.mgbc file based on the 
        /// assets from project and assets needed by editor
        /// </summary>
        /// <param name="assetList"></param>
        public static void UpdateContent(List<Asset> assetList)
        {
            string contentFileText = File.ReadAllText(Routes.CONTENT_FILE);
            foreach (Asset asset in assetList)
            {
                // If asset is already in file don't add it again
                if (!contentFileText.Contains(asset.GetPathString()))
                    File.AppendAllText(Routes.CONTENT_FILE, asset.GetContentText());
            }
            foreach (Asset asset in _editorHandle.GetAssets())
            {
                // If asset is already in file don't add it again
                if (!contentFileText.Contains(asset.GetPathString()))
                    File.AppendAllText(Routes.CONTENT_FILE, asset.GetContentText());
            }
        }
    }
}