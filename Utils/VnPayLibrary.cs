using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace BookStore.Utils
{

    public class VnPayLibrary
    {
        public const string VERSION = "2.1.0";
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        public SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public VnPayLibrary(){}

        // set requese data to sortlist
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        // create url vnpay throw vpn_url 
        public string CreateRequestUrl(string vnp_Url, string vnp_HashSecret)
        {
            var data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString().TrimEnd('&');
            string signData = queryString;
            string vnp_SecureHash = UtilsVnPay.HmacSHA512(vnp_HashSecret, signData);
            return vnp_Url + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
        }

        // check signature
        public bool ValidateSignature(string inputHash, string vnp_HashSecret)
        {
            string rspRaw = GetResponseData();

            string myChecksum = UtilsVnPay.HmacSHA512(vnp_HashSecret, rspRaw);

            Console.WriteLine("ValidateSignature:rspRaw: " + rspRaw);
            Console.WriteLine("ValidateSignature:myCheckSum: " + myChecksum);
            Console.WriteLine("ValidateSignature:inputHash: " + inputHash);
            
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }


        // get response dat except 'vnp_SecureHashType' and 'vnp_SecureHash'
        private string GetResponseData()
        {
            var data = new StringBuilder();
            // Remove sensitive keys
            _responseData.Remove("vnp_SecureHashType");
            _responseData.Remove("vnp_SecureHash");

            foreach (var kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string re = data.ToString().TrimEnd('&');
            return re;
        }

        // get value by key in _response data
        public string GetResponseDataByKey(string key)
        {
            _responseData.TryGetValue(key, out var value);
            return value ?? string.Empty;
        }
    }

    public class UtilsVnPay
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

        public static string GetIpAddress(HttpContext httpContext)
        {
            try
            {
                var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                ?? httpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
                {
                    ipAddress = httpContext.Request.Headers["REMOTE_ADDR"];
                }

                return ipAddress ?? "0.0.0.0";
            }
            catch (Exception ex)
            {
                return "Invalid IP: " + ex.Message;
            }
        }

    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
