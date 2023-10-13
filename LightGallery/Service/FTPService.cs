using FluentFTP;
using LightGallery.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LightGallery.Service;

public interface IFTPService
{
    public Task UploadFile(GalleryFile galleryFile, Stream fileStream);
    public Task<Stream> GetFile(GalleryFile galleryFile);
    public string GetFileRemotePath(GalleryFile galleryFile);
}

public class FTPService : IFTPService
{
    private IConfiguration _config;

    private readonly string _username;
    private readonly string _password;
    private readonly string _host;
    private readonly int _port;
    

    public FTPService(IConfiguration config)
    {
        _config = config;
        
        _username = _config["FTP:UserName"];
        _password= _config["FTP:Password"];
        _host = _config["FTP:Host"];
    }

    public string GetFileRemotePath(GalleryFile galleryFile)
    {
        return $"./{galleryFile.IdGallery}/{galleryFile.FolderIndex:000000}/{galleryFile.Id}.{galleryFile.Extension}";
    }
    
    public async Task UploadFile(GalleryFile galleryFile, Stream fileStream)
    {
        var client = new AsyncFtpClient(_host, _username, _password, 21);
        await client.AutoConnect();

        await client.UploadStream(fileStream, GetFileRemotePath(galleryFile), FtpRemoteExists.Overwrite, true);
    }


    public async Task<Stream> GetFile(GalleryFile galleryFile)
    {
        var client = new AsyncFtpClient(_host, _username, _password, 21);
        await client.AutoConnect();
        
        return await client.OpenRead(GetFileRemotePath(galleryFile));
    }
}