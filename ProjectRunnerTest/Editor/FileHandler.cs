using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using Forms = System.Windows.Forms;


namespace MonoEditorEndless.Editor
{
    internal class FileHandler
    {
        /// <summary>
        /// save an object of type T into an XML type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public bool SaveXml<T>(T instance, string name, string basePath)
        {
            bool status = true;
            try
            {
                // Overrides the file if it already exists
                using (var writer = new StreamWriter(new FileStream(Path.Combine(basePath, name), FileMode.Create)))
                {
                    XmlSerializer serializer = new XmlSerializer(instance.GetType());

                    serializer.Serialize(writer, instance);
                }
            }
            catch
            {
                status = false;
            }
            return status;
        }
        /// <summary>
        /// Load an XML file to an object of class T and return the new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pp"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public T LoadClassXml<T>(T instance, string filepath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    instance = (T)new XmlSerializer(typeof(T)).Deserialize(reader.BaseStream);
                }
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message 
                // describing the error 
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            return instance;
        }
        /// <summary>
        /// Opens up a dialog and waits till it resolves
        /// Copy the selected file in the content folder
        /// </summary>
        /// <returns>path to the file</returns>
        public string LoadFileFromComputer()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            var newPath = string.Empty;
            Thread thread = new Thread(() =>
            {
                using (Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    //openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    // Opens the Dialog 
                    if (openFileDialog.ShowDialog() == Forms.DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;

                        // Copying logic
                        // Create the new path to copy the file into
                        string[] paths = { Routes.CONTENT_DIRECTORY, "..", "Audio", Path.GetFileName(filePath) };
                        Path.GetExtension(filePath);
                        newPath = Path.Combine(paths);

                        try
                        {
                            // Check if the source file exists
                            if (File.Exists(filePath))
                            {
                                // Copy the source file to the destination file
                                File.Copy(filePath, newPath, true);
                                Debug.WriteLine("File copied successfully.");
                            }
                            else
                            {
                                Debug.WriteLine("Source file does not exist.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions that may occur
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }

                        // Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return newPath;
        }
        /// <summary>
        /// Opens up a dialog and waits till it resolves
        /// </summary>
        /// <returns></returns>
        public string LoadFileFromComputerNoCopy()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            Thread thread = new Thread(() =>
            {
                using (Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    //openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    // Opens the Dialog 
                    if (openFileDialog.ShowDialog() == Forms.DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;

                        // Copying logic
                        // Create the new path to copy the file into
                        string[] paths = { Routes.CONTENT_DIRECTORY, "..", "Audio", Path.GetFileName(filePath) };
                        Path.GetExtension(filePath);

                     

                        // Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return filePath;
        }
        /// <summary>
        /// Checks if a file is in right format
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_allowedExtentions"></param>
        /// <returns></returns>
        public bool CheckValidity(string name, string[] _allowedExtentions)
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
    }
}
