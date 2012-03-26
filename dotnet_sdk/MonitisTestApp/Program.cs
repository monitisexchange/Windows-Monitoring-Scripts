using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Monitis;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace MonitisTestApp
{
    class Program
    {
        private static string apiKey = "3HQB2PHSMHCRRHV5M9FR8MTBC";
        private static string sekretKey = "275SP14E12PLO0RRB12QF301CT ";
        private static string userName = "monitis_test@adoriasoft.com";
        private static string pass = "SuperPassword!";

        static void Main(string[] args)
        {
            //bool? b = null;
            //Console.WriteLine(b.ToString());
            //OutputType output = OutputType.XML;

            //string testXmlString =
            //     @"<result><startDate>2012-03-20 17:21</startDate><postData>null</postData><interval>5</interval><testId>82276</testId><detailedType>null</detailedType><authPassword>null</authPassword><tag>tag new</tag><authUsername>null</authUsername><params></params><type>ping</type><url>www.yandex.ru</url><locations><location><id>4</id><name>UK</name><checkInterval>5</checkInterval><fullName>UK</fullName></location><location><id>11</id><name>NL</name><checkInterval>5</checkInterval><fullName>Netherlands</fullName></location></locations><name>test pingah25h</name><sla></sla><matchText>null</matchText><match>null</match><timeout>10000</timeout></result>";
            //var ob =
            //     Monitis.Helper.DeserializeObject<Monitis.Structures.ExternalMonitorInformation>
            //         (testXmlString, output, "result");

            Console.WriteLine("END! Press any key to exit");
            //Console.ReadKey();
        }
    }
}
