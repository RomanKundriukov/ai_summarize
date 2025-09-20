using ai_summarize.Model;
using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_summarize.Service
{
    internal class OllamaService : IDisposable
    {

        private bool disposed = false;
        private readonly Uri _uri;
        private readonly OllamaApiClient _ollama;
        private readonly PromptService _promptService = new PromptService();
        internal OllamaService(Uri uri)
        {
            _uri = uri;
            _ollama = new OllamaApiClient(_uri);
        }

        #region Aufräumen
        ~OllamaService()
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
                _ollama.Dispose();

            }
            // unmanaged hier freigeben
            disposed = true;
        }
        #endregion

        /// <summary>
        /// Laden alle Models from Ollama Container auf Localhost.
        /// </summary>
        /// <returns>Models Namen</returns>
        /// <exception cref="Exception"></exception>
        internal async Task<List<OllamaModels>> GetModelsAsync()
        {
            try
            {
                var models = await _ollama.ListLocalModelsAsync();

                return models
                    .Select(m => new OllamaModels { ModelName = m.Name })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal async IAsyncEnumerable<string> SummarizeFile(string fileText, string modelName = "granite3.2:8b", string promptKey = "documentSummary")
        {
            string prompt = _promptService.GetPromptByKey(promptKey);

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                // Hier Aufruf, der das Streaming ermöglicht
                await foreach (var stream in _ollama.GenerateAsync(new GenerateRequest
                {
                    Model = modelName,
                    Prompt = $"{prompt}:\n\n{fileText}",
                    Stream = true
                }))
                {

                    yield return stream?.Response ?? "Wow. Model generiert leere Daten";
                }
            }
            else
            {
                throw new Exception("Prompt ist leer");
            }


        }



    }
}
