namespace Library;
public interface IFileHandler
{
    public Task<string> ReadFileAsync(string filePath);
    public Task WriteFileAsync(string filePath, string text);
}
public class FileHandler : IFileHandler
{
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    public async Task<string> ReadFileAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            await _semaphore.WaitAsync();
            try
            {
                return await File.ReadAllTextAsync(filePath);
                
            }
            finally
            {
                _semaphore.Release();
            }
        }
       else throw new FileNotFoundException();
    }
    public async Task WriteFileAsync(string filePath, string text)
    {
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        try
        {
            await _semaphore.WaitAsync();
            await File.WriteAllTextAsync(filePath, text);
        }
        finally
        {
            _semaphore.Release();
        }


    }
}

