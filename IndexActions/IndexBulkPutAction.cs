using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;

namespace azure_lucene_indexer
{
    /// <summary>
    /// An Entry in teh Lucene Index
    /// </summary>
    
    [DataContract]
    public class IndexBulkPutAction : IndexAction
    {

        /// <summary>
        /// Number id Members Added
        /// 
        /// </summary>
        [Required]
        [DataMember(Name = "Count")]
        public int Count { get; set; }

    }

}