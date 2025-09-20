# ai_summarize

Drag-and-drop text file summarizer powered by local AI models.

This project lets you drop a `.txt` file into the dashboard to generate a concise summary using an Ollama-hosted model. By default it targets the `granite3.2:8b` model, but the configuration is flexible so you can experiment with other local models.

## Requirements
- Running [Ollama](https://ollama.com/) instance on your machine or inside a local container.
- Installed `granite3.2:8b` (or another compatible) model in Ollama.
- Windows environment with .NET for building and running the WPF application.

## Features
- Drag-and-drop interface for uploading text files.
- Sends file content to your local Ollama model for summarization.
- Designed to run entirely on your hardware for privacy and control.

## Getting Started
1. Install and start Ollama locally (or in a container) and pull the desired model:
   ```bash
   ollama pull granite3.2:8b
   ```
2. Ensure the Ollama API endpoint is reachable (default: `http://localhost:11434`).
3. Open the solution in Visual Studio or run:
   ```bash
   dotnet build
   dotnet run
   ```
4. Drag a text file into the app window and wait for the generated summary.

## Configuration
- **Model**: Change the default model name in the application settings if you prefer another Ollama model.
- **Hardware**: The app will attempt to use available CPU/GPU resources as exposed by Ollama. Future updates will include explicit detection and configuration.

## Roadmap
- [ ] Check CPU or GPU usage automatically.
- [ ] Support additional input formats (PDF, DOCX, etc.).
- [ ] Update the UI/UX design.
- [ ] Integrate a vector database for large file handling.

## Contributing
Pull requests and suggestions are welcome. Please open an issue for feature requests or bug reports.

## License
No
