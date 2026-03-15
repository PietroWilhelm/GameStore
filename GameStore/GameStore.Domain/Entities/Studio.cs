using GameStore.Domain.Commom;

namespace GameStore.Domain.Entities;

public class Studio : BaseEntity
{
    public string Name { get; private set; }
    public DateTime FoundationDate { get; private set; }
    
    public List<Game> Games { get; private set; } = new List<Game>();

    public Studio(string name, DateTime foundationDate)
    {
        UpdateName(name);
        SetFoundationDate(foundationDate);
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new Exception("O nome da desenvolvedora não pode ser vazio.");
        
        Name = newName;
    }

    public void SetFoundationDate(DateTime date)
    {
        if (date > DateTime.Now)
            throw new Exception("A data de fundação não pode ser no futuro.");

        FoundationDate = date;
    }
}