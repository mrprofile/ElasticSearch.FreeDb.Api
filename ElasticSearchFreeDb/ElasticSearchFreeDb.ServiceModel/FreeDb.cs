using ElasticSearchFreeDb.ServiceModel.Filters;
using ServiceStack;

namespace ElasticSearchFreeDb.ServiceModel
{
    [QueryRequestFilter]
    [Route("/freedb/search")]
    public class FreeDb : QueryBase, IReturn<FreeDbResponse>
    {
        
    }

    [QueryRequestFilter]
    [Route("/freedb/matchsearch")]
    public class FreeDbMatchSearch : QueryBase, IReturn<FreeDbResponse>
    {
        public string Genre { get; set; }
        public string Artist { get; set; }
        public int Year { get; set; }
    }

    public class FreeDbResponse
    {
        public string Result { get; set; }
    }

    [QueryRequestFilter]
    [Route("/content/search")]
    public class ContentRequest
    {
        
    }
}