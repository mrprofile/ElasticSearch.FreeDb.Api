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

        public object Any(AutoCompleteRequest request)
        {
            var multiResult = Client().MultiSearch(ms => ms
                .Search<AutoComplete>("Artist",
                    i =>
                        i.Index("autocomplete")
                            .Type("autocomplete")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.Filtered(
                                        x => x.Query(f => f.Match(m => m.OnField(a => a.ObjectType).Query("Artist"))
                                                          &&
                                                          f.Match(m1 => m1.OnField(m2 => m2.Name).Query(request.Query))))))
                .Search<AutoComplete>("Song",
                    i =>
                        i.Index("autocomplete")
                            .Type("autocomplete")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.Filtered(
                                        x => x.Query(f => f.Match(m => m.OnField(a => a.ObjectType).Query("Song"))
                                                          &&
                                                          f.Match(m1 => m1.OnField(m2 => m2.Name).Query(request.Query))))))
                .Search<AutoComplete>("Album",
                    i =>
                        i.Index("autocomplete")
                            .Type("autocomplete")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.Filtered(
                                        x => x.Query(f => f.Match(m => m.OnField(a => a.ObjectType).Query("Album"))
                                                          &&
                                                          f.Match(m1 => m1.OnField(m2 => m2.Name).Query(request.Query)))))));

            return new
            {
                artists = multiResult.GetResponse<AutoComplete>("Artist").Documents.Distinct(),
                songs = multiResult.GetResponse<AutoComplete>("Song").Documents.Distinct(),
                albums = multiResult.GetResponse<AutoComplete>("Album").Documents.Distinct()
            };
        }

        public object Any(AutoCompleteSimpleRequest request)
        {
            var multiResult = Client().MultiSearch(ms => ms
                .Search<AutoComplete>("Artist",
                    i =>
                        i.Index("autocomplete")
                            .Type("autocomplete")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.Filtered(
                                        x => x.Query(f => f.Match(m => m.OnField(a => a.ObjectType).Query("Artist"))
                                                          && f.Match(m1 => m1.OnField(m2 => m2.Name).Query(request.Query))))))
                .Search<AutoComplete>("Song",
                    i =>
                        i.Index("autocomplete")
                            .Type("autocomplete")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.Filtered(
                                        x => x.Query(f => f.Match(m => m.OnField(a => a.ObjectType).Query("Song"))
                                                          && f.Match(m1 => m1.OnField(m2 => m2.Name).Query(request.Query))))))
                .Search<AutoComplete>("Album",
                    i =>
                        i.Index("autocomplete")
                            .Type("autocomplete")
                            .Skip((request.Offset) * request.Limit)
                            .Take(request.Limit)
                            .Query(
                                q =>
                                    q.Filtered(
                                        x => x.Query(f => f.Match(m => m.OnField(a => a.ObjectType).Query("Album"))
                                                          && f.Match(m1 => m1.OnField(m2 => m2.Name).Query(request.Query)))))));

            return new
            {
                artists = multiResult.GetResponse<AutoComplete>("Artist").Documents.Select(x => x.Name).Distinct(),
                songs = multiResult.GetResponse<AutoComplete>("Song").Documents.Select(x => x.Name).Distinct(),
                albums = multiResult.GetResponse<AutoComplete>("Album").Documents.Select(x => x.Name).Distinct()
            };
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
                                                .Query(request.Query))));

            returnValue.TimeElapsed = sp.Elapsed.TotalSeconds;
            returnValue.CurrentPage = (request.Offset == 0) ? 1 : request.Offset;
            returnValue.Result = results.Documents.ToList();
            returnValue.ItemCount = results.Documents.Count();
            returnValue.TotalItems = results.Total;
            returnValue.TotalPages = (int)Math.Ceiling((double)results.Total / request.Limit);

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