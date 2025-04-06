using System.Text.Json.Serialization;

namespace lib.Models;

public class Company
{
    [JsonPropertyName("organisasjonsnummer")]
    public string OrganizationNumber { get; set; }

    [JsonPropertyName("navn")]
    public string Name { get; set; }

    [JsonPropertyName("registreringsdatoEnhetsregisteret")]
    public DateOnly DateRegistered { get; set; }

    [JsonPropertyName("registrertIMvaregisteret")]
    public bool VatRegistered { get; set; }
}