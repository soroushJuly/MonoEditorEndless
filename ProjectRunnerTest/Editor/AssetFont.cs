using System.Xml.Linq;
using System;
using System.Diagnostics;

namespace MonoEditorEndless.Editor
{
    public class AssetFont : Asset
    {
        // Argument-less constructor to make the serialization possible
        public AssetFont() : base()
        {
        }
        public AssetFont(string fontName, bool isUsedInGame = false) : base(fontName, "Font", isUsedInGame)
        {
            _importerDetails = "/importer:FontDescriptionImporter\r\n/processor:FontDescriptionProcessor\r\n/processorParam:PremultiplyAlpha=True\r\n/processorParam:TextureFormat=Compressed\r\n";
            CreateFontSprite();
            GenerateContentText();
            _assetType = AssetType.FONT;
        }
        private void CreateFontSprite()
        {
            // Create the XML document
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XComment("This file contains an xml description of a font, and will be read by the XNA"),
                new XComment("Framework Content Pipeline. Follow the comments to customize the appearance"),
                new XComment("of the font in your game, and to change the characters which are available to draw with."),
                new XElement("XnaContent",
                    new XAttribute(XNamespace.Xmlns + "Graphics", "Microsoft.Xna.Framework.Content.Pipeline.Graphics"),
                    new XElement("Asset",
                        new XAttribute("Type", "Graphics:FontDescription"),
                        new XComment("Modify this string to change the font that will be imported."),
                        new XElement("FontName", "PeaberryBase"),
                        new XComment("Size is a float value, measured in points. Modify this value to change the size of the font."),
                        new XElement("Size", 24),
                        new XComment("Spacing is a float value, measured in pixels. Modify this value to change the amount of spacing in between characters."),
                        new XElement("Spacing", 0),
                        new XComment("UseKerning controls the layout of the font. If this value is true, kerning information will be used when placing characters."),
                        new XElement("UseKerning", true),
                        new XComment("Style controls the style of the font. Valid entries are \"Regular\", \"Bold\", \"Italic\", and \"Bold, Italic\", and are case sensitive."),
                        new XElement("Style", "Regular"),
                        new XComment("If you uncomment this line, the default character will be substituted if you draw or measure text that contains characters which were not included in the font."),
                        // Uncomment the next line to include the DefaultCharacter element
                        // new XElement("DefaultCharacter", "*"),
                        new XComment("CharacterRegions control what letters are available in the font. Every character from Start to End will be built and made available for drawing. The default range is from 32, (ASCII space), to 126, ('~'), covering the basic Latin character set. The characters are ordered according to the Unicode standard. See the documentation for more information."),
                        new XElement("CharacterRegions",
                            new XElement("CharacterRegion",
                                new XElement("Start", "&#32;"),
                                new XElement("End", "&#126;")
                            )
                        )
                    )
                )
            );
            int dotIndex = _name.IndexOf(".");
            // Save the document to a file
            Debug.WriteLine(_name.Substring(dotIndex + 1));
            doc.Save("fontDescription.xml");

            Console.WriteLine("XML file created successfully.");
        }
    }
}
