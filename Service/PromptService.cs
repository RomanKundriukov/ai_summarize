using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ai_summarize.Service
{
    internal class PromptService : IDisposable
    {
        private static bool disposed = false;
        private static string _pathToPrompt = string.Empty;
        private static readonly XmlDocument _xml = new XmlDocument();
        internal PromptService()
        {
            IsPromptExist();
        }

        #region Aufräumen
        ~PromptService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                // managed freigeben
                

            }
            // unmanaged hier freigeben
            disposed = true;
        }
        #endregion

        internal void IsPromptExist()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            if (!Directory.Exists(Path.Combine(basePath, "Prompt")))
            {
                Directory.CreateDirectory(Path.Combine(basePath, "Prompt"));
            }

            if (!File.Exists(Path.Combine(basePath, "Prompt/Prompt.xml")))
            {
                CreatePromptXmlFile(Path.Combine(basePath, "Prompt/Prompt.xml"));
            }

            _pathToPrompt = Path.Combine(basePath, "Prompt/Prompt.xml");
        }

        /// <summary>
        /// Create basis Prompt XML File
        /// </summary>
        /// <param name="path"></param>
        /// <returns> true / false</returns>
        /// <exception cref="Exception"></exception>
        private bool CreatePromptXmlFile(string path)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                // XML-Deklaration
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(decl);

                // Root-Element
                XmlElement root = doc.CreateElement("prompts");
                doc.AppendChild(root);

                // <prompt>
                XmlElement prompt = doc.CreateElement("prompt");
                root.AppendChild(prompt);

                // <key>
                XmlElement key = doc.CreateElement("key");
                key.InnerText = "documentSummary";
                prompt.AppendChild(key);

                // <value>
                XmlElement value = doc.CreateElement("value");
                value.InnerText = "Fasse den folgenden Text prägnant zusammen.\n\n### Regeln:\n- Erkenne die Sprache des Textes automatisch.\n- Erstelle die Zusammenfassung **in derselben Sprache** wie der Originaltext.\n- Konzentriere dich nur auf die wichtigsten Punkte und Kernaussagen.\n- Keine zusätzlichen Kommentare, Erklärungen oder Hinweise zur Sprache.\n- Gib die Zusammenfassung als Fließtext zurück, ohne JSON oder andere Strukturen.\n\n### Eingabetext:\n";
                prompt.AppendChild(value);

                // Speichern
                doc.Save(path);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// get a Prompt by einem Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string</returns>
        /// <exception cref="Exception"></exception>
        internal string GetPromptByKey(string key)
        {
            try
            {
                _xml.Load(_pathToPrompt);
                XmlNode? node = _xml.SelectSingleNode($"/prompts/prompt[key='{key}']/value");
                if (node != null)
                {
                    return node.InnerText;
                }
                else
                {
                    return "No Prompt was found";
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            
        }
    }
}
