using System;

namespace ETModel
{
    public static class UnitFactory
    {
        public static Unit Create(int account)
        {
            try
            {
                Unit unit = ComponentFactory.Create<Unit, int>(account);


                return unit;
            }
            catch (Exception e)
            {
                Log.Error("创建单位失败：" + e);
                return null;
            }
        }

    }
}
