
namespace BookShoppingCartMvcUI.Shared;

public class FileService(IWebHostEnvironment environment) : IFileService
{
    private readonly IWebHostEnvironment _environment = environment;

    public void DeleteFile(string fileName)
    {
        var wwwPath = _environment.WebRootPath;
        var fileNameWithPath = Path.Combine(wwwPath, "images\\", fileName);
        if (!File.Exists(fileNameWithPath))
            throw new FileNotFoundException(fileName);
        File.Delete(fileNameWithPath);
    }

    public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
    {
        var wwwPath = _environment.WebRootPath;
        var path = Path.Combine(wwwPath, "images");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        var extention = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(extention))
            throw new InvalidOperationException($"Only {string.Join(",",
                allowedExtensions)} files allowed");

        string fileName = $"{Guid.NewGuid()}{extention}";
        string fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }
}