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

        /// <summary>
        /// Asynchronously generates a summary of the specified file text using the given language model and prompt key.
        /// </summary>
        /// <remarks>The summary is generated in a streaming fashion, allowing the caller to process each
        /// segment as it becomes available. This method is intended for internal use.</remarks>
        /// <param name="fileText">The full text content of the file to be summarized. Cannot be null or empty.</param>
        /// <param name="modelName">The name of the language model to use for summarization. Defaults to "granite3.2:8b" if not specified.</param>
        /// <param name="promptKey">The key identifying the prompt template to use for generating the summary. Defaults to "documentSummary" if
        /// not specified.</param>
        /// <returns>An asynchronous stream of strings containing segments of the generated summary. Each string represents a
        /// portion of the summary as it is produced.</returns>
        /// <exception cref="Exception">Thrown if the prompt associated with <paramref name="promptKey"/> is null, empty, or consists only of
        /// white-space characters.</exception>
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
