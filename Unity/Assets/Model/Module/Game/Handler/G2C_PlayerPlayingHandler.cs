using ETModel;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class G2C_PlayerPlayingHandler : AMHandler<G2C_PlayerPlaying>
    {
        protected override async ETTask Run(ETModel.Session session, G2C_PlayerPlaying message)
        {
            
            await ETTask.CompletedTask;
        }
    }
}

