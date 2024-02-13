using Domain.DTOs;
using Domain.Interfaces;
using System.Xml.Serialization;

namespace Infrastructure.Clients;

public class ExchangeRatesClient : IExchangeRatesClient
{
    private readonly HttpClient _httpClient;
    public ExchangeRatesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ExchangeRates> GetExchangeRatesByDateAsync(DateTime date)
    {
        var response = await _httpClient.GetAsync($"https://www.lb.lt/webservices/ExchangeRates/ExchangeRates.asmx/getExchangeRatesByDate?Date={date}");
        var rates = await response.Content.ReadAsStringAsync();
        return DeserializeXml<ExchangeRates>(rates);
    }
    static T DeserializeXml<T>(string xmlString)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));

        using (StringReader reader = new StringReader(xmlString))
        {
            return (T)serializer.Deserialize(reader)!;
        }
    }

}
