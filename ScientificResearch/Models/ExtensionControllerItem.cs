using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificResearch.Models
{
    public static class ExtensionControllerItem
    {
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="controllerItem"></param>
        /// <param name="data"></param>
        public static void ParseDataToControllerItem(this ControllerItem controllerItem, byte[] data)
        {
            int startIndex = 5;

            // 解析开关状态
            controllerItem.PowerState = data[startIndex] == 0 ? false : true;

            // 解析各路数据
            controllerItem.LightsColorValues = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                startIndex += 2;
                int prev = (data[startIndex] << 8);
                int next = data[startIndex - 1];
                controllerItem.LightsColorValues.Add(prev | next);
            }

            startIndex += 4;
            // 解析关闭定时器
            controllerItem.OffTimerCount = 0;
            controllerItem.OffTimersMinutes = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                if (data[startIndex + 1] == 255 || data[startIndex] == 255)
                {
                    startIndex += 2;
                    continue;
                }

                int prev = (data[startIndex + 1] << 8);
                int next = data[startIndex];
                controllerItem.OffTimersMinutes.Add(prev | next);

                startIndex += 2;

                controllerItem.OffTimerCount++;
            }

            // 解析打开定时器
            controllerItem.OnTimerCount = 0;
            controllerItem.OnTimersMinutes = new List<int>();
            controllerItem.OnTimersColorValues = new List<List<int>>();
            for (int i = 0; i < 8; i++)
            {
                if (data[startIndex + 1] == 255 || data[startIndex] == 255)
                {
                    startIndex += 8;
                    continue;
                }

                int prev = (data[startIndex + 1] << 8);
                int next = data[startIndex];
                controllerItem.OnTimersMinutes.Add(prev | next);

                List<int> colorValues = new List<int>();
                for (int j = 0; j < 6; j++)
                {
                    colorValues.Add(data[startIndex + 2]);
                    startIndex += 1;
                }

                controllerItem.OnTimersColorValues.Add(colorValues);
                controllerItem.OnTimerCount++;
                startIndex += 2;
            }
        }

        /// <summary>
        /// 对模型中的定时器排序
        /// </summary>
        /// <param name="controllerItem"></param>
        public static void SortTimers(this ControllerItem controllerItem)
        {
            for (int i=0;i<controllerItem.OnTimerCount;i++)
            {
                for (int j=i;j<controllerItem.OnTimerCount;j++)
                {
                    if (controllerItem.OnTimersMinutes[i] > controllerItem.OnTimersMinutes[j])
                    {
                        // 交换时间点及颜色值信息
                        int tmp = controllerItem.OnTimersMinutes[j];
                        controllerItem.OnTimersMinutes[j] = controllerItem.OnTimersMinutes[i];
                        controllerItem.OnTimersMinutes[i] = tmp;

                        List<int> tmpColorValue = controllerItem.OnTimersColorValues[j];
                        controllerItem.OnTimersColorValues[j] = controllerItem.OnTimersColorValues[i];
                        controllerItem.OnTimersColorValues[i] = tmpColorValue;
                    }
                }
            }

            for (int i = 0; i < controllerItem.OffTimerCount; i++)
            {
                for (int j = i; j < controllerItem.OffTimerCount; j++)
                {
                    if (controllerItem.OffTimersMinutes[i] > controllerItem.OffTimersMinutes[j])
                    {
                        // 交换时间点
                        int tmp = controllerItem.OffTimersMinutes[j];
                        controllerItem.OffTimersMinutes[j] = controllerItem.OffTimersMinutes[i];
                        controllerItem.OffTimersMinutes[i] = tmp;
                    }
                }
            }
        }
    }
}
