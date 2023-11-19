using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Pets.API.Settings;
using Microsoft.Extensions.Logging;

namespace Pets.API.Services;

public interface IImageStorageService
{
    Task UploadPetImage(Guid petId, IFormFile imageFile, CancellationToken cancellationToken);
    Task GetImage(Guid petId, HttpContext context, CancellationToken cancellationToken);
    Task DeleteImage(Guid petId, CancellationToken cancellationToken);
}

public class ImageStorageService : IImageStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _imagesContainer;
    private readonly ILogger<ImageStorageService> _logger;
    private readonly BlobContainerClient _blobContainerClient;

    public ImageStorageService(IOptions<BlobStorageSettings> blobStorageSettings,
        ILogger<ImageStorageService> logger)
    {
        _imagesContainer = blobStorageSettings.Value.ImagesContainer;
        _logger = logger;
        try
        {
            _blobServiceClient = new BlobServiceClient(blobStorageSettings.Value.ConnectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_imagesContainer);
            _blobContainerClient.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error creating blob client");
        }
    }

    public async Task UploadPetImage(Guid petId, IFormFile imageFile, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"pets/{petId}");
        await using var fileStream = imageFile.OpenReadStream();

        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = imageFile.ContentType },
            cancellationToken: cancellationToken);
    }

    public async Task GetImage(Guid petId, HttpContext context, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"pets/{petId}");
        await blobClient.DownloadToAsync(context.Response.Body, cancellationToken);
    }

    public async Task DeleteImage(Guid petId, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"pets/{petId}");
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

    }
}