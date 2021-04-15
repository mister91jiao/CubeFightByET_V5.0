using System.Collections.Generic;
using System.Linq;

namespace ETModel
{

    [ObjectSystem]
    public class UnitComponentAwakeSystem : AwakeSystem<UnitComponent>
    {
        public override void Awake(UnitComponent self)
        {
            self.Awake();
        }
    }
    public class UnitComponent : Component
    {
        private Dictionary<int, Unit> AccountToUnit = new Dictionary<int, Unit>();

        public void Awake()
        {
            Log.Info("单位管理组件初始化");
        }

        /// <summary>
        /// 添加玩家实体到字典管理
        /// </summary>
        public void addUnitToDict(int account, Unit player)
        {
            if (!AccountToUnit.ContainsKey(account))
            {
                AccountToUnit.Add(account, player);
            }
            else
            {
                Log.Error("Unit账号重复添加：" + account);
            }

        }

        /// <summary>
        /// 移除一个玩家的Unit
        /// </summary>
        public void removeOneUnit(int account)
        {
            if (AccountToUnit.ContainsKey(account))
            {
                AccountToUnit.Remove(account);
            }
            else
            {
                Log.Error("这个Unit不存在Map服里: " + account);
            }
        }

        /// <summary>
        /// 通过账号来获取Unit
        /// </summary>
        public Unit getUnitByAccount(int account)
        {
            if (AccountToUnit.TryGetValue(account, out Unit unit))
            {
                return unit;
            }
            else
            {
                Log.Info("错误，没有获取到unit：" + account);
                return null;
            }
        }

        /// <summary>
        /// 得到不少于多少个Unit
        /// </summary>
        public List<Unit> getCountUnits(int minCount)
        {
            if (AccountToUnit.Count > minCount)
            {
                return AccountToUnit.Values.ToList<Unit>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 的带当前map服中的玩家数
        /// </summary>
        public int UnitCounts()
        {
            return AccountToUnit.Count;
        }

        /// <summary>
        /// Map服中包含这个account的Unit
        /// </summary>
        public bool UnitHaveBeCreated(int account)
        {
            return AccountToUnit.ContainsKey(account);
        }

    }
}
