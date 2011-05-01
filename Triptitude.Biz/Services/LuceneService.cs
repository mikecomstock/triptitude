using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;
using Token = Lucene.Net.Analysis.Token;
using Version = Lucene.Net.Util.Version;

namespace Triptitude.Biz.Services
{
    public class LuceneService
    {
        public string LuceneDestinationsIndexPath
        {
            get
            {
//#if DEBUG
//                return ConfigurationManager.AppSettings["LuceneDestinationsIndexPath"];
//#else
                return HttpContext.Current.Server.MapPath(@"/App_Data/LuceneIndexes/Destinations/");
//#endif
            }
        }

        public void IndexDestinations()
        {
            var directoryInfo = new DirectoryInfo(LuceneDestinationsIndexPath);
            var directory = FSDirectory.Open(directoryInfo);

            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
            IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(100));

            IEnumerable<Destination> countries = new CountriesRepo().FindAll().ToList();
            IEnumerable<Destination> regions = new RegionsRepo().FindAll().ToList();
            IEnumerable<Destination> cities = new CitiesRepo().GetDataReaderForIndexing();
            IEnumerable<Destination> destinations = countries.Union(regions).Union(cities);
            int i = 0;
            foreach (var destination in destinations)
            {
                if (++i % 5000 == 0)
                {
                    Console.Clear();
                    Console.WriteLine(i + destination.FullName);
                }
                Document doc = new Document();
                doc.Add(new Field("fullName", destination.FullName, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("id", destination.GeoNameID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                indexWriter.AddDocument(doc);
            }

            indexWriter.Commit();
            indexWriter.Optimize();
            indexWriter.Close();
        }

        public IEnumerable<DestinationSearchResult> SearchDestinations(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) yield break;

            DirectoryInfo directoryInfo = new DirectoryInfo(LuceneDestinationsIndexPath);
            FSDirectory directory = FSDirectory.Open(directoryInfo);

            IndexSearcher searcher = new IndexSearcher(directory, true);
            StandardAnalyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
            TokenStream tokenStream = analyzer.TokenStream("fullName", new StringReader(term));
            BooleanQuery query = new BooleanQuery();
            
            Token token = tokenStream.Next();
            while (token != null)
            {
                Term thisTerm = new Term("fullName", token.Term());

                TermQuery termQuery = new TermQuery(thisTerm);
                query.Add(termQuery, BooleanClause.Occur.SHOULD);

                Query prefixQuery = new PrefixQuery(thisTerm);
                query.Add(prefixQuery, BooleanClause.Occur.MUST);

                token = tokenStream.Next();
            }

            TopScoreDocCollector collector = TopScoreDocCollector.create(10, true);
            searcher.Search(query, collector);
            ScoreDoc[] hits = collector.TopDocs().scoreDocs;

            foreach (ScoreDoc scoreDoc in hits)
            {
                var document = searcher.Doc(scoreDoc.doc);
                yield return new DestinationSearchResult
                {
                    FullName = document.Get("fullName"),
                    GeoNameId = int.Parse(document.Get("id"))
                };
            }
        }

        public class DestinationSearchResult
        {
            public string FullName { get; set; }
            public int GeoNameId { get; set; }
        }
    }
}