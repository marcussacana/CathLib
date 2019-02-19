using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CathLib {
    public class XML {
        
        XmlDocument Document;
        public XML(byte[] Script) {
            Document = new XmlDocument();
            Document.Load(XmlReader.Create(new MemoryStream(Script)));
        }

        public string[] Import() {
            List<string> Strings = new List<string>();
            var Entries = Document.DocumentElement;
            foreach (var Element in Entries.GetElementsByTagName("Replace").Cast<XmlNode>()) {
                    string Value = Element.FirstChild.InnerXml;
                    Strings.Add(HttpUtility.HtmlDecode(Value));
            }
            
            return Strings.ToArray();
        }

        public byte[] Optimize() {
            var Entries = Document.DocumentElement;
            foreach (var Child in Entries.ChildNodes.Cast<XmlNode>()) {
                string Match = string.Empty;
                foreach (var Entry in Child.ChildNodes.Cast<XmlNode>()) {
                    if (Entry.Name == "Match")
                        Match = Entry.FirstChild.InnerXml;
                    if (Entry.Name == "Replace") {
                        if (Match == Entry.FirstChild.InnerXml)
                            Child.RemoveAll();
                        break;
                    }
                }
            }

            foreach (var Element in Entries.GetElementsByTagName("Match").Cast<XmlNode>())
                Element.FirstChild.InnerXml = Element.FirstChild.InnerXml.Replace("\n", ":[BREAKLINE]:");
            foreach (var Element in Entries.GetElementsByTagName("Replace").Cast<XmlNode>())
                Element.FirstChild.InnerXml = Element.FirstChild.InnerXml.Replace("\n", ":[BREAKLINE]:");
            

            var Elements = (from x in Entries.ChildNodes.Cast<XmlNode>() where x.HasChildNodes orderby 
                            (from y in x.ChildNodes.Cast<XmlNode>() where y.Name == "Match" select y).Single().InnerXml.Length 
                            descending select x).ToArray();

            Entries.RemoveAll();
            
            foreach (var Elem in Elements)
                Entries.AppendChild(Elem);


            using (MemoryStream Stream = new MemoryStream())
            using (XmlWriter Writer = XmlWriter.Create(Stream, WSettings)) {
                Document.Save(Writer);

                string XML = Encoding.UTF8.GetString(Stream.ToArray());

                return Encoding.UTF8.GetBytes(XML.Replace(":[BREAKLINE]:", "&#xA;").Replace("\r\n  <Patch></Patch>", ""));
            }

        }


        XmlWriterSettings WSettings = new XmlWriterSettings() {
            Indent = true,
            NewLineHandling = NewLineHandling.Entitize
        };
        public byte[] Export(string[] Strings) {
            int i = 0;

            var Entries = Document.DocumentElement;
            foreach (var Element in Entries.GetElementsByTagName("Match").Cast<XmlNode>())
                Element.FirstChild.InnerXml = Element.FirstChild.InnerXml.Replace("\n", ":[BREAKLINE]:");
            foreach (var Element in Entries.GetElementsByTagName("Replace").Cast<XmlNode>()) 
                Element.FirstChild.InnerXml = HttpUtility.HtmlEncode(Strings[i++]).Replace("\n", ":[BREAKLINE]:");


            using (MemoryStream Stream = new MemoryStream())
            using (XmlWriter Writer = XmlWriter.Create(Stream, WSettings)) {
                Document.Save(Writer);

                string XML = Encoding.UTF8.GetString(Stream.ToArray());

                return Encoding.UTF8.GetBytes(XML.Replace(":[BREAKLINE]:", "&#xA;"));
            }
        }
    }
}
