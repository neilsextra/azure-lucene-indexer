using Lucene.Net.Store;
using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Documents;
using System.Linq;

namespace azure_lucene_indexer
{
    public class LuceneIndexer
    {
        private Directory luceneIndexDirectory;
        private IndexWriter indexWriter;
        private Analyzer analyzer = new Lucene.Net.Analysis.WhitespaceAnalyzer();
        private QueryParser parser;
        private const int MAX_HITS = 1000;

        public LuceneIndexer(string indexPath)
        {

            luceneIndexDirectory = FSDirectory.Open(indexPath);
            indexWriter = new IndexWriter(luceneIndexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

        }

        /// <summary>
        /// Add an Index Entry
        /// </summary>
        /// <param name="id">The Index Identifier</param>
        /// <param name="name">The Search Name</param>
        public void AddIndexEntry(String id, String name)
        {
            Document doc = new Document();
            doc.Add(new Field("Id", id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", name, Field.Store.YES, Field.Index.ANALYZED));
            indexWriter.AddDocument(doc);
            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);
            indexWriter.Commit();

        }

        public void AddIndexEntries(IndexEntry[] indexEntries)
        {
            foreach (var indexEntry in indexEntries)
            {
                Document doc = new Document();
                doc.Add(new Field("Id", indexEntry.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Name", indexEntry.Name, Field.Store.YES, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }

            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);
            indexWriter.Commit();

        }

       public void Delete(string id)
       {

            indexWriter.DeleteDocuments(new Term("Id", id));
            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);
            indexWriter.Commit();

       }
       
        public IndexEntry Get(string id)
        {
            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);
            var query = new TermQuery(new Term("Id", id));

            var hits = searcher.Search(query, MAX_HITS).ScoreDocs;
            var results = hits.Select(hit => MapDocument(hit, searcher.Doc(hit.Doc))).ToList();

            return results.Count == 0 ? null : results.ElementAt(0);

        }

        public IndexEntry[] Search(string term)
        {
            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);

            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Name", analyzer);
            Query query = parser.Parse(term.Trim());


            var hits = searcher.Search(query, MAX_HITS).ScoreDocs;
            var results = hits.Select(hit => MapDocument(hit, searcher.Doc(hit.Doc))).ToArray();
     
            return results;

        }

        private IndexEntry MapDocument(ScoreDoc hit, Document document)
        {
            return new IndexEntry
            {

                Id = document.Get("Id"),
                Name = document.Get("Name"),
                Score = hit.Score

            };
        }
        
    }

}