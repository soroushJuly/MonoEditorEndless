using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless;
using MonoEditorEndless.Editor;
using MonoEditorEndless.Editor.ImGuiTools;
using MonoEditorEndless.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
        private GraphicsDeviceManager _graphics;
        // To manage and communicate with the game
        private GameHandle _gameHandle;
        // To manage and communicate with the editor
        private EditorHandle _editorHandle;

        // Current project class being used
        private Project _project;

        private string _gameTitle = "Untitled";

        private string _bgMusicName;
        private string _platform = "win-x64";

        // Current build mode
        private bool _isDebug = false;

        private List<SoundEffect> _soundList;

        public Application()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferMultiSampling = true;

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
            recentProjectName = fileHandler.LoadClassXml(recentProjectName, Path.Combine(Routes.SAVED_PROJECTS, "recent_project"));
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
                if (fileHandler.SaveXml(_project, "default_project", Routes.SAVED_PROJECTS))
                    // Update recent project
                    fileHandler.SaveXml(new string("default_project"), "recent_project", Routes.SAVED_PROJECTS);
            }


            // Get the TargetFramework attribute
            //var targetFramework = Assembly.GetExecutingAssembly()
            //    .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
            //    .FirstOrDefault() as TargetFrameworkAttribute;

            //if (targetFramework != null)
            //{
            //    Debug.WriteLine($"Running on Target Framework: {targetFramework.FrameworkName}");
            //}
            //else
            //{
            //    Debug.WriteLine("Target Framework attribute not found.");
            //}
            _editorHandle = new EditorHandle(this, _graphics);

            // Update the content file
            UpdateContent(_project.GetAllAsset());

            _bgMusicName = "";

            BuildContent();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load editor contents
            _editorHandle.LoadContent(Content);
            // Initialize the game handle
            _gameHandle = new GameHandle(Content, GraphicsDevice);


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
            _gameHandle.Update(gameTime);
        }
        private string LoadFile()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            var newPath = string.Empty;
            SoundEffect soundEffect = null;
            Thread thread = new Thread(() =>
            {
                using (Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    // Opens the Dialog 
                    if (openFileDialog.ShowDialog() == Forms.DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        _bgMusicName = filePath;

                        // Copying logic
                        //string programDataDir =
                        //Environment.GetFolderPath(Environment.CurrentDirectory);\

                        string[] paths = { Environment.CurrentDirectory, "Content", "Audio", Path.GetFileName(filePath) };

                        newPath = Path.Combine(paths);


                        //string newPath = @"C:\Users\";
                        //File.SetAttributes(newPath, FileAttributes.Normal);
                        try
                        {
                            // Check if the source file exists
                            if (File.Exists(filePath))
                            {
                                // Copy the source file to the destination file
                                File.Copy(filePath, newPath, true);

                                Console.WriteLine("File copied successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Source file does not exist.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions that may occur
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                    }

                }


                //Forms.MessageBox.Show(fileContent, "File Content at path: " + filePath, Forms.MessageBoxButtons.OK);
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
            soundEffect = SoundEffect.FromFile(newPath);

            _soundList.Add(soundEffect);
            return filePath;
        }
        private void BuildContent()
        {
            string contentProjectPath = Routes.CONTENT_DIRECTORY; // Path to content project
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
        }
        private void UpdateContent(List<Asset> assetList)
        {
            string contentFileText = File.ReadAllText(Routes.CONTENT_DIRECTORY);
            foreach (Asset asset in assetList)
            {
                // If asset is already in file don't add it again
                if (!contentFileText.Contains(asset.GetPathString()))
                    File.AppendAllText(Routes.CONTENT_DIRECTORY, asset.GetContentText());
            }
        }
    }
}