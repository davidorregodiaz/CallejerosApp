using System.Text.Json.Serialization;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AdoptionStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Completed = 4
}
