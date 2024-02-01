namespace Property.Core.Services;

public class ImageService(string storagePath)
{
    public async Task<string[]> SaveImages(IEnumerable<(Stream Image, string? Extension)> images)
    {
        var tasks = images
            .Where(i => i.Extension is not null)
            .Select(i => SaveImage(i.Image, i.Extension!));
        return await Task.WhenAll(tasks);
    }

    private async Task<string> SaveImage(Stream image, string extension)
    {
        var fileName = Guid.NewGuid().ToString("N") + "." + extension;
        var path = Path.Combine(storagePath, fileName);

        await using var fileStream = File.Create(path);
        await image.CopyToAsync(fileStream);
        await fileStream.FlushAsync();

        return fileName;
    }
}
