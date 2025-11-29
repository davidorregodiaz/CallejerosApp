using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AnimalAggregate;

public class MedicalRecord : ValueObject
{
    public string Vaccine { get; }
    public bool IsStirilized { get; }
    public bool IsDewormed { get; }
    public string HealthState { get; }
    
    private  MedicalRecord() { }

    private MedicalRecord(string vaccine, bool isStirilized, bool isDewormed, string healthState)
    {
        Vaccine = vaccine;
        IsStirilized = isStirilized;
        IsDewormed = isDewormed;
        HealthState = healthState;
    }
    
    //TODO Hacer validaciones a los campos 
    public static MedicalRecord Create(string vaccine, bool isStirilized, bool isDewormed, string healthState)
    {
        return new MedicalRecord(
            vaccine,
            isStirilized,
            isDewormed,
            healthState);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Vaccine;
        yield return IsStirilized;
        yield return IsDewormed;
        yield return HealthState;
    }
}
