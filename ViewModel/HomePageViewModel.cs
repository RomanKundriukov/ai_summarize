using ai_summarize.Model;
using ai_summarize.Service;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_summarize.ViewModel
{
    internal class HomePageViewModel
    {
        internal ObservableCollection<OllamaModels> models = new ObservableCollection<OllamaModels>();
        private readonly OllamaService _ollama = new OllamaService(new Uri("http://localhost:11434"));

        internal HomePageViewModel() 
        {
            LoadModels();
           
        }
        private async void LoadModels()
        {
            List<OllamaModels> ollamaModels = await _ollama.GetModelsAsync();

            if(ollamaModels.Count <= 0)
                models.Add(new OllamaModels { ModelName = "You don´t have a Models." });
            else
            {
                foreach (var item in ollamaModels)
                {
                    models.Add(new OllamaModels { ModelName = item.ModelName });
                }
            }    

            
        }

        private string test = "test";

        internal string Test
        {
            get =>  test;
            set => test = value;    
        }
    }
}
