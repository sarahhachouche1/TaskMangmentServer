namespace Library
{
    public class FileWatcher
    {
        private readonly FileSystemWatcher _fileSystemWatcher;

        public delegate Task FileChangedDelegate(string content);
        public event FileChangedDelegate OnFileChanged;

        public FileWatcher(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(path));
            }

            _fileSystemWatcher = new FileSystemWatcher(path);

            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.InternalBufferSize = 32_768;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName; ;

            _fileSystemWatcher.Created += async (sender, e) => await OnChangedAsync(sender, e);
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private async Task OnChangedAsync(object sender, FileSystemEventArgs e)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        string content = await File.ReadAllTextAsync(e.FullPath);
                        OnFileChanged?.Invoke(content);
                        break;
                    }
                    catch (IOException ex)
                    {
                        Console.Write(ex);
                        await Task.Delay(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }

}

