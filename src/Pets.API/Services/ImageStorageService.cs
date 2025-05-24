using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Pets.API.Settings;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Pets.API.Responses.Dtos;
using static System.Net.Mime.MediaTypeNames;

namespace Pets.API.Services;

public interface IImageStorageService
{
    Task UploadPetImage(Guid petId, bool isProfileImage, IFormFile imageFile, CancellationToken cancellationToken);
    Task GetImage(Guid petId, bool isProfileImage, HttpContext context, CancellationToken cancellationToken);
    Task<List<PetImageDto>> GetListOfPetImages(Guid petId, CancellationToken cancellationToken);
    Task DeleteAllPetImages(Guid petId, CancellationToken cancellationToken);
    Task DeleteImage(Guid imaageId, CancellationToken cancellationToken);
    Task GetImage(Guid imageId, HttpContext context, CancellationToken cancellationToken);
}

public class ImageStorageService : IImageStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _imagesContainer;
    private readonly ILogger<ImageStorageService> _logger;
    private readonly BlobContainerClient _blobContainerClient;

    private const string IS_PROFILE_IMAGE_TAG = "isProfileImage";

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

    public async Task UploadPetImage(Guid petId, bool isProfileImage, IFormFile imageFile, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"pets/{Guid.NewGuid()}");
        await using var fileStream = imageFile.OpenReadStream();

        BlobUploadOptions options = new BlobUploadOptions();
        options.Tags = new Dictionary<string, string>
        {
            { "petId", petId.ToString() },
            { IS_PROFILE_IMAGE_TAG, isProfileImage.ToString() }
        };
        options.HttpHeaders = new BlobHttpHeaders { ContentType = imageFile.ContentType };

        await blobClient.UploadAsync(fileStream,
            options,
            cancellationToken: cancellationToken);
    }

    public async Task<List<PetImageDto>> GetListOfPetImages(Guid petId, CancellationToken cancellationToken)
    {
        string query = $@"""petId"" = '{petId}'";

        var imageList = new List<PetImageDto>();

        await foreach (TaggedBlobItem taggedBlobItem in _blobContainerClient.FindBlobsByTagsAsync(query, cancellationToken))
        {
            imageList.Add(new PetImageDto
            {
                ImageName = taggedBlobItem.BlobName,
                IsProfileImage = taggedBlobItem.Tags.Any(x => x.Key == IS_PROFILE_IMAGE_TAG && x.Value == true.ToString())
            });
        }

        return imageList;
    }

    public async Task GetImage(Guid imageId, HttpContext context, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"pets/{imageId}");
        await blobClient.DownloadToAsync(context.Response.Body, cancellationToken);
    }

    public async Task GetImage(Guid petId, bool isProfileImage, HttpContext context, CancellationToken cancellationToken)
    {
        string query = $@"""petId"" = '{petId}' AND ""{IS_PROFILE_IMAGE_TAG}"" = '{isProfileImage}'";

        await foreach (TaggedBlobItem taggedBlobItem in _blobContainerClient.FindBlobsByTagsAsync(query, cancellationToken))
        {
            var blobClient = _blobContainerClient.GetBlobClient(taggedBlobItem.BlobName);
            await blobClient.DownloadToAsync(context.Response.Body, cancellationToken);
            break;
        }
    }

    public async Task DeleteAllPetImages(Guid petId, CancellationToken cancellationToken)
    {
        string query = $@"""petId"" = '{petId}'";

        await foreach (TaggedBlobItem taggedBlobItem in _blobContainerClient.FindBlobsByTagsAsync(query, cancellationToken))
        {
            await _blobContainerClient.DeleteBlobIfExistsAsync(taggedBlobItem.BlobName);
        }
    }

    public async Task DeleteImage(Guid imageId, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"pets/{imageId}");
        await blobClient.DeleteIfExistsAsync();
    }
}