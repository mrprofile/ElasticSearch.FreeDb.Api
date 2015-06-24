using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ElasticSearchFreeDb.ServiceModel.Types;
using Nest;
using ServiceStack;
using ElasticSearchFreeDb.ServiceModel;

namespace ElasticSearchFreeDb.ServiceInterface
{
    public class FreeDbServices : Service
    {
        public ElasticClient Client()
        {
            var node = new Uri("http://10.128.36.197:9200/");
            var settings = new ConnectionSettings(node, defaultIndex: "disk");

            return new ElasticClient(settings);
        }

        public object Any(FreeDb request)
        {
            var sp = Stopwatch.StartNew();
            var returnValue = new PageResult<List<Disk>>();

            var results =
                Client().Search<Disk>(
                    i =>
                        i.Index("disk")
                        .Type("disk")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.QueryString(
                                        qs =>
                                            qs.OnFields(new[] { "artist", "title", "tracks" })
                                                .Query(request.Query)))
                                                    .Aggregations((a => a.Terms("genre.raw", t => t.Field("genre.raw").Size(20)))));
            
            var myAgg = results.Aggs.Terms("genre.raw");
            var genreGroup = myAgg.Items.ToDictionary(item => item.Key, item => item.DocCount);
            
            returnValue.TimeElapsed = sp.Elapsed.TotalSeconds;
            returnValue.CurrentPage = (request.Offset == 0) ? 1 : request.Offset;
            returnValue.Result = results.Documents.ToList();
            returnValue.ItemCount = results.Documents.Count();
            returnValue.TotalItems = results.Total;
            returnValue.TotalPages = (int)Math.Ceiling((double)results.Total / request.Limit);
            returnValue.Genres = genreGroup;

            return returnValue;
        }

        public object Any(FreeDbMatchSearch request)
        {
            var sp = Stopwatch.StartNew();
            var returnValue = new PageResult<List<Disk>>();

            var results =
                Client().Search<Disk>(
                    i =>
                        i.Index("disk")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(q => q.Match(m => m.OnField(p => p.Artist).Query(request.Artist)) && q.Match(m => m.OnField(p => p.Genre).Query(request.Genre)))
                            .Aggregations((a => a.Terms("genre.raw", t => t.Field("genre.raw").Size(20)))));

            returnValue.TimeElapsed = sp.Elapsed.TotalSeconds;
            returnValue.CurrentPage = (request.Offset == 0) ? 1 : request.Offset;
            returnValue.Result = results.Documents.ToList();
            returnValue.ItemCount = results.Documents.Count();
            returnValue.TotalItems = results.Total;
            returnValue.TotalPages = (int)Math.Ceiling((double)results.Total / request.Limit);
            
            return returnValue;
        }
    }
}