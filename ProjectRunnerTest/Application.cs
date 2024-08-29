using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless;
using MonoEditorEndless.Editor;
using MonoEditorEndless.Editor.Components;
using MonoEditorEndless.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using System.Threading;
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
        private static bool _isDebug = false;

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

            _project.AssetAdded += (object sender, EventArgs e) => { UpdateContent(_project.GetAllAsset()); BuildContent(); _gameHandle.Refresh(); };

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
            aggregator.PlayPressed += (object sender, EventArgs e) => { _gameHandle.Start(); };
            aggregator.PausePressed += (object sender, EventArgs e) => { _gameHandle.Stop(); };
            aggregator.PlayFromStartPressed += (object sender, EventArgs e) => { _gameHandle.Start(true); };
            aggregator.RestartPressed += (object sender, ReplayEventArgs e) => { _gameHandle.Restart(e.IsFromStart()); };
            aggregator.MenuMakerPressed += (object sender, EventArgs e) => { _gameHandle.MenuMaker(); };
            aggregator.RefreshSpectate += (object sender, EventArgs e) => { _gameHandle.Refresh(); };



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
        public static void BuildContent()
        {
            string contentProjectPath = Routes.CONTENT_FILE; // Path to content.mgbc project
            Thread thread = new Thread(() =>
            {
                string CurrentDirectory = Environment.CurrentDirectory;
                string buildMode = _isDebug ? "Debug" : "Release";
                string platform = _isDebug ? "" : "/win-x64";
                //System.Runtime.
                CurrentDirectory = Path.Combine(CurrentDirectory, "..", "..", "..");
                var processInfo = new ProcessStartInfo("dotnet")
                {
                    //Arguments = "",
                    Arguments = $"mgcb /build /@:{contentProjectPath} /platform:DesktopGL /outputDir:bin/{buildMode}/{Routes.FRAMEWORK_TARGET} /intermediateDir:obj /quiet",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    //CreateNoWindow = true
                    //WorkingDirectory = CurrentDirectory,
                    //CreateNoWindow = true
                };
                using (var process = Process.Start(processInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
            ModalLoading.Instance.Stop();
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