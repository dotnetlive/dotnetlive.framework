using System;

namespace DotNetLive.Framework.DependencyManagement
{
    [Flags]
    public enum ExecuteOrderType
    {
        /// <summary>
        /// 最低级别
        /// </summary>
        Lowest = -2,
        /// <summary>
        /// 低级别
        /// </summary>
        Lower = -1,
        /// <summary>
        /// 正常顺序
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 高级别
        /// </summary>
        Higher = 1,
        /// <summary>
        /// 最高级别
        /// </summary>
        Highest = 2
    }
}
