using System;

namespace ETModel
{
    public static class GloabConfigHelper
    {
        /// <summary>
        /// 全局延迟
        /// </summary>
        public static int ping = 0;

        public static ControllerType controllerType = ControllerType.PC;

        /// <summary>
        /// 每秒发包次数，但是不会超过帧数
        /// </summary>
        public static float tick = 1.0f / 32.0f;
    }

    public enum ControllerType
    {
        PC,
        Mobile
    }
}