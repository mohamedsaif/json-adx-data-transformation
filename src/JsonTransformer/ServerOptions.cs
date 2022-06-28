using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTransformer
{
    public class ServerOptions
    {
        /// <summary>
        /// Indicates the current used version and is displayed in the health endpoint
        /// </summary>
        public string AppVersion { get; set; }
    }
}
