namespace QLBS.Helpers
{
    public static class Utils
    {
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;

            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress)
                            .AddressList
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null)
                        ipAddress = remoteIpAddress.ToString();
                }

                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = context.Request.Headers["X-Forwarded-For"].ToString();

                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = "127.0.0.1";
            }
            catch
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }
    }
}