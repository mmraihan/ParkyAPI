using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public static string APIBaseUrl = "https://localhost:44312/";
        public static string NationalParkApiPath = APIBaseUrl + "api/v1/nationalParks/";
        public static string TrailApiPath = APIBaseUrl + "api/v2/trails/";
    }
}
