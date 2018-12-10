using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace InterviewTest.Models
{
    public class XMLResultSet
    {
        public string returnresult { get; set; }
    }
    public class FinalResult : IEnumerable<FinalResult>
    {
        FinalResult[] resultList = null;

        public string resultName { get; set; }

        public string resultAirportCode { get; set; }

        public string resultAddress { get; set; }

        public IEnumerator<FinalResult> GetEnumerator()
        {
            foreach (FinalResult i in resultList)
            {
                if (i == null)
                {
                    break;
                }
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}