using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ScientificResearch.Models
{
    /// <summary>
    /// 控制器
    /// </summary>
    [Serializable]
    public class ControllerItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        [DisplayName("区域")]
        public string Zone { get; set; }

        /// <summary>
        /// 分组id
        /// </summary>
        [DisplayName("分组")]
        public int GroupId { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        [DisplayName("分组名称")]
        public string GroupName { get; set; }

        /// <summary>
        /// 地址id
        /// </summary>
        [DisplayName("组内地址ID")]
        public int AddressId { get; set; }

        /// <summary>
        /// 组内地址名称
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// 开关状态
        /// </summary>
        [DisplayName("开关状态")]
        public bool PowerState { get; set; }

        private List<int> lightsColorValues;
        /// <summary>
        /// 各路值
        /// </summary>
        public List<int> LightsColorValues
        {
            get
            {
                if (lightsColorValues == null) {
                    return new List<int>();
                }
                else
                {
                    return lightsColorValues;
                }
            }
            set
            {
                lightsColorValues = value;
            }
        }

        /// <summary>
        /// 关断时间点个数
        /// </summary>
        public int OffTimerCount { get; set; }

        private List<int> offTimersMinutes;
        /// <summary>
        /// 关断时间信息
        /// </summary>
        public List<int> OffTimersMinutes
        {
            get
            {
                if (offTimersMinutes == null)
                {
                    return new List<int>();
                }

                return offTimersMinutes;
            }
            set
            {
                offTimersMinutes = value;
            }
        }

        /// <summary>
        /// 打开时间点个数
        /// </summary>
        public int OnTimerCount { get; set; }

        public List<int> onTimersMinutes;

        /// <summary>
        /// 打开时间点信息
        /// </summary>
        public List<int> OnTimersMinutes
        {
            get
            {
                if (onTimersMinutes == null)
                {
                    return new List<int>();
                }

                return onTimersMinutes;
            }
            set
            {
                onTimersMinutes = value;
            }
        }

        private List<List<int>> onTimersColorValues;
        /// <summary>
        /// 打开时间点颜色值信息
        /// </summary>
        public List<List<int>> OnTimersColorValues
        {
            get
            {
                if (onTimersColorValues == null)
                {
                    return new List<List<int>>();
                }

                return onTimersColorValues;
            }
            set
            {
                onTimersColorValues = value;
            }
        }
    }
}
