namespace IPLAuctionSystem.Infrastructure.Services;

using IPLAuctionSystem.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
    {
        var uploadPath = Path.Combine(_env.WebRootPath, folder);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(uploadPath, uniqueFileName);

        using var fileStreamOut = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOut);

        return uniqueFileName;
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('~', '/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }
        return false;
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('~', '/'));
        return Task.FromResult(File.Exists(fullPath));
    }
}
