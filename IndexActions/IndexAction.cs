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
    public class IndexAction
    {
        [Required]
        [DataMember(Name = "Operation")]
        public string Operation { get; set; }
        
        [Required]
        [DataMember(Name = "Status")]
        public int Status { get; set; }

        [Required]
        [DataMember(Name = "Message")]
        public string Message { get; set; }

    }

}