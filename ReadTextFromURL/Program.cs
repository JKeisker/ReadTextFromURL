using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadTextFromURL
{
    class Program
    {
        static Dictionary<string, int> countWords = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            ReadFromURL();
            Console.Read();
        }

        static void ReadFromURL()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://en.wikipedia.org/wiki/Microsoft");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream getStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader strmRdr = new StreamReader(getStream, encode);
            long ilength = response.ContentLength;

            char[] read = new char[ilength];
            int count = strmRdr.Read(read, 0, (int)ilength);
            string result = "";
            while (count > 0)
            {
                string s = new string(read, 0, count);
                result += s;
                count = strmRdr.Read(read, 0, (int)ilength);
            }

            result = Regex.Replace(result, "<.*?>", string.Empty);
            result = WebUtility.HtmlDecode(result);

            int idx = result.IndexOf("History");
            idx = result.IndexOf("History", idx + 1);
            result = result.Substring(idx);
            idx = result.IndexOf("Corporate affairs");
            string getrid = result.Substring(idx);
            result = result.Substring(0, result.Length - getrid.Length);

            response.Close();
            strmRdr.Close();

            CountWordOccurance(result);
        }

        static void CountWordOccurance(string result)
        {
            string[] arrResult = result.Split(' ');

            foreach (string s in arrResult)
            {
                if (countWords.ContainsKey(s))
                {
                    int n = countWords[s];
                    n++;
                    countWords[s] = n;
                }
                else
                {
                    countWords.Add(s, 1);
                }
            }

            countWords = countWords.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            int i = 0;
            for (; i < 10; i++)
            {
                KeyValuePair<string, int> kvp = countWords.ElementAt(i);
                Console.WriteLine(kvp.Key + " " + kvp.Value.ToString());
            }
        }
    }
}
