using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Linq;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web;

namespace EZTier.LogServiceHost
{

    [ServiceContract(Namespace="http://mvnet.unfcu.com/2009")]
    public interface ILoggerService
    {
        /*
        [WebInvoke(
                 Method = "POST",
                 RequestFormat = WebMessageFormat.Json,
                 ResponseFormat = WebMessageFormat.Json,
                 BodyStyle = WebMessageBodyStyle.Bare,
                 UriTemplate = "?filename={filename}"
        )] 
        [WebGet(UriTemplate = "?filename={filename}&nd={nd}&_search={search}&rows={rows}&page={page}&sidx={sidx}&sord={sord}", 
            ResponseFormat = WebMessageFormat.Json
            )]
         */
        //GET: /LoggerService/GetDetail?filename=%23&nd=1246883438361&_search=false&rows=10&page=1&sidx=Uid&sord=asc

        [OperationContract]
        [WebGet(
            UriTemplate = "GetCobDates",
            ResponseFormat = WebMessageFormat.Json
            )]
        List<CobDate> GetCobDates();

        [OperationContract]
        [WebGet(
            UriTemplate = "GetFileNames",
            ResponseFormat = WebMessageFormat.Json
            )]
        List<FileName> GetFileNames();

        [OperationContract]
        [WebGet(
            UriTemplate = "GetHeader?company={company}&cobdate={cobdate}&nd={nd}&_search={search}&rows={rows}&page={page}&sidx={sidx}&sord={sord}",
            ResponseFormat = WebMessageFormat.Json
            )]
        jGridPagedResult<LogHeaderEntry> GetHeader(
            string company,
            int cobdate,
            string nd,
            string search,
            int rows,
            int page,
            string sidx,
            string sord
            );

        [OperationContract]
        [WebGet(
            UriTemplate = "GetCounts?company={company}&cobdate={cobdate}&filename={filename}&nd={nd}&_search={search}&rows={rows}&page={page}&sidx={sidx}&sord={sord}",
            ResponseFormat = WebMessageFormat.Json
            )]
        jGridPagedResult<LogFileCounts> GetCounts(
            string company,
            int cobdate,
            string filename,
            string nd,
            string search,
            int rows,
            int page,
            string sidx,
            string sord
            );        

