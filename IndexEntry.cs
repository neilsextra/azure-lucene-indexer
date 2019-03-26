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
    public class IndexEntry
    {
        /// <summary>
        /// Unique Identifier for the Entry
        /// 
        /// </summary>
        [Required]
        [DataMember(Name = "Id")]
        public string Id { get; set; }

        /// <summary>
        /// The Name of Person or Organisation
        /// </summary>
        [Required]
        [DataMember(Name = "Name")]

        public string Name { get; set; }

        [DataMember(Name = "Score")]
        public float Score { get; set; }
        
    }

}