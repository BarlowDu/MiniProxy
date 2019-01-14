using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Utils
{
    public class TypeUtils
    {
        /// <summary>
        /// 判断是否是实体(非基础类型与string)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsModel(Type type)
        {
            if (type.IsPrimitive)
            {
                return false;
            }

            if (type == typeof(decimal))
            {
                return false;
            }
            if (type == typeof(string))
            {
                return false;
            }

            return true;
        }

        public static bool IsPrimitive(Type type) {

            if (type.IsPrimitive)
            {
                return true;
            }

            if (type == typeof(decimal))
            {
                return true;
            }

            return false;
        }
    }
}
