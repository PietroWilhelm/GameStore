using GameStore.Domain.Enum;

namespace GameStore.Domain.Entities;


public class Game(string name, string description, DateTime launchdate, ContentTypeEnum contentTypeEnum) 
    : Content(name, description, launchdate, contentTypeEnum)
{
}