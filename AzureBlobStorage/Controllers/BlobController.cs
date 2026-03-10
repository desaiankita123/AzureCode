using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace AzureBlobStorage.Controllers
{
    public class BlobController : Controller
    {
        private readonly string _connectionString;
        private readonly BlobContainerClient _containerClient;
        private readonly string _containerName;


        public BlobController(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            var containerName = configuration.GetConnectionString("AzureBlobStorageContainer");
            _containerClient = new BlobContainerClient(
                configuration.GetConnectionString("AzureBlobStorage"),
                containerName);
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file )
        {
            try
            {
                if (file == null & file.Length == 0)
                    return BadRequest("Please Select The File");

               await _containerClient.CreateIfNotExistsAsync();

                string blobName = file.FileName;
                BlobClient blobClient = _containerClient.GetBlobClient(blobName);

                using var Stream = file.OpenReadStream();
                await blobClient.UploadAsync(Stream,overwrite:true);

                return Ok(new {
                    Message="File Uploaded Successfully",
                    FileName = blobName,
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    Message = "Upload failed!",
                    Error = ex.Message
                });
            }
        }

        //download the file
        [HttpPost("DownLoadFile")]
        public async Task<IActionResult> DownLoadFile(string FileName)
        {
            try
            {

                if (string.IsNullOrEmpty(FileName))
                    return BadRequest("Please provide a file name.");

                BlobClient blobClient = _containerClient.GetBlobClient(FileName);

                if (!await blobClient.ExistsAsync())
                    return NotFound("File not found!");

                var download = await blobClient.DownloadContentAsync();
                return File(download.Value.Content.ToStream(), "application/octet-stream", FileName);


            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete("deleteFile")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return BadRequest("Please provide a file name.");

                BlobClient blobClient = _containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                    return NotFound("File not found!");

                await blobClient.DeleteAsync();
                return Ok(new { Message = "File deleted successfully!", FileName = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Delete failed!", Error = ex.Message });
            }
        }

        [HttpGet("listOfBlobs")]
        public async Task<IActionResult> ListOfBlobs()
        {
            try
            {
                var blobs = new List<string>();
                await foreach (var blobItem in _containerClient.GetBlobsAsync())
                {
                    blobs.Add(blobItem.Name);
                }
                return Ok(new { Files = blobs });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new 
                {
                    Message = "Failed!",
                    Error = ex.Message

                });
            }
        }
    }
}
