using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Gate)]
    public class Actor_OtherPlayerDieHandler : AMActorHandler<Player, Actor_OtherPlayerDie>
    {
        protected override async ETTask Run(Player player, Actor_OtherPlayerDie message)
        {
            player.session.Send(new G2C_OtherPlayerDie()
            {
                DiePlayerAccount = message.DiePlayerAccount
            });

            await ETTask.CompletedTask;
        }
    }
}
