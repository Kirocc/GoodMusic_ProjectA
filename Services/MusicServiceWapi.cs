﻿using System;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Newtonsoft.Json;

namespace Services;

public class MusicServiceWapi : IMusicService
{
    private readonly ILogger<MusicServiceWapi> _logger;
    private readonly HttpClient _httpClient;

    //To ensure Json deserializern is using the class implementations instead of interfaces 
    private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        Converters = {
            new AbstractConverter<MusicGroup, IMusicGroup>(),
            new AbstractConverter<Album, IAlbum>(),
            new AbstractConverter<Artist, IArtist>()
        },
    };

    #region constructors
    public MusicServiceWapi(IHttpClientFactory httpClientFactory, ILogger<MusicServiceWapi> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(name: "MusicWebApi");
    }
    #endregion

    #region Admin Services
    public async Task<GstUsrInfoAllDto> InfoAsync()
    {
        string uri = $"admin/info";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        response.EnsureSuccessStatusCode();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var info = JsonConvert.DeserializeObject<GstUsrInfoAllDto>(s);
        return info;
    }


    public async Task<GstUsrInfoAllDto> SeedAsync(int nrOfItems) 
    {
        string uri = $"admin/seed?count={nrOfItems}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        response.EnsureSuccessStatusCode();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var info = JsonConvert.DeserializeObject<GstUsrInfoAllDto>(s);
        return info;
    }
    public async Task<GstUsrInfoAllDto> RemoveSeedAsync(bool seeded)
    {
        string uri = $"admin/removeseed?seeded={seeded}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        response.EnsureSuccessStatusCode();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var info = JsonConvert.DeserializeObject<GstUsrInfoAllDto>(s);
        return info;
    }
    #endregion

    #region MusicGroup CRUD
    public async Task<RespPageDto<IMusicGroup>> ReadMusicGroupsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) 
    {
        string uri = $"musicgroup/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        response.EnsureSuccessStatusCode();

        //Get the response data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<RespPageDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }
    public async Task<IMusicGroup> ReadMusicGroupAsync(Guid id, bool flat)
    {
        string uri = $"musicgroup/Readitem?id={id}&flat={flat}";
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        response.EnsureSuccessStatusCode();

        string ResponseObject = await response.Content.ReadAsStringAsync();

        var resp = JsonConvert.DeserializeObject<IMusicGroup>(ResponseObject, _jsonSettings);

        return resp;
    }
    public async Task<IMusicGroup> DeleteMusicGroupAsync(Guid id)
    {
       var apiUrl = $"https://seido-webservice-307d89e1f16a.azurewebsites.net/api/MusicGroup/DeleteItem/{id}";

    var response = await _httpClient.DeleteAsync(apiUrl);

    if (!response.IsSuccessStatusCode)
    {
        var errorMessage = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException($"Item with id {id} not found.");
        }
        throw new Exception($"Failed to delete item {id}. Status Code: {response.StatusCode}, Reason: {errorMessage}");
    }

    // If the item was deleted successfully, return the response content
    var responseContent = await response.Content.ReadAsStringAsync();
    var deletedItem = JsonConvert.DeserializeObject<IMusicGroup>(responseContent, _jsonSettings);

    return deletedItem;
    }
    public async Task<IMusicGroup> UpdateMusicGroupAsync(MusicGroupCUdto item)
    {
        var apiUrl = $"/api/MusicGroup/UpdateItem/{item.MusicGroupId}";

        var requestContent = new StringContent(
            JsonConvert.SerializeObject(item, _jsonSettings),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PutAsync(apiUrl, requestContent);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IMusicGroup>(responseContent, _jsonSettings);
    }
    public async Task<IMusicGroup> CreateMusicGroupAsync(MusicGroupCUdto item)
    {
        string url = $"MusicGroup/CreateItem"; 


        Console.WriteLine(item.ToString(), url);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, item);
        response.EnsureSuccessStatusCode();

        string ResponseObject = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<IMusicGroup>(ResponseObject, _jsonSettings);

        return resp;
    }
    #endregion

    #region Album CRUD      
    public async Task<RespPageDto<IAlbum>> ReadAlbumsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        string uri = $"album/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        response.EnsureSuccessStatusCode();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<RespPageDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<IAlbum> ReadAlbumAsync(Guid id, bool flat)
    {
        throw new NotImplementedException();
    }
    public async Task<IAlbum> DeleteAlbumAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    public async Task<IAlbum> UpdateAlbumAsync(AlbumCUdto item)
    {
        throw new NotImplementedException();
    }
    public async Task<IAlbum> CreateAlbumAsync(AlbumCUdto item)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Artist CRUD 
    public async Task<RespPageDto<IArtist>> ReadArtistsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        string uri = $"artist/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        response.EnsureSuccessStatusCode();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<RespPageDto<IArtist>>(s, _jsonSettings);
        return resp;
    }
    public async Task<IArtist> ReadArtistAsync(Guid id, bool flat)
    {
        throw new NotImplementedException();
    }
    public async Task<IArtist> DeleteArtistAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    public async Task<IArtist> UpdateArtistAsync(ArtistCUdto item)
    {
        throw new NotImplementedException();
    }
    public async Task<IArtist> CreateArtistAsync(ArtistCUdto item)
    {
        throw new NotImplementedException();
    }
    public async Task<IArtist> UpsertArtistAsync(ArtistCUdto item)
    {
        throw new NotImplementedException();
    }
    #endregion
    
}
public class AbstractConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract
{
    public override Boolean CanConvert(Type objectType)
        => objectType == typeof(TAbstract);

    public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        => serializer.Deserialize<TReal>(reader);

    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        => serializer.Serialize(writer, value);
}

