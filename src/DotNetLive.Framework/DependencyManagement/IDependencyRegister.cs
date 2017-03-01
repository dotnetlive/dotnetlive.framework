using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetLive.Framework.DependencyManagement
{
    /// <summary>
    /// 各个项目内用于组件注册的接口, 系统启动之后会自动扫描所有继承自这个接口的实现类
    /// </summary>
    public interface IDependencyRegister
    {
        ExecuteOrderType ExecuteOrder { get; }
    }

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
