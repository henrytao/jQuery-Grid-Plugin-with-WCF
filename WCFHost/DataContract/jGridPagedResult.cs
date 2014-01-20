using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EZTier.LogServiceHost
{
    [DataContract]
    public abstract class BindableObjects 
    {
        // dummy abstract class 
    }

    [DataContract]
    public class jGridPagedResult<T> where T : BindableObjects
    {
        [DataMember]
        public List<T> rows
        {get; set;}

        [DataMember]
        public int records
        { get; set; }
        [DataMember]
        public int total
        { get; set; }
        [DataMember]
        public int page
        { get; set; }   
    }

    /*
    [DataContract]
    public class jQueryGridLogDetailResponse
    {
        [DataMember]
        public List<LogDetailEntry> rows
        { get; set; }
        [DataMember]
        public int records
        { get; set; }
        [DataMember]
        public int total
        { get; set; }
        [DataMember]
        public int page
        { get; set; }
    }

    [DataContract]
    public class jQueryGridLogHeaderResponse
    {
        [DataMember]
        public List<LogHeaderEntry> rows
        { get; set; }
        [DataMember]
        public int records
        { get; set; }
        [DataMember]
        public int total
        { get; set; }
        [DataMember]
        public int page
        { get; set; }
    }

    [DataContract]
    public class jQueryGridLogFileCountsResponse
    {
        [DataMember]
        public List<LogFileCounts> rows
        { get; set; }
        [DataMember]
        public int records
        { get; set; }
        [DataMember]
        public int total
        { get; set; }
        [DataMember]
        public int page
        { get; set; }
    }
    */

}
