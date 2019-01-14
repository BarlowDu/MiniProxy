using HttpClientProxy.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpClientProxy
{
    [HttpProxy(Prefix  = "http://tyapi.yiche.com/webapi/api")]
    public interface IClient
    {
        [ResquestMapping(Path = "/Common/GetBanner")]
        string GetBanner(int bigRegionNO,int source);

        [ResquestMapping(Path = "/OfflineActivity/GetActivityList")]
        string GetActivityList(int bigregionid, int cityid, int listdatatype, int pageindex, int pagesize);
    }
}
