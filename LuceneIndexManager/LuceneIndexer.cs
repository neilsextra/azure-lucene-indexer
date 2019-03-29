using Lucene.Net.Store;
using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

         PerFieldAnalyzerWrapper analyzerWrapper;
        
        private QueryParser parser;
        private const int MAX_HITS = 1000;
 
        public LuceneIndexer(string indexPath)
        {
            Analyzer whitespaceAnalyzer = new Lucene.Net.Analysis.WhitespaceAnalyzer();
            Analyzer nGramAnalyzer = new NGramAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            analyzerWrapper = new PerFieldAnalyzerWrapper(new Lucene.Net.Analysis.WhitespaceAnalyzer());

            analyzerWrapper.AddAnalyzer("Name", whitespaceAnalyzer);
            analyzerWrapper.AddAnalyzer("Mobile", nGramAnalyzer);

            luceneIndexDirectory = FSDirectory.Open(indexPath);
            indexWriter = new IndexWriter(luceneIndexDirectory, analyzerWrapper, IndexWriter.MaxFieldLength.UNLIMITED);

        }

        /// <summary>
        /// Add an Index Entry
        /// </summary>
        /// <param name="entry">The Entry to put</param>
        /// <param name="mobile">The Mobile Phone Number</param>
        public void AddIndexEntry(NameValueCollection entry)
        {
            Document doc = CreateDocument(entry["id"], entry["name"], entry["mobile"]);

            indexWriter.AddDocument(doc);
            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);
            indexWriter.Commit();

        }
 
        /// <summary>
        /// Add an Index Entries
        /// </summary>
        /// <param name="indexEntries">Index Entries</param>

        public void AddIndexEntries(IndexEntry[] indexEntries)
        {
            foreach (var indexEntry in indexEntries)
            {
                Document doc = CreateDocument(indexEntry.Id, indexEntry.Name, indexEntry.Mobile);
               
                indexWriter.AddDocument(doc);
            
            }

            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);
            indexWriter.Commit();

        }

       /// <summary>
        /// Delete Index Entries
        /// </summary>
        /// <param name="id">Unique Identifier</param>
        public void Delete(string id)
        {

            indexWriter.DeleteDocuments(new Term("Id", id));
            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);
            indexWriter.Commit();

        }

        /// <summary>
        /// Get an index Entry given an Identifier
        /// </summary>
        /// <param name="id">Unique Identifier</param>
        public IndexEntry Get(string id)
        {
            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);
            var query = new TermQuery(new Term("Id", id));

            var hits = searcher.Search(query, MAX_HITS).ScoreDocs;
            var results = hits.Select(hit => MapDocument(hit, searcher.Doc(hit.Doc))).ToList();

            return results.Count == 0 ? null : results.ElementAt(0);

        }

        /// <summary>
        /// Search the index entries given a term
        /// </summary>
        /// <param name="field">Search Field</param>
        /// <param name="term">Search Term</param>
        public IndexEntry[] Find(string field, string value)
        {
            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);

            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, field, analyzerWrapper);
            Query query = parser.Parse(value.Trim());

            var hits = searcher.Search(query, MAX_HITS).ScoreDocs;
            var results = hits.Select(hit => MapDocument(hit, searcher.Doc(hit.Doc))).ToArray();
     
            return results;

        }

        /// <summary>
        /// Search the index entries given a term
        /// </summary>
        /// <param name="term">Search Term</param>
        public IndexEntry[] Search(string term)
        {

            IndexSearcher searcher = new IndexSearcher(luceneIndexDirectory);
            MultiFieldQueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, 
                                                                     new string[] {"Name", "Mobile"},
                                                                     analyzerWrapper);           
            Query query = parser.Parse(term);

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
                Mobile = document.Get("Mobile"),
                Score = hit.Score

            };

        }

        public Document CreateDocument(String id, String name, String mobile = null)
        {
            Document doc = new Document();
            doc.Add(new Field("Id", id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", name, Field.Store.YES, Field.Index.ANALYZED));

            if (mobile != null) {
                doc.Add(new Field("Mobile", mobile, Field.Store.YES, Field.Index.ANALYZED));
            }

            return doc;

        }
        
    }

}