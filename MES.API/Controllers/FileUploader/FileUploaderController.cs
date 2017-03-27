using MES.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace MES.API.Controllers.FileUploader
{

    [AdminPrefix("FileUploaderApi")]
    public class FileUploaderApiController : ApiControllerBase
    {
        string MESContainer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]); //MES.Business.Library.Constants.MESContainer.ToString();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("NPE.Web.Restrictions", "NPE0002:HttpGetPostAttributeFound"), HttpPost]
        public Task<List<FileDetails>> Upload(string folderName)
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            if (string.IsNullOrEmpty(folderName) || folderName == "undefined" || folderName == "null" || folderName.IndexOf(" ") > -1)
            {
                folderName = "";
            }
            else
                folderName = folderName + "/";

            var multipartStreamProvider = new AzureBlobStorageMultipartProvider(MESContainer, folderName);
            return Request.Content.ReadAsMultipartAsync<AzureBlobStorageMultipartProvider>(multipartStreamProvider).ContinueWith<List<FileDetails>>(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                AzureBlobStorageMultipartProvider provider = t.Result;
                return provider.Files;
            });
        }

        [HttpDelete]
        public void DeleteFile(string source)
        {
            MES.Business.Library.Helper.BlobHelper.DeleteBlobfile(MESContainer, source);
        }
        [HttpPost]
        public Task<List<FileDetails>> UploadSpecificSizeImage(int h, int w)
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string folderName = "";
            var multipartStreamProvider = new AzureBlobStorageMultipartProvider(MESContainer, folderName, h, w);
            return Request.Content.ReadAsMultipartAsync<AzureBlobStorageMultipartProvider>(multipartStreamProvider).ContinueWith<List<FileDetails>>(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                AzureBlobStorageMultipartProvider provider = t.Result;
                return provider.Files;
            });
        }
        public IHttpActionResult UploadEditedFile(string filePath)
        {
            string folderName = "";
            var multipartStreamProvider = new AzureBlobStorageMultipartProvider(MESContainer, folderName);
            return Ok(multipartStreamProvider.UploadEditedFile(filePath));
        }
    }

    public class FileDetails
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string Location { get; set; }
        public string FileName { get; set; }
    }

    public class AzureBlobStorageMultipartProvider : MultipartFileStreamProvider
    {
        string container = string.Empty;
        string fileUploadFolderName = string.Empty;
        int? height = null;
        int? width = null;

        public AzureBlobStorageMultipartProvider(string container, string folderName, int? height = null, int? width = null)
            : base(Path.GetTempPath())
        {
            this.container = container;
            this.height = height;
            this.width = width;
            this.fileUploadFolderName = folderName;
            Files = new List<FileDetails>();
        }

        public List<FileDetails> Files { get; set; }

        public override Task ExecutePostProcessingAsync()
        {
            // Upload the files to azure blob storage and remove them from local disk
            foreach (var fileData in this.FileData)
            {
                if (fileData.Headers.ContentDisposition.FileName != null)
                {
                    string fileName, origFileName;// = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
                    string uploadedpath;

                    origFileName = fileName = GetDeserializedFileName(fileData);

                    //string fileExtenstion = fileName.Substring(fileName.Length - (fileName.LastIndexOf('.') - 2));
                    string fileExtenstion = string.Empty;
                    int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
                    if (fileExtPos >= 0)
                        fileExtenstion = fileName.Substring(fileExtPos, fileName.Length - fileExtPos);

                    // Retrieve reference to a blob
                    String filePath = fileData.LocalFileName;
                    FileStream fileStream = new FileStream(filePath, FileMode.Open);

                    fileName = Guid.NewGuid() + fileExtenstion;

                    if (height != null && width != null)
                    {
                      
                        Size size = new Size(width ?? 0, height ?? 0);
                        fileStream.Close();
                        using (MemoryStream inStream = new MemoryStream())
                        {
                            using (Stream input = File.OpenRead(filePath))
                            {
                                input.CopyTo(inStream);
                            }
                            inStream.Position = 0;

                            using (MemoryStream outStream = new MemoryStream())
                            {
                                outStream.Position = 0;
                                uploadedpath = MES.Business.Library.Helper.BlobHelper.UploadFileToBlob(container, fileUploadFolderName, fileName, outStream);
                                // Do something with the stream.
                            }
                        }
                    }
                    else
                    {
                        uploadedpath = MES.Business.Library.Helper.BlobHelper.UploadFileToBlob(container, fileUploadFolderName, fileName, fileStream);
                        fileStream.Close();
                    }



                    File.Delete(fileData.LocalFileName);
                    Files.Add(new FileDetails
                    {
                        Location = uploadedpath,
                        Name = origFileName,
                        FileName = fileName
                    });
                }
            }

            return base.ExecutePostProcessingAsync();
        }


        private object GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            if (result.FormData.HasKeys())
            {
                var unescapedFormData = Uri.UnescapeDataString(result.FormData
                    .GetValues(0).FirstOrDefault() ?? String.Empty);
                if (!String.IsNullOrEmpty(unescapedFormData))
                    return JsonConvert.DeserializeObject<T>(unescapedFormData);
            }

            return null;
        }

        private string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        public String UploadEditedFile(string filePath)
        {
            string fileName = string.Empty;
            string fileExtenstion = Path.GetExtension(filePath);
            fileName = Guid.NewGuid() + fileExtenstion;
            string uploadedpath = MES.Business.Library.Helper.BlobHelper.UploadFileToBlob(container, fileName, filePath);
            return uploadedpath;
        }
    }
}
