using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Triptitude.Biz.Models;
using Triptitude.Biz.Repos;

namespace Triptitude.Biz.Services
{
    public class LuceneService
    {
        public string LuceneDestinationsIndexPath
        {
            get { return ConfigurationManager.AppSettings["LuceneDestinationsIndexPath"]; }
        }

        public void IndexDestinations()
        {
            var directoryInfo = new DirectoryInfo(LuceneDestinationsIndexPath);
            var directory = FSDirectory.Open(directoryInfo);

            Analyzer analyzer = new KeywordAnalyzer();
            IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(15));

            IEnumerable<Destination> countries = new CountriesRepo().FindAll().ToList();
            IEnumerable<Destination> regions = new RegionsRepo().FindAll().ToList();

            foreach (var country in countries.Union(regions))
            {
                var nameField = new Field("fullName", country.FullName, Field.Store.YES, Field.Index.ANALYZED);
                var idField = new Field("id", country.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);

                var doc = new Lucene.Net.Documents.Document();
                doc.Add(nameField);
                doc.Add(idField);
                indexWriter.AddDocument(doc);
            }

            indexWriter.Commit();
            indexWriter.Optimize();
            indexWriter.Close();
        }

        public IEnumerable<DestinationSearchResult> SearchDestinations(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) yield break; ;

            DirectoryInfo directoryInfo = new DirectoryInfo(LuceneDestinationsIndexPath);
            FSDirectory directory = FSDirectory.Open(directoryInfo);

            IndexSearcher searcher = new IndexSearcher(directory, true);
            Analyzer analyzer = new KeywordAnalyzer();
            QueryParser queryParser = new QueryParser(Version.LUCENE_29, "fullName", analyzer);
            var fuzzyQuery = queryParser.GetFuzzyQuery("fullName", term, 0.4f);
            //Query query = queryParser.Parse(term);
            //Query fuzzyQuery = new FuzzyQuery(new Term("fullName", term));

            //Query prefixQuery = new PrefixQuery(new Term("fullName", term));
            //MultiFieldQueryParser q = new MultiFieldQueryParser();
            //BooleanQuery query = new BooleanQuery();
            //query.Add(fuzzyQuery, BooleanClause.Occur.SHOULD);
            //query.Add(prefixQuery, BooleanClause.Occur.SHOULD);

            TopScoreDocCollector collector = TopScoreDocCollector.create(10, true);
            searcher.Search(fuzzyQuery, collector);
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