using GameStore.Domain.Commom;

namespace GameStore.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }

    public List<Content> Contents { get; private set; }

    private Genre()
    {
    }
    
    public Genre(string name, string description)
    {
        Name = name;
        Description = description;
    }
}