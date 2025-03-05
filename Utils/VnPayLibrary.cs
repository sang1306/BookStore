using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;


namespace BookStore.Utils
{
    public class VnPayLibrary
    {
        private readonly SortedDictionary<string, string> _requestData = new();
        private readonly SortedDictionary<string, string> _responseData = new();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData[key] = value;
            }
        }

        public string CreateRequestUrl(string baseUrl, string secretKey)
        {
            var queryString = QueryHelpers.AddQueryString(baseUrl, _requestData);
            var rawData = string.Join("&", _requestData.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var hash = HmacSHA512(secretKey, rawData);
            return $"{queryString}&vnp_SecureHash={hash}";
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData[key] = value;
            }
        }

        public bool ValidateSignature(string secretKey)
        {
            var secureHash = _responseData["vnp_SecureHash"];
            _responseData.Remove("vnp_SecureHash");

            var rawData = string.Join("&", _responseData.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var hash = HmacSHA512(secretKey, rawData);

            return secureHash == hash;
        }

        private string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
        }
    }

}
