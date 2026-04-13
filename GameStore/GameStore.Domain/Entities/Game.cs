using GameStore.Domain.Enum;

namespace GameStore.Domain.Entities;

public class Game : Content
{
    public Guid StudioId { get; private set; }
    public Studio Studio { get; private set; } = null!;

    protected Game()
    {
    }

    public Game(string name, string description, DateTime launchDate, ContentTypeEnum contentTypeEnum, Guid studioId)
        : base(name, description, launchDate, contentTypeEnum)
    {
        SetStudio(studioId);
    }

    public void SetStudio(Guid studioId)
    {
        if (studioId == Guid.Empty)
            throw new Exception("StudioId is invalid");

        StudioId = studioId;
    }
}
