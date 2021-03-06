﻿//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;

//namespace Birder.Services
//{
//    public class LocalFileClient : IFileClient
//    {
//        private string _fileRoot;

//        public LocalFileClient(string fileRoot)
//        {
//            _fileRoot = fileRoot;
//        }

//        public Task DeleteFile(string storeName, string filePath)
//        {
//            var path = Path.Combine(_fileRoot, storeName, filePath);

//            if (File.Exists(path))
//            {
//                File.Delete(path);
//            }
//            return Task.CompletedTask;
//        }

//        public Task<bool> FileExists(string storeName, string filePath)
//        {
//            var path = Path.Combine(_fileRoot, storeName, filePath);

//            return Task.FromResult(File.Exists(path));
//        }

//        public Task<List<string>> GetAllFileUrl(string storeName)
//        {
//            var urls = new List<string>();
//            urls.Add("https://preview.ibb.co/jrsA6R/img12.jpg");
//            urls.Add("https://preview.ibb.co/jrsA6R/img12.jpg");
//            urls.Add("https://preview.ibb.co/jrsA6R/img12.jpg");
//            urls.Add("https://preview.ibb.co/jrsA6R/img12.jpg");
//            urls.Add("https://preview.ibb.co/jrsA6R/img12.jpg");
//            return Task.FromResult(urls);
//        }

//        public Task<Stream> GetFile(string storeName, string filePath)
//        {
//            var path = Path.Combine(_fileRoot, storeName, filePath);
//            Stream stream = null;
            
//            if (File.Exists(path))
//            {
//                stream = File.OpenRead(path);
//            }

//            return Task.FromResult(stream);
//        }

//        public Task<string> GetFileUrl(string storeName, string filePath)
//        {
//            //var path = Path.Combine(_fileRoot, storeName, filePath);
//            //return Task.FromResult<string>(path);
//            return Task.FromResult<string>(null);
//        }

//        public async Task SaveFile(string storeName, string filePath, Stream fileStream)
//        {
//            var folderPath = Path.Combine(_fileRoot, storeName);

//            Directory.CreateDirectory(folderPath);

//            var path = Path.Combine(_fileRoot, storeName, filePath);

//            if (File.Exists(path))
//            {
//                File.Delete(path);
//            }

//            using (var file = new FileStream(path, FileMode.CreateNew))
//            {
//                await fileStream.CopyToAsync(file).ConfigureAwait(false);
//            }
//        }
//    }
//}
    
