# Pets API

## Settings
### Pets.Db
In settings we have the connection string, but most probably is different for each of us so worth moving it to secrets

```
{
  "ConnectionStrings": {
    "DefaultConnection": "server=(localdb)\\mssqllocaldb;database=PetsDB;trusted_connection=yes;MultipleActiveResultSets=True"
  }
}
```
### Pets.PI
Really we are just keeping two things in secrets at the moment, alongside the connection string that could (not consistent with the above) as the rest are on settings (either the develpment or the appsettings.json). An example of secrets file would be 

```
{
  "ConnectionStrings": { "DefaultConnection": "server=(localdb)\\mssqllocaldb;database=PetsDB;trusted_connection=yes;MultipleActiveResultSets=True" },
  "JWT": {
    "Secret": "{Insert here any key with 256 bit length}"
  },
  "SendGridApiKey": "{Send Grid API Key}",
  "GoogleGeocodingApiKey": "{Key for the Google Geocoding API}",
  "FeatureManagement": {
    "UseGeocodingApi": false
  }
}
```

### Geocoding API
When saving addresses we are calling Google Geocoding API for retrieving lat and long, based on the key above. However, to avoid passing the free limit we have a feature flag (UseGeocodingApi, as per the above appsettings.json) to enable it. If disabled, we just return a stubbed response that at the moment just picks up a random lat and long (rest of properties are not really used) within London.

### Azurite
For blob storage
```
npm install -g azurite
azurite --silent --location c:\azurite --debug c:\azurite\debug.log
```


## TODO

- [ ] Add social logins
- [ ] Seed roles
- [ ] Add right authorisation to endpoints
- [ ] Auto verify emails for users in development and set it back to required for non development
- [ ] Rework auth responses classes
- [ ] Add probes to healthcheck
- [ ] Remove wwwwroot
- [X] Normalize routes (/api?) - No api
- [ ] Policy authorisation vs role authorisation
- [ ] Remove StartUp
- [ ] Add Ip to refresh token?
