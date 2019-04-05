using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;

namespace azure_lucene_indexer   
{ 

    public class MultiPartFile
    {
       public string Filename 
       { 
           get; 
           set; 
        }

        public int UserId
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }


        public byte[] FileContents
        {
            get;
            set;
        }

    }
}