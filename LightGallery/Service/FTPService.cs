using FluentFTP;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LightGallery.Service;

public interface IFTPService
{
    public Task<Stream> GetFile(Stream s, string fileName);
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


    public async Task<Stream> GetFile(Stream fileStream, string fileName)
    {
        var client = new AsyncFtpClient(_host, _username, _password, 21);
        await client.AutoConnect();

        var b = await client.GetWorkingDirectory();
        var a = await client.GetFileSize("./testImage.jpg");
        
        var result = await client.DownloadStream(fileStream, "testImage.jpg");
        

        return fileStream;
    }
}