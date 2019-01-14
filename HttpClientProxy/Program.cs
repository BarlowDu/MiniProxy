using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {


            IClient client = HttpProxyBuilder.GetProxy<IClient>();
            string result=client.GetBanner(2,1);
            Console.WriteLine(result);

            Console.WriteLine(new String('-', 20));
            result = client.GetBanner(1, 1);
            Console.WriteLine(result);


            Console.WriteLine(new String('-', 20));
            result = client.GetActivityList(2,0,1,1,10);
            Console.WriteLine(result);

            Console.ReadKey();
        }

        
    }



}
