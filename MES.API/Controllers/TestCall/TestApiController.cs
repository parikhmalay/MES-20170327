using MES.API.Attributes;
using NPE.Web.Common.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MES.API.Controllers.TestCall
{
    [AdminPrefix("TestApi")]
    public class TestApiController : SecuredApiControllerBase
    {
        [HttpPostRoute("Search")]
        public IHttpActionResult Search(object objSearchData)
        {
            
            return Ok(objSearchData);
        }

        [HttpGetRoute("GetInfo")]
        public IHttpActionResult GetInfo()
        {
            return Ok("Success");
        }
    }
}