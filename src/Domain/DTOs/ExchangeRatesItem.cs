using System.Xml.Serialization;

namespace Domain.DTOs;

[Serializable]
[XmlRoot("item")]
public class ExchangeRateItem
{
    [XmlElement("date")]
    public string Date { get; set; } = string.Empty;
    [XmlElement("currency")]
    public string Currency { get; set; } = string.Empty;
    [XmlElement("quantity")]
    public int Quantity { get; set; }
    [XmlElement("rate")]
    public decimal Rate { get; set; }
    [XmlElement("unit")]
    public string Unit { get; set; } = string.Empty;

}
[XmlRoot("ExchangeRates")]
public class ExchangeRates
{
    [XmlElement("item")]
    public List<ExchangeRateItem>? Rates { get; set; }

    public static implicit operator List<object>(ExchangeRates v)
    {
        throw new NotImplementedException();
    }
}
