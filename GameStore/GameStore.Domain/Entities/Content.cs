using GameStore.Domain.Commom;
using GameStore.Domain.Entities;
using GameStore.Domain.Enum;

public abstract class Content : BaseEntity
{
    public string Name { get; private set; }

    public string Description { get; private set; }
    
    public DateTime LaunchDate { get; private set; }

    public ContentTypeEnum ContentType { get; private set; }

    //N.N - Relacionamento
    public List<Genre> Genres { get; set; } = [];
    
    protected Content()
    {
    }
    
    protected Content(string name, string description, DateTime launchDate,  ContentTypeEnum contentType = ContentTypeEnum.others)
    {
        Name = name;

        if (string.IsNullOrEmpty(description)) throw new Exception("Description is empty");
        
        Description = description;
        
        if (launchDate.Year < 1958) throw new Exception("Year must be greater than 1958");
        
        LaunchDate = launchDate;
        
        Active = false;

        ContentType = contentType;
    }
}