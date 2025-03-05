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
        private readonly IConfiguration _configuration;
        public const string VERSION = "2.1.0";
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        public SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        private string vnp_Url;
        private string vnp_Api;
        private string vnp_TmnCode;
        private string vnp_HashSecret;

        public VnPayLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            vnp_Url = _configuration["Vnpay:vnp_Url"];
            vnp_Api = _configuration["Vnpay:vnp_Api"];
            vnp_TmnCode = _configuration["Vnpay:vnp_TmnCode"];
            vnp_HashSecret = _configuration["Vnpay:vnp_HashSecret"];
        }

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl()
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

        public bool ValidateSignature(string inputHash)
        {
            string rspRaw = GetResponseData();
            string myChecksum = UtilsVnPay.HmacSHA512(vnp_HashSecret, rspRaw);


            Console.WriteLine("ValidateSignature:rspRaw: " + rspRaw);
            Console.WriteLine("ValidateSignature:myCheckSum: " + myChecksum);
            Console.WriteLine("ValidateSignature:inputHash: " + inputHash);
            
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

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
