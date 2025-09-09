using ai_summarize.Model;
using OllamaSharp;
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
        internal OllamaService(Uri uri)
        {
            _uri = uri;
            _ollama = new OllamaApiClient(_uri);
        }

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
            finally
            {
                Dispose();
            }
           
        }

        internal void ChatAsync()
        {

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

    }
}
