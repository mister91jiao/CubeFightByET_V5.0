using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Gate)]
    public class Actor_PlayerResurrectionHandler : AMActorHandler<Player, Actor_PlayerResurrection>
    {
        protected override async ETTask Run(Player player, Actor_PlayerResurrection message)
        {
            player.session.Send(new G2C_PlayerResurrection()
            {
                ResurrectionPlayerAccount = message.ResurrectionPlayerAccount,
                PositionX = message.PositionX,
                PositionY = message.PositionY,
                PositionZ = message.PositionZ,
            });

            await ETTask.CompletedTask;
        }
    }
}
