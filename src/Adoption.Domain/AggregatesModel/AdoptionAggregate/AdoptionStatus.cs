using System.Text.Json.Serialization;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AdoptionStatus
{
    Pending,
    Approved,
    Rejected,
    Completed 
}
