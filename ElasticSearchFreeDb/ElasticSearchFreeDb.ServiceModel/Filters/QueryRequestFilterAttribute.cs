using System;
using ServiceStack.Web;

namespace ElasticSearchFreeDb.ServiceModel.Filters
{
    public class QueryRequestFilterAttribute : Attribute, IHasRequestFilter
    {
        public void RequestFilter(IRequest req, IResponse res, object requestDto)
        {
            var query = req.QueryString["q"] ?? req.QueryString["query"];
            var limit = req.QueryString["limit"];
            var offset = req.QueryString["offset"];

            var dto = requestDto as QueryBase;
            if (dto == null) { return; }
            dto.Query = query;
            dto.Limit = string.IsNullOrEmpty(limit) ? 24 : int.Parse(limit);
            dto.Offset = string.IsNullOrEmpty(offset) ? 0 : int.Parse(offset);
        }

        public IHasRequestFilter Copy()
        {
            return this;
        }

        public int Priority
        {
            get { return -100; }
        }
    }
}