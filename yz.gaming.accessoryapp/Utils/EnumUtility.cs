using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace yz.gaming.accessoryapp.Utils
{
    public class EnumUtility
    {
        /// <summary>
        /// 取得枚举的描述名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(Enum value)
        {
            try
            {
                StringBuilder sb = new StringBuilder(50);
                string[] str = value.ToString().Split(',');

                for (int i = 0; i < str.Length; i++)
                {
                    FieldInfo field = value.GetType().GetField(str[i].Trim());

                    if (field == null)
                    {
                        return str[i].Trim();
                    }

                    DescriptionAttribute[] objs = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (objs == null || objs.Length == 0)
                    {
                        sb.Append(str[i].Trim());
                    }
                    else
                    {
                        DescriptionAttribute da = objs[0];
                        sb.Append(da.Description);
                    }

                    if (i != str.Length - 1)
                    {
                        sb.Append(" | ");
                    }
                }

                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }

        }
    }
}
