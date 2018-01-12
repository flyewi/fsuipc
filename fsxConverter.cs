using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace fsuipcserve
{
    class fsxConverter
    {

        private enum LightType
        {
            Navigation,
            Beacon,
            Landing,
            Taxi,
            Strobes,
            Instruments,
            Recognition,
            Wing,
            Logo,
            Cabin
        }


        public static double fsxairspeed2knots(int fsxspeed) {
            return  (double)fsxspeed / 128d;
        }

        public static double fsxground2knots(int fsxspeed)
        {   // 65536 m/s 
            return (double)fsxspeed * 3600.0 / 65536.0 / 1852.0;
        }

        public static string lightList (System.Collections.BitArray lights)
        {
            
            return JsonConvert.SerializeObject(lights);
        }

        public static double fsxlatitude2degrees (long latitude)
        {

            return (double)latitude * 90.0 / (10001750.0 * 65536.0 * 65536.0);

        }
        public static double fsxlongitude2degrees(long longitude)
        {
            return (double)longitude * 360.0 / (65536.0 * 65536.0 * 65536.0 * 65536.0);
        }
        

    }
}
