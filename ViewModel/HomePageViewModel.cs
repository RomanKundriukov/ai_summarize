using ai_summarize.Model;
using ai_summarize.Service;
using ai_summarize.View;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace ai_summarize.ViewModel
{
    internal class HomePageViewModel : IDisposable, INotifyPropertyChanged
    {
        private bool disposed = false;

        #region Variables

        private string _summarizeText = "Drop your Dokument Hier";

        internal string SummarizeText
        {
            get => _summarizeText;
            set
            {
                if (_summarizeText != value)
                {
                    _summarizeText = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _isSummarized = Visibility.Collapsed;

        internal Visibility IsSummarized
        {
            get => _isSummarized;
            set
            {
                if (_isSummarized != value)
                {
                    _isSummarized = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion

        internal ObservableCollection<OllamaModels> models = new ObservableCollection<OllamaModels>();
        private readonly OllamaService _ollama = new OllamaService(new Uri("http://localhost:11434"));
        private readonly FileService _fileService = new FileService();


        internal HomePageViewModel()
        {
            LoadModels();

        }

        #region Aufräumen
        ~HomePageViewModel()
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
                models.Clear();
            }
            // unmanaged hier freigeben
            disposed = true;
        }
        #endregion

        /// <summary>
        /// Asynchronously loads the list of available models and updates the models collection.
        /// </summary>
        /// <remarks>If no models are found, a placeholder entry is added to indicate that no models are
        /// available. Any errors encountered during loading are reported via the application notification
        /// service.</remarks>
        private async void LoadModels()
        {
            try
            {
                List<OllamaModels> ollamaModels = await _ollama.GetModelsAsync();

                if (ollamaModels.Count <= 0)
                    models.Add(new OllamaModels { ModelName = "You don´t have a Models." });
                else
                {
                    foreach (var item in ollamaModels)
                    {
                        models.Add(new OllamaModels { ModelName = item.ModelName });
                    }
                }
            }
            catch (Exception ex)
            {
                models.Add(new OllamaModels { ModelName = "You don´t have a Models." });
                AppNotificationService.GetNotification("Error", ex.Message);
            }
        }

        private void ClearSummarizeText()
        {
            SummarizeText = string.Empty;
        }

        /// <summary>
        /// Handles the drop event for the grid, processing files that are dragged and dropped onto the control.
        /// </summary>
        /// <remarks>This method processes dropped files if the data package contains storage items. Only
        /// the first file in the collection is considered. This method is intended for internal use and should not be
        /// called directly.</remarks>
        /// <param name="sender">The source of the event, typically the grid control receiving the drop.</param>
        /// <param name="e">The event data containing information about the drag-and-drop operation.</param>
        internal async void Grid_Drop(object sender, DragEventArgs e)
        {
            // Check if the dropped data contains files
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // Get the file(s) from the DataPackage
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0 && items.Count == 1)
                {
                    // Display the first file's name
                    var storageFile = items[0] as StorageFile;
                    if (storageFile != null)
                    {
                        var format = storageFile.FileType.Replace(".", string.Empty);
                        //check File Format
                        if (!_fileService.IsFileFormatSupported(format))
                        {
                            AppNotificationService.GetNotification("Hinweis", "File Format ist momentan nicht unterschtützt");
                        }
                        else
                        {
                            await CreateSummarizeOneFile(storageFile.Path);
                        }
                    }
                }
                else
                {
                    AppNotificationService.GetNotification("Hinweis", "Please drop one file");
                }
            }
        }

        /// <summary>
        /// Handles the DragOver event for the grid to indicate that a copy operation is allowed during a drag-and-drop
        /// interaction.
        /// </summary>
        /// <param name="sender">The source of the event, typically the grid control where the drag operation is occurring.</param>
        /// <param name="e">A DragEventArgs object that contains the event data for the drag-and-drop operation.</param>
        internal void Grid_DragOver(object sender, DragEventArgs e)
        {
            // This is not being called
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        internal async Task CreateSummarizeOneFile(string path)
        {

            if (!string.IsNullOrWhiteSpace(_summarizeText))
                ClearSummarizeText();

            string fileText = await _fileService.ReadTxtFile(path);

            if (fileText.Length != 0 && fileText is not null)
            {
                try
                {

                    IsSummarized = Visibility.Visible;

                    await foreach (var chunk in _ollama.SummarizeFile(fileText: fileText))
                    {
                        SummarizeText += chunk;
                    }
                }
                catch (Exception ex)
                {
                    AppNotificationService.GetNotification("Hinweis", ex.Message);
                }
                finally
                {
                    IsSummarized = Visibility.Collapsed;

                }

            }
        }
    }
}