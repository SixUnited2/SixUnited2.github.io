using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace yz.gaming.accessoryapp.Utils
{
    /// <summary>
    /// 媒体颜色帮助类
    /// </summary>
    public class MediaColorUtils
    {
        #region 相似比较
        /// <summary>
        /// [ChatGPT] 计算两个颜色之间的欧几里得距离（即两个颜色在 RGB 空间中的距离）
        /// </summary>
        public static double ColorDistance(Color color1, Color color2)
        {
            int rDiff = color1.R - color2.R;
            int gDiff = color1.G - color2.G;
            int bDiff = color1.B - color2.B;


            return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }


        /// <summary>
        /// [ChatGPT] 判断两个颜色是否近似（使用 两个颜色之间的欧几里得距离 与 给定阈值 进行比较，如果距离小于指定的阈值，则认为这两个颜色近似）
        /// </summary>
        public static bool AreColorsSimilar1(Color color1, Color color2, double threshold = 26)
        {
            double distance = ColorDistance(color1, color2);
            return distance <= threshold;
        }


        /// <summary>
        /// 获取两个颜色的 RGB 分量差之和
        /// </summary>
        public static int ColorSumOfComponentDifferences(Color color1, Color color2)
        {
            int rDiff = Math.Abs(color1.R - color2.R);
            int gDiff = Math.Abs(color1.G - color2.G);
            int bDiff = Math.Abs(color1.B - color2.B);
            return rDiff + gDiff + bDiff;
        }


        /// <summary>
        /// [ChatGPT] 判断两个颜色是否近似（判断两个颜色的 RGB 分量差之和是否小于指定的阈值，如果小于则认为这两个颜色近似）
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool AreColorsSimilar2(Color color1, Color color2, int threshold = 45)
        {
            int sum = ColorSumOfComponentDifferences(color1, color2);
            return sum <= threshold;
        }


        #endregion


        #region 媒体颜色转换


        /// <summary>
        /// System.Drawing.Color 转 System.Windows.Media.Color
        /// </summary>
        /// <returns><see cref="Color"/> 对象，转换失败返回透明色</returns>
        public static Color DrawingColorToMediaColor(System.Drawing.Color drawingColor)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(drawingColor.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Colors.Transparent;
            }
        }


        /// <summary>
        /// 从颜色字符串（支持RGB和ARGB）转换为媒体颜色
        /// </summary>
        /// <param name="colorStr">ARGB颜色字符串（如#FF000000、#000000）</param>
        /// <returns><see cref="Color"/> 对象，转换失败返回透明色</returns>
        public static Color ColorStrToMediaColor(string colorStr)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(colorStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Colors.Transparent;
            }
        }


        #endregion
    }
}
