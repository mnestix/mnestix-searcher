namespace MnestixSearcher.ApiClient
{
    public partial class BasyxApiClient
    {
        /// <summary>
        /// Custom constructor that sets BaseUrl from HttpClient.BaseAddress.
        /// </summary>
        public BasyxApiClient(HttpClient httpClient, bool useBaseAddress)
            : this(httpClient)
        {
            if (useBaseAddress && httpClient.BaseAddress != null)
            {
                BaseUrl = httpClient.BaseAddress.ToString();
            }
        }
    }
}
