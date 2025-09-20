using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_summarize.Service
{
    delegate string ReadFileDelegate(byte[] file);
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

        /// <summary>
        /// Lesen ein txt File asynchron
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string</returns>
        internal async Task<string> ReadTxtFile(string filePath)
        {
            var sb = new StringBuilder();
            using var reader = new StreamReader(filePath);

            try
            {
                string? line = string.Empty;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            catch (Exception ex)
            {
                AppNotificationService.GetNotification("Hinweis", ex.Message);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return sb.ToString();
        }
    }


}

