using GameStore.Domain.Enum;

namespace GameStore.Domain.Entities;


public class Game(string name, string description, DateTime launchDate, ContentTypeEnum contentTypeEnum) 
    : Content(name, description, launchDate, contentTypeEnum);
