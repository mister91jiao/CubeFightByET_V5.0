using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_PlayerHealthUpuateHandler : AMHandler<G2C_PlayerHealthUpuate>
    {
        private PlayerCubeHealthComponent healthComponent = null;

        protected override async ETTask Run(ETModel.Session session, G2C_PlayerHealthUpuate message)
        {
            //Debug.LogError("角色新血量：" + message.NewHealth + " 角色是否死亡：" + message.Die);

            if (healthComponent == null)
            {
                healthComponent = MapHelper.nowPlayerCube.GetComponent<PlayerCubeHealthComponent>();
            }

            healthComponent.SetPlayerCubeHealth(message.NewHealth);

            if (message.Die)
            {
                healthComponent.SetPlayerDie();
            }

            await ETTask.CompletedTask;
        }
    }
}
