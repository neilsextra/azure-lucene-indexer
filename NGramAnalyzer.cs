using Lucene.Net.Analysis;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LuceneSearch.AzureLucene
{
    public class NGramAnalyzer : Analyzer
    {
        private readonly Version _version;
        private readonly int _minGram;
        private readonly int _maxGram;

        public NGramAnalyzer(Version version, int minGram = 2, int maxGram = 8)
        {
            _version = version;
            _minGram = minGram;
            _maxGram = maxGram;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            // Splits words at punctuation characters, removing punctuation.
            // Splits words at hyphens, unless there's a number in the token...
            // Recognizes email addresses and internet hostnames as one token.
            var tokenizer = new StandardTokenizer(_version, reader);

            TokenStream filter = new StandardFilter(tokenizer);

            // Normalizes token text to lower case.
            filter = new LowerCaseFilter(filter);

            // Removes stop words from a token stream.
            filter = new StopFilter(true, filter, StopAnalyzer.ENGLISH_STOP_WORDS_SET);

            return new NGramTokenFilter(filter, _minGram, _maxGram);

        }

    }

}
