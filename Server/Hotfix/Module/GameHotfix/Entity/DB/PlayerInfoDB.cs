using ETModel;
using MongoDB.Bson.Serialization.Attributes;

namespace ETHotfix
{
    [BsonIgnoreExtraElements]
    public class PlayerInfoDB : Entity
    {
        public int account;

        public string pwd;

        public int KillCount = 0;
        public int DeathCount = 0;
        public int MoneyCount = 0;
    }
}
