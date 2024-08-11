using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OgTech.Api.Errors;
using OgTech.Api.Helper;
using OgTech.Controllers;
using OgTech.Core.Entities;
using OgTech.Core.Repo;
using OgTech.Repository.Identity;
using System.IO.Compression;

namespace OgTech.Api.Controllers
{

    public class ImagesController : ApiBaseController
    {
        private readonly IGenericRepository<Image> _imageRepository;

        public ImagesController( IGenericRepository<Image> imageRepository)
        {
            _imageRepository = imageRepository;
        }


        [HttpPost("upload-zip")]
        public async Task<ActionResult> GetZipFile(IFormFile zipFile)
        {
            bool result;

            if (zipFile == null || zipFile.Length == 0)
            {
                return BadRequest("The uploaded file is empty:(");
            }

            // Create Temp Folder Path With Guid
            var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedImages", Guid.NewGuid().ToString());

            // Create Temp Folder
            Directory.CreateDirectory(tempFolder);

            // Create Temp Zip File Path
            var zipFilePath = Path.Combine(tempFolder, zipFile.FileName);
           
            try
            {

                // Copy this ZipFile in this Path (zipFilePath)
                await using (var fs = new FileStream(zipFilePath, FileMode.Create))
                {
                    await zipFile.CopyToAsync(fs);
                }

                // I will UnZip the File in this Path
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, tempFolder);

                // After that I will Remove this File
                System.IO.File.Delete(zipFilePath);

                // I will Upload Files Here in temp folder
                result = await UploadFilesInBatches(tempFolder);

                // Check If this Process was successful or not
                if (result)
                {
                    // Check existing of this Directory
                    if (Directory.Exists(tempFolder))
                    {
                        // Delete the file where I save last file
                        var lastFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedImages", "LastFile.txt");
                        if (System.IO.File.Exists(lastFile))
                        {
                            System.IO.File.Delete(lastFile);
                        }

                        // Delete the Directory
                        Directory.Delete(tempFolder, true);
                    }

                    // Return Files Uploaded.
                    return Ok("Files Uploaded.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(500, "An Error happened: " + ex.Message));
            }

            // Return Files Uploaded if the Process was Successful :)
            if (!result)
            {
                return Ok("Files Uploaded.");
            }
            // Return (Files were not Uploaded.) if the Process was not Successful :(
            return Ok("Files were not Uploaded.");
        }
        

       

        private async Task<bool> UploadFilesInBatches(string tempFolder)
        {
            // Counter To check if all files were uploaded or not
            int cnt = 0;

            // Files I will upload
            var files = Directory.GetFiles(tempFolder).OrderBy(o => o).ToList();
            try
            {
                // Batch Size, 10 files per batch
                int batchSize = 10;

                // Get the last file to continue next
                var lastFile = await GetLastFileNameAsync();

                // The beginning where I will continue to upload
                int theBeginning = 0;

                if (!string.IsNullOrEmpty(lastFile))
                {
                    // Get the last file position 
                    theBeginning = files.FindIndex(F => Path.GetFileName(F) == lastFile);

                    // Set the beginning where I will start
                    if (theBeginning == -1)
                    {
                        cnt = 0;
                        theBeginning = 0;
                    }
                    else
                    {
                        cnt = theBeginning;
                        theBeginning++;
                    }
                   
                }


                for (int i = theBeginning; i < files.Count; i += batchSize)
                {
                    var batches = files.Skip(i).Take(batchSize);

                    foreach (var file in batches)
                    {
                        cnt++;

                        await FilesSettings.UploadFilesAsync(file, _imageRepository);

                        await SaveLastFileName(Path.GetFileName(file));
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return false;
            }
            return cnt == files.Count;
        }


        private async Task SaveLastFileName(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedImages", "LastFile.txt");
            await System.IO.File.WriteAllTextAsync(path,fileName);
        }

        private async Task<string> GetLastFileNameAsync()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedImages", "LastFile.txt");
            if (!System.IO.File.Exists(path))
            {
                return string.Empty;
            }

            return await System.IO.File.ReadAllTextAsync(path);
        }


        /*
        [HttpPost("upload-images")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<string>>> UploadImage(List<IFormFile> files)
        {
            if (files is null)
            {
                return BadRequest(new ApiResponse(400, "No files uploaded."));
            }
            try
            {
                var fileNames = await FilesSettings.UploadFilesAsync(files, _imageRepository);

                return Ok(fileNames);
            }
            catch (Exception)
            {
                return BadRequest(new ApiResponse(500, "An error occurred while uploading files"));

            }

        }*/
    }
}
