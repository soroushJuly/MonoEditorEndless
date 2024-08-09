using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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
    }
}