        [OperationContract]
        [WebGet(
            UriTemplate = "GetDetail?company={company}&cobdate={cobdate}&filename={filename}&nd={nd}&_search={search}&rows={rows}&page={page}&sidx={sidx}&sord={sord}",
            ResponseFormat = WebMessageFormat.Json
            )]
        jGridPagedResult<LogDetailEntry> GetDetail(
            string company,
            int cobdate,
            string filename,
            string nd,
            string search,
            int rows,
            int page,
            string sidx,
            string sord
            );        
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class LoggerService : ILoggerService
    {
        string company = "";
        int cobdate;
        string filename = "";
        int page, limit;
        string sidx = "";
        string sord = "";

        public List<CobDate> GetCobDates()
        {
            List<CobDate> cob_date_list = new List<CobDate>();
            LoggerDataContext context = new LoggerDataContext(Properties.Settings.Default.DWH2008ConnectionString);
            var cobdates = context.LOGGERs.Where(c => c.COB_DATE > 0).GroupBy(g => g.COB_DATE).Select(g => g.Key).OrderByDescending(g => g);

            foreach (var date in cobdates)
                cob_date_list.Add(new CobDate { 
                    text = DateTime.ParseExact(date.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString(),
                    value = date });

            return cob_date_list;
        }

        public List<FileName> GetFileNames()
        {
            List<FileName> file_name_list = new List<FileName>();
            LoggerDataContext context = new LoggerDataContext(Properties.Settings.Default.DWH2008ConnectionString);
            var filenames = context.LOGGERs.Where(c => c.FILE_NAME != null).GroupBy(g => g.FILE_NAME).OrderBy(g => g.Key).Select(g => g.Key);

            foreach (var name in filenames) {
                file_name_list.Add(new FileName { text = name, value = name
                });
            }

            return file_name_list;
        }
        
        public jGridPagedResult<LogHeaderEntry> GetHeader(
            string company,
            int cobdate,
            string nd,
            string search,
            int rows,
            int page,
            string sidx,
            string sord
            )
        {
            this.company = company;
            this.cobdate = cobdate;
            this.page = page; // requested page
            this.limit = rows; // get how many rows we want to have into the grid
            this.sidx = sidx; // get index row - ie, user click to sort
            bool b_sord = sord.Equals("asc") ? true : false; // get the direction

            Console.WriteLine("Pages requested: http://localhost:8168/LoggerService/GetHeader?cobdate={0}&nd={1}&_search={2}&rows={3}&page={4}&sidx={5}&sord={6}", cobdate.ToString(), nd, search, rows.ToString(), page.ToString(), sidx, sord);

            return PrepareHeaderResponse();
        }

        public jGridPagedResult<LogDetailEntry> GetDetail(
            string company,
            int cobdate,
            string filename,
            string nd,
            string search,
            int rows,
            int page,
            string sidx,
            string sord
            )
        {
            this.company = company; //GB0010003
            this.cobdate = cobdate; //20090106
            this.filename = filename;
            this.page = page; // requested page
            this.limit = rows; // get how many rows we want to have into the grid
            this.sidx = sidx; // get index row - ie, user click to sort
            bool b_sord = sord.Equals("asc") ? true : false; // get the direction

            return PrepareDetailResponse();        
        }

        public jGridPagedResult<LogFileCounts> GetCounts(
            string company,
            int cobdate,
            string filename,
            string nd,
            string search,
            int rows,
            int page,
            string sidx,
            string sord
            )
        {
            this.company = company;
            this.cobdate = cobdate;
            this.filename = filename;
            this.page = page; // requested page
            this.limit = rows; // get how many rows we want to have into the grid
            this.sidx = sidx; // get index row - ie, user click to sort
            bool b_sord = sord.Equals("asc") ? true : false; // get the direction

            return PrepareFileCountsResponse();
        }

        jGridPagedResult<LogHeaderEntry> PrepareHeaderResponse()
        {
            List<LogHeaderEntry> entries = new List<LogHeaderEntry>();
            LoggerDataContext context = new LoggerDataContext(Properties.Settings.Default.DWH2008ConnectionString);
            int total_pages = 0;
            int start = 0;
            int count = 0;

            var max_cobdates_by_company = context.LOGGERs.Where(c => c.FILE_NAME != null)
                .GroupBy(g => new { g.COMPANY })
                .Select(g=> new {g.Key, MaxCobDate = g.Max(c=> c.COB_DATE)});

            var first_company = max_cobdates_by_company.FirstOrDefault();

            var headers = context.LOGGERs.Where(c => c.FILE_NAME != null)
                .Where( c=> c.COB_DATE == (cobdate == -1 ? first_company.MaxCobDate : cobdate)
                    && c.COMPANY == (company == "default" ? first_company.Key.COMPANY : company) )
                .GroupBy(g => new { g.COMPANY, g.COB_DATE, g.FILE_NAME })
                .Select(g => new {
                    g.Key,
                    StartTime = g.Min(c => c.DATE_AND_TIME),
                    EndTime = g.Max(c => c.DATE_AND_TIME),
                    ListOfProcessIds = g.AsEnumerable().Select(c => new { c.PROCESS_ID }).Distinct()
                })
                .Select(g=>new {
                    g.Key,
                    ElapsedTime = g.EndTime.GetValueOrDefault().Subtract(g.StartTime.GetValueOrDefault()),
                    StartTime = g.StartTime.Value,
                    EndTime = g.EndTime.Value,
                    g.ListOfProcessIds
                });

            /*
            var combine = max_cobdates_by_company.Join(headers1, 
                m => new { COMPANY = m.Key.COMPANY, COB_DATE = m.MaxCobDate }, 
                h => new { COMPANY = h.Key.COMPANY, COB_DATE = h.Key.COB_DATE }, (m, h) => h);             

            var headers =
                from g in
                    (
                    from c in context.LOGGERs
                    where c.FILE_NAME != null 
                    group c by new { c.COMPANY, c.COB_DATE, c.FILE_NAME }
                        into g
                        select new
                        {
                            g.Key,
                            StartTime = g.Min(c => c.DATE_AND_TIME),
                            EndTime = g.Max(c => c.DATE_AND_TIME),
                            ListOfProcessIds = g.AsEnumerable().Select(c => new { c.PROCESS_ID }).Distinct()
                        }
                    )
                select new
                {
                    g.Key,
                    ElapsedTime = g.EndTime.GetValueOrDefault().Subtract(g.StartTime.GetValueOrDefault()),
                    StartTime = g.StartTime.Value,
                    EndTime = g.EndTime.Value,
                    g.ListOfProcessIds
                };
            */

            count = headers.Count();

            if (count > 0)
                total_pages = (int)Math.Ceiling((double)count / limit);
            else
                total_pages = 0;
            
            if (page > total_pages) page = total_pages;

            start = (limit * page) - limit;

            if (page > 0)
                headers = headers.Skip((page - 1) * limit).Take(limit);

            StringBuilder process_string_builder = new StringBuilder();

            foreach (var header in headers)
            {
                if (process_string_builder.Length > 0)
                    process_string_builder.Remove(0, process_string_builder.Length);
                
                foreach (var processid in header.ListOfProcessIds)
                    process_string_builder.Append(processid.PROCESS_ID.GetValueOrDefault().ToString() + ",");
                
                string processes = process_string_builder.ToString().Substring(0, process_string_builder.Length - 1);

                entries.Add(new LogHeaderEntry{
                    Id = string.Format("company={0}&cobdate={1}&filename={2}", header.Key.COMPANY, header.Key.COB_DATE.ToString(), header.Key.FILE_NAME),
                    Company = header.Key.COMPANY,
                    CobDate = header.Key.COB_DATE.ToString(),
                    ProcessIds = processes,
                    FileName = header.Key.FILE_NAME,
                    ElapsedTime = (Math.Round(header.ElapsedTime.TotalMilliseconds/1000.0,2)).ToString(),
                    StartTime = header.StartTime.ToString("MM/dd/yyyy hh:mm:ss"),
                    EndTime = header.EndTime.ToString("MM/dd/yyyy hh:mm:ss")
                }
                    );
            }

            /*
            jQueryGridLogHeaderResponse response = 
                new jQueryGridLogHeaderResponse{
                    page = page,
                    total = total_pages,
                    rows = entries.ToList(),
                    records = count
                };
            */

            jGridPagedResult<LogHeaderEntry> response =
                new jGridPagedResult<LogHeaderEntry>
                {
                    page = page,
                    total = total_pages,
                    rows = entries.ToList(),
                    records = count
                };

            return response;
        }

        jGridPagedResult<LogFileCounts> PrepareFileCountsResponse()
        {
            List<LogFileCounts> entries = new List<LogFileCounts>();
            LoggerDataContext context = new LoggerDataContext(Properties.Settings.Default.DWH2008ConnectionString);
            int total_pages = 0;
            int start = 0;
            int count = 0;
            
            var counts = context.LOGGERs.Where(c => c.RECORD_COUNT_SOURCE != "NONE" && c.COMPANY == company && c.COB_DATE == cobdate && c.FILE_NAME == filename).GroupBy(c => new { c.COMPANY, c.COB_DATE, c.PROCESS_ID, c.FILE_NAME, c.RECORD_COUNT_SOURCE }).Select(g => new { g.Key, COUNT = g.Sum(c => c.COUNT) });
            count = counts.Count();

            if (count > 0)
                total_pages = (int)Math.Ceiling((double)count / limit);
            
            if (page > total_pages) page = total_pages;

            start = (limit * page) - limit;

            if (page > 0)
                counts = counts.Skip((page - 1) * limit).Take(limit);

            foreach (var item in counts)
                entries.Add(new LogFileCounts { FileName = item.Key.FILE_NAME, ProcessId = item.Key.PROCESS_ID.GetValueOrDefault().ToString(), RecordCountSource = item.Key.RECORD_COUNT_SOURCE, Count = item.COUNT.GetValueOrDefault().ToString("n0") });

            if (entries.Count < 1)
                entries.Add(new LogFileCounts { FileName = filename, ProcessId = "-1", RecordCountSource = "NONE", Count = "0" });

            jGridPagedResult<LogFileCounts> response =
                new jGridPagedResult<LogFileCounts>
                {
                    page = page,
                    total = total_pages,
                    rows = entries.ToList(),
                    records = count
                };

            return response;
        }

        jGridPagedResult<LogDetailEntry> PrepareDetailResponse()
        {
            List<LogDetailEntry> entries = new List<LogDetailEntry>();
            LoggerDataContext context = new LoggerDataContext(Properties.Settings.Default.DWH2008ConnectionString);
            int total_pages = 0;
            int start = 0;
            int count = 0;

            //count = context.LOGGERs.Count(d => d.FILE_NAME == (filename.ToLower().Equals("all") ? d.FILE_NAME : filename));
            count = context.LOGGERs.Where(d => d.COMPANY == company && d.COB_DATE == cobdate && d.FILE_NAME == filename).Count(); 

            if (count > 0)
            {
                total_pages = (int)Math.Ceiling((double)count / limit);
            }
            else
            {
                return null;
                //total_pages = 0;
            }

            if (page > total_pages) page = total_pages;

            start = (limit * page) - limit; // do not put limit*(page - 1)

            /* Linq Paging
            var _pageNum = 3;
            var _pageSize = 20;
            query = query.Skip((_pageNum - 1) * _pageSize).Take(_pageSize); 
            */
            
            var details = from d in context.LOGGERs
                          .Where(d => d.COMPANY == company && d.COB_DATE == cobdate && d.FILE_NAME == filename)
                          //where d.FILE_NAME == (filename.ToLower().Equals("all") ? d.FILE_NAME : filename)
                          select d;
            
            if (page > 0)
                details = details.Skip((page - 1) * limit).Take(limit);

            foreach(var entry in details)
            {
                entries.Add(new LogDetailEntry
                {
                    Uid = entry.UID.ToString(),
                    //Company = entry.COMPANY,
                    //CobDate = entry.COB_DATE.ToString(),
                    DateAndTime = entry.DATE_AND_TIME.Value.ToString("MM/dd/yyyy hh:mm:ss"),
                    ProcessId = entry.PROCESS_ID.Value.ToString(),
                    //FileName = entry.FILE_NAME,
                    EntryType = entry.ENTRY_TYPE,
                    Message = entry.MESSAGE,
                    RecordCountSource = entry.RECORD_COUNT_SOURCE,
                    Count = entry.COUNT.Value.ToString()
                });
            }
            
            /*
            jQueryGridLogDetailResponse response = 
                new jQueryGridLogDetailResponse{
                    page = page,
                    total = total_pages,
                    rows = entries.ToList(),
                    records = count
                    };
            */

            jGridPagedResult<LogDetailEntry> response = 
                new jGridPagedResult<LogDetailEntry>{
                    page = page,
                    total = total_pages,
                    rows = entries.ToList(),
                    records = count
                };
            return response;

            /*
            foreach (var entry in details)
            {
                entries.Add(new LogEntry
                {
                    Uid = entry.UID.ToString(),
                    Company = entry.COMPANY,
                    CobDate = entry.COB_DATE.ToString(),
                    DateAndTime = entry.DATE_AND_TIME.Value.ToString(),
                    ProcessId = entry.PROCESS_ID.Value.ToString(),
                    FileName = entry.FILE_NAME,
                    EntryType = entry.ENTRY_TYPE,
                    Message = entry.MESSAGE,
                    RecordCountSource = entry.RECORD_COUNT_SOURCE,
                    Count = entry.COUNT.Value.ToString()
                });
            }
            */

        }

    }
}
