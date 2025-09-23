using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UglyToad.PdfPig;
using Windows.Storage;

namespace ai_summarize.Service
{
    internal delegate string ReadFile(string path);
    internal class FileService : IDisposable
    {
        private bool disposed = false;
        public FileService()
        {
        }

        #region Aufräumen

        ~FileService()
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


        /// <summary>
        /// Prüfft, ob File Format unterstützt wird. Alle File Formats sind in der FileFormat.cs in eineem Enum gespeichert.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <returns>true oder false</returns>
        internal bool IsFileFormatSupported(string fileFormat)
        {
            if (string.IsNullOrWhiteSpace(fileFormat))
                return false;

            return Enum.TryParse<FileFormat>(fileFormat, ignoreCase: true, out _);

        }

        public ReadFile GetReadFileDelegate(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            return ext switch
            {
                ".txt" => ReadTxtFile,
                ".docx" => ReadDocxFile,
                ".pdf" => ReadPdfFile,
                ".xlsx" => ReadXlsxFile,
                _ => throw new NotSupportedException($"Dateityp {ext} nicht unterstützt.")
            };
        }

        /// <summary>
        /// Lesen ein txt File asynchron
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string</returns>
        private static string ReadTxtFile(string filePath)
        {
            var sb = new StringBuilder();
            using var reader = new StreamReader(filePath);

            try
            {
                string? line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading file: {ex.Message}", ex);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return sb.ToString();
        }
        private static XDocument GetX(OpenXmlPart part)
        {
            using var s = part.GetStream();
            return XDocument.Load(s);
        }

        private static string ReadDocxFile(string filePath)
        {
            try
            {
                using var doc = WordprocessingDocument.Open(filePath, false);
                XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

                var main = doc.MainDocumentPart ?? throw new InvalidOperationException("MainDocumentPart not found.");

                // Hauptdokument laden
                var xdoc = GetX(main);
                var parts = new List<string>(capacity: 1 << 10);

                // Body-Text
                parts.Add(string.Concat(xdoc.Descendants(w + "t").Select(t => (string)t)));

                // (Optional) Kopf-/Fußzeilen mitnehmen
                foreach (var hp in main.HeaderParts)
                    parts.Add(string.Concat(GetX(hp).Descendants(w + "t").Select(t => (string)t)));

                foreach (var fp in main.FooterParts)
                    parts.Add(string.Concat(GetX(fp).Descendants(w + "t").Select(t => (string)t)));

                return string.Join(Environment.NewLine, parts);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private static string ReadXlsxFile(string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                    using var wb = new XLWorkbook(filePath);

                foreach (var ws in wb.Worksheets)
                {
                    foreach (var row in ws.RangeUsed()?.RowsUsed() ?? Enumerable.Empty<IXLRangeRow>())
                    {
                        var line = string.Join("\t", row.Cells().Select(c => c.GetFormattedString()));
                        if (!string.IsNullOrEmpty(line)) sb.AppendLine(line);

                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private static string ReadPdfFile(string filePath)
        {
            try
            {
                using var pdf = PdfDocument.Open(filePath);
                var parts = pdf.GetPages().Select(p => p.Text);

                string text =  string.Join(Environment.NewLine + Environment.NewLine, parts);

                if (!string.IsNullOrWhiteSpace(text)) return text;
                else throw new Exception("No text was found in the PDF. Please upload a PDF that contains text.");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }


}

