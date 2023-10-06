// using AutoMapper;
// using LightGallery.Models;
// using LightGallery.Models.Results;
// using Microsoft.EntityFrameworkCore;
//
// namespace LightGallery.Service;
//
// public interface IFileService
// {
//     public Task<Stream> GetFile(Guid idGalleryFile);
// }
//
// public class EndoraFileService : IFTPService
// {
//     private IConfiguration _config;
//
//     private readonly string _username;
//     private readonly string _password;
//     private readonly string _host;
//     private readonly int _port;
//     
//
//     public EndoraFileService(IConfiguration config)
//     {
//         _config = config;
//         
//         _username = _config["FTP:UserName"];
//         _password= _config["FTP:Password"];
//         _host = _config["FTP:Host"];
//     }
//
//
//     public async Task<Stream> GetFile(Guid idGalleryFile)
//     {
//         
//         try
//         {
//             if (_ftpClient.FileExists(remoteFilePath))
//             {
//                 Stream ftpStream = _ftpClient.OpenRead(remoteFilePath);
//                 return File(ftpStream, "application/octet-stream", filename);
//             }
//             else
//             {
//                 return NotFound("File not found on FTP server.");
//             }
//         }
//
//         return fileStream;
//     }
// }