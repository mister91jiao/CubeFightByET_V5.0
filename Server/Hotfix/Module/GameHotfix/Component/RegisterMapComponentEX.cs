using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using MongoDB.Bson.Serialization;

namespace ETHotfix
{
    [ObjectSystem]
    public class RegisterMapComponentStartSystem : StartSystem<RegisterMapComponent>
    {
        public override void Start(RegisterMapComponent self)
        {
            self.Start();
        }
    }
    public static class RegisterMapComponentEX
    {

        public static void Start(this RegisterMapComponent self)
        {
            Log.Info("注册序列化类");

            BsonClassMap.RegisterClassMap<PlayerInfoDB>(); // do it before you access DB

        }
    }
}
