using System.Runtime.Serialization;

namespace EZTier.LogServiceHost
{
    [DataContract]
    public class CobDate
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class FileName
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class LogHeaderEntry : BindableObjects
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string CobDate { get; set; }
        [DataMember]
        public string ProcessIds { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string ElapsedTime { get; set; }
        [DataMember]
        public string StartTime { get; set; }
        [DataMember]
        public string EndTime { get; set; }
    }

    [DataContract]
    public class LogFileCounts : BindableObjects
    {
        /*
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string CobDate { get; set; }
        */
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string ProcessId { get; set; }
        [DataMember]
        public string RecordCountSource { get; set; }
        [DataMember]
        public string Count { get; set; }
    }

    [DataContract]
    public class LogDetailEntry : BindableObjects
    {
        [DataMember]
        public string Uid { get; set; }
        [DataMember]
        /*
        public string Company { get; set; }
        [DataMember]
        public string CobDate { get; set; }
        [DataMember]
        */
        public string ProcessId { get; set; }
        [DataMember]
        /*
        public string FileName { get; set; }
        [DataMember]
        */
        public string DateAndTime { get; set; }
        [DataMember]
        public string EntryType { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string RecordCountSource { get; set; }
        [DataMember]
        public string Count { get; set; }
    }

}
