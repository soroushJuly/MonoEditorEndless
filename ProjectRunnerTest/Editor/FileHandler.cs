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
        public T LoadClassXml<T>(T pp, string filepath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    pp = (T)new XmlSerializer(typeof(T)).Deserialize(reader.BaseStream);
                }
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message 
                // describing the error 
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            return pp;
        }
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
                        string[] paths = { Environment.CurrentDirectory, "Content", "Audio", Path.GetFileName(filePath) };
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
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end

            return newPath;
        }
    }
}
