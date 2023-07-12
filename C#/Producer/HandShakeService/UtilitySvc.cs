using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandShakeService
{
    public static class UtilitySvc
    {
        /// <summary>
        /// This method is to check if the given uri is valid or not.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Boolean IsValidUri(String uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }
        public static bool isValidInt(string value)
        {
            try
            {

                int x = 0;
                if (int.TryParse(value, out x))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
