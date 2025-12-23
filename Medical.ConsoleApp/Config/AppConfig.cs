using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.ConsoleApp.Config
{
    public static class AppConfig
    {
        public static readonly string ConnectionString =
       "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=medical;";
    }
}
