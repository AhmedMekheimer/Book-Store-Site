using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Models.File_Entities
{
    public class FileService
    {
        private static readonly string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private static readonly string[] videoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".wmv" };

        public static (string, FileType) UploadNewFile(IFormFile NewFile)
        {
            if (NewFile is not null && NewFile.Length > 0)
            {
                // Save File in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NewFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);
                var extension = Path.GetExtension(NewFile.FileName).ToLowerInvariant();
                FileType fileType = FileType.Image;
                using (var stream = System.IO.File.Create(filePath))
                {
                    NewFile.CopyTo(stream);
                }
                if (imageExtensions.Contains(extension))
                {
                    fileType = FileType.Image;
                }
                else if (videoExtensions.Contains(extension))
                {
                    fileType = FileType.Video;
                }
                return (fileName, fileType);
            }
            else
                return (null, FileType.Image);
        }
    }
}

