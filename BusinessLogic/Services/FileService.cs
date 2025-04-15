using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services;

public class FileService
{
    public static async Task<ServiceResult<string>> UploadImageAsync(IFormFile file, string folderPath)
    {
        if (file == null || file.Length == 0)
        {
            return new ServiceResult<string>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "File is empty or null."
            };
        }
        try
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return new ServiceResult<string>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = fileName
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<string>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = $"An error occurred while uploading the file: {ex.Message}"
            };
        }
    }
}
