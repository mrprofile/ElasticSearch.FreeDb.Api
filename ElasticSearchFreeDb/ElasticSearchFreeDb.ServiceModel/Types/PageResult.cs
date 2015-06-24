using System.Security.AccessControl;

namespace ElasticSearchFreeDb.ServiceModel.Types
{
    public class PageResult<T> where T : class
    {
        public T Result { get; set; }

        public long TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int ItemCount { get; set; }
        public int CurrentPage { get; set; }
        public double TimeElapsed { get; set; }
        
        public dynamic Genres { get; set; }
    }
}