using GameStore.Domain.Commom;
using GameStore.Domain.Enum;

public abstract class Content : BaseEntity
{
    public string Name { get; private set; }

    public string Description { get; private set; }
    
    public DateTime LaunchDate { get; private set; }

    public ContentTypeEnum ContentType { get; private set; }
    
    protected Content(string name, string description, DateTime launchDate,  ContentTypeEnum contentType = ContentTypeEnum.Outros)
    {
        Name = name;

        if (string.IsNullOrEmpty(description)) throw new Exception("Description is empty");
        
        Description = description;
        
        if (launchDate.Year < 1958) throw new Exception("Year must be greater than 1895");
        
        LaunchDate = launchDate;
        
        Active = false;

        ContentType = contentType;
    }
}