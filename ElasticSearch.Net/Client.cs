using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Net
{
   public class Client
    {
       public Client(dynamic config)
       {
           Transport transport = new Transport(config);
       }
    }
}
