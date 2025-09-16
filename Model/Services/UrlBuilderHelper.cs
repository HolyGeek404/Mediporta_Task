namespace Model.Services;

public static class UrlBuilderHelper
{
   public static string BuildStackOverflowTagUrl(int page,
      int pageSize,
      string order,
      string sort,
      string apiKey)
   {
      var url = $"tags?page={page}" +
                $"&pagesize={pageSize}" +
                $"&order={order}" +
                $"&sort={sort}" +
                $"&site=stackoverflow" +
                $"&apikey={apiKey}";
      return url;
   }
   public static string BuildTagApiUrl(int page,
      int pageSize,
      string order,
      string sort)
   {
      var url = $"tags?page={page}" +
                $"&pagesize={pageSize}" +
                $"&order={order}" +
                $"&sort={sort}";
      return url;
   }

   public static string BuildRefreshTagUrl()
   {
       return "tags/refresh";
   }
}  