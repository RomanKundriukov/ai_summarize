using ai_summarize.Model;
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

        internal HomePageViewModel() 
        {
            LoadModels();
        }
        private void LoadModels()
        {
            models.Add(new OllamaModels { ModelName = "Model 1"});
            models.Add(new OllamaModels { ModelName = "Model 2"});
            models.Add(new OllamaModels { ModelName = "Model 3"});
            
        }

        private string test = "test";

        internal string Test
        {
            get =>  test;
            set => test = value;    
        }
    }
}
