using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace QLCHBS.Utils
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = new StringBuilder();

            foreach (var kv in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            var queryString = data.ToString();

            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(data.Length - 1, 1);
            }

            baseUrl += "?" + queryString;
            var signData = queryString;

            if (!string.IsNullOrEmpty(vnp_HashSecret))
            {
                var vnpSecureHash = HmacSHA512(vnp_HashSecret, signData);
                baseUrl += "&vnp_SecureHash=" + vnpSecureHash;
            }

            return baseUrl;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            Console.WriteLine("\n>>> VnPayLibrary.ValidateSignature <<<");

            var rspRaw = GetResponseData();
            Console.WriteLine($"Response raw data for signing:");
            Console.WriteLine(rspRaw);

            var myChecksum = HmacSHA512(secretKey, rspRaw);
            Console.WriteLine($"\nMy calculated checksum:");
            Console.WriteLine(myChecksum);

            Console.WriteLine($"\nInput hash (from VNPay):");
            Console.WriteLine(inputHash);

            var isMatch = myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
            Console.WriteLine($"\nChecksums match: {isMatch}");

            return isMatch;
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        private string GetResponseData()
        {
            Console.WriteLine("\n>>> GetResponseData <<<");

            var data = new StringBuilder();

            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                Console.WriteLine("Removing vnp_SecureHashType");
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                Console.WriteLine("Removing vnp_SecureHash");
                _responseData.Remove("vnp_SecureHash");
            }

            Console.WriteLine("\nBuilding hash data from parameters (NO URL encoding for validation):");
            foreach (var kv in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                Console.WriteLine($"  {kv.Key} = {kv.Value}");
                data.Append(kv.Key + "=" + kv.Value + "&");
            }

            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            var result = data.ToString();
            Console.WriteLine($"\nFinal hash data string:");
            Console.WriteLine(result);

            return result;
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}