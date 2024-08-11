using OgTech.Core.Entities;
using OgTech.Core.Repo;
using System.IO;
using System.IO.Pipes;

namespace OgTech.Api.Helper
{
    public static class FilesSettings
    {
        public static async Task UploadFilesAsync(string FilePath,
                                                  IGenericRepository<Image> genericRepository
                                                  )
        {

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedImages");

            await using var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);


            var file = new FormFile(fs, 0, fs.Length, null, Path.GetFileName(FilePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };


            if (file is not null)
            {
                
                var fileExisting = await genericRepository.GetByNameAsync(file.FileName);

                if (fileExisting is not null)
                {
                    genericRepository.Delete(fileExisting);

                    DeleteFile(fileExisting.Path);
                }


                var parts = file.FileName.Split('-');

                var brandPath = Path.Combine(path, parts[0]);

                if (!Directory.Exists(brandPath))
                {
                    Directory.CreateDirectory(brandPath);
                }

                var modelPath = Path.Combine(brandPath, parts[1]);

                if (!Directory.Exists(modelPath))
                {
                    Directory.CreateDirectory(modelPath);
                }

                var folderPath = GetNextFolderPath(modelPath, 2);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, file.FileName);

                await using (var sf = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(sf);
                }

                var image = new Image()
                {

                    Name = file.FileName,
                    Path = filePath,
                };

                await genericRepository.AddAsync(image);
               
            }
          
        }

      
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static string GetNextFolderPath(string path, long _Max)
        {
            var folderNumber = 1;
            string folderPath;
            int fileCount;

            do
            {
                folderPath = Path.Combine(path, $"_{folderNumber}");
                fileCount = Directory.Exists(folderPath) ? Directory.GetFiles(folderPath).Length : 0;

                if (fileCount >= _Max)
                {
                    folderNumber++;
                }
            } while (fileCount >= _Max);

            return folderPath;
        }
    }
}
