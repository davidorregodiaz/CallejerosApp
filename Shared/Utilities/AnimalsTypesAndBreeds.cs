namespace Shared.Utilities;

public static class AnimalsTypesAndBreeds
{
    public static readonly List<string> AnimalsTypes = ["Perro", "Gato", "Roedor"];

    public static readonly Dictionary<string, List<string>> BreedsByType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Perro"] = ["---All breeds---","Labrador", "Pastor Belga", "Bulldog"],
        ["Gato"] = ["---All breeds---","Siamés", "Persa", "Bengalí"],
        ["Roedor"] = ["---All breeds---","Hámster", "Cobaya", "Chinchilla"]
    };

    public static List<string> GetBreedsForType(string type)
    {
        Console.WriteLine(type);
        return BreedsByType.TryGetValue(type, out var breeds)
            ? breeds
            : new List<string>();
    }
    
}
