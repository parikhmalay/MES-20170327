using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Library.Helper
{
    public static class BlobHelper
    {
        static string blobPolicyName = "VEPolicy";

        #region Azure Storage
        /// <summary>
        /// Gets the BLOB container.
        /// </summary>
        /// <returns></returns>
        public static CloudBlobContainer GetBlobContainer(string containerName)
        {
            // Retrieve storage account from connection-string
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container 
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Create the container if it doesn't already exist
            container.CreateIfNotExists();
            BlobContainerPermissions blobPermissions = new BlobContainerPermissions();
            // The shared access policy provides 
            // read/write access to the container for 10 hours.
            //blobPermissions.SharedAccessPolicies.Add(blobPolicyName, new SharedAccessBlobPolicy()
            //{
            //    // To ensure SAS is valid immediately, don’t set start time.
            //    // This way, you can avoid failures caused by small clock differences.
            //    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
            //    Permissions = SharedAccessBlobPermissions.Write |
            //       SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Delete
            //});
            // The public access setting explicitly specifies that 
            // the container is private, so that it can't be accessed anonymously.
            blobPermissions.PublicAccess = BlobContainerPublicAccessType.Container;
            // Set the permission policy on the container.
            container.SetPermissions(blobPermissions);
            return container;
        }
        /// <summary>
        /// Uploads the file to BLOB.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="uploadFile">The upload file.</param>
        public static string UploadFileToBlob(string containerName, string fileUploadFolderName, string blobName, Stream uploadFile)
        {
            CloudBlobContainer container = GetBlobContainer(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUploadFolderName + blobName);
            blob.UploadFromStream(uploadFile);
            uploadFile.Close();
            return blob.Uri.ToString();
        }
        //Physical file to Blob
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName">azure blob container name from web.config</param>
        /// <param name="fileUploadFolderName">azure storage folder name</param>
        /// <param name="blobName">filename</param>
        /// <param name="path">physical file path</param>
        /// <returns></returns>
        public static string UploadFileToBlob(string containerName, string fileUploadFolderName, string blobName, string path)
        {
            CloudBlobContainer container = GetBlobContainer(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUploadFolderName + blobName);
            byte[] fileData = null;
            using (WebClient client = new WebClient())
            {
                fileData = client.DownloadData(path);
            }
            blob.UploadFromByteArray(fileData, 0, fileData.Length);
            return blob.Uri.ToString();
        }
        //Physical file to Blob
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string UploadFileToBlob(string containerName, string blobName, string path)
        {
            CloudBlobContainer container = GetBlobContainer(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            byte[] fileData = null;
            using (WebClient client = new WebClient())
            {
                fileData = client.DownloadData(path);
            }
            blob.UploadFromByteArray(fileData, 0, fileData.Length);
            return blob.Uri.ToString();
        }
        //Physical file to Blob
        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string UploadFileToBlob(string containerName, string fileUploadFolderName, string blobName, byte[] fileData)
        {
            CloudBlobContainer container = GetBlobContainer(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUploadFolderName + blobName);

            blob.UploadFromByteArray(fileData, 0, fileData.Length);
            return blob.Uri.ToString();
        }
        /// <summary>
        /// Uploads the file to BLOB.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="uploadFile">The upload file.</param>
        public static string UploadFileToBlob(CloudBlobContainer container, string blobName, Stream uploadFile)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            blob.UploadFromStream(uploadFile);
            return blob.Uri.ToString();
        }
        /// <summary>
        /// Gets the BLOB URL.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <returns></returns>
        public static string GetBlobUrl(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            string sasToken = container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), blobPolicyName);

            new Uri(blob.Uri.AbsoluteUri + sasToken);
            return blob.Uri.AbsoluteUri;
        }
        public static byte[] GetBlobStream(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);
            using (var memStream = new MemoryStream())
            {
                blob.DownloadToStream(memStream);
                return memStream.ToArray();
            }
        }
        public static byte[] GetBlobStreamByUrl(string containerName, string fileurl)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(GetFileName(containerName, fileurl));

            using (var memStream = new MemoryStream())
            {
                blob.DownloadToStream(memStream);
                return memStream.ToArray();
            }
        }
        public static bool CheckFileExists(string containerName, string fileurl)
        {
            var container = GetBlobContainer(containerName);
            return container.GetBlockBlobReference(GetFileName(containerName, fileurl)).Exists();
        }

        public static bool DeleteBlobfile(string containerName, string fileurl)
        {
            try
            {
                var container = GetBlobContainer(containerName);
                var blob = container.GetBlockBlobReference(GetFileName(containerName, fileurl));
                //var blob = GetBlobContainer(containerName).GetBlockBlobReference(fileurl);
                return blob.DeleteIfExists();
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string GetFileName(string containerName, string fileUrl)
        {
            // the file name will be blob name. if there is any folder in container then the blob name will be like folderName/DocumentName.fileType
            string filename = string.Empty;
            int isContainerNameMatched = 0;
            Uri u = new Uri(fileUrl);
            string OnlyFileName = string.Empty;
            if (!string.IsNullOrEmpty(fileUrl))
                OnlyFileName = Path.GetFileName(fileUrl);

            for (int i = 0; i < u.Segments.Length; i++)
            {
                if (isContainerNameMatched > 0 || u.Segments[i].ToUpper() == (containerName + "/").ToUpper())
                {
                    isContainerNameMatched = isContainerNameMatched + 1;
                    if (isContainerNameMatched > 1)
                    {
                        if (i != u.Segments.Length - 1)
                            filename = filename + u.Segments[i];
                        else
                            filename = filename + OnlyFileName;
                    }
                }
            }


            return filename;
        }

        public static Stream AttachFile_ConvertBlobToStream(string FilePath)
        {
            string containerName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]);
            byte[] bytes = MES.Business.Library.Helper.BlobHelper.GetBlobStream(containerName, GetFileName(containerName, FilePath));
            Stream fileStream = new MemoryStream(bytes);
            return fileStream;
        }

        public static string GetPhysicalPath(string fileUrl)
        {
            string containerName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BlobContainerName"]);
            return "/" + GetFileName(containerName, fileUrl);
        }

        #endregion

    }
}
