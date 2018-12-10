using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterviewTest.Models
{
    public class Banner : IEnumerable<Banner>
    {
        Banner[] bannerItems = null;
        public int BannerId { get; set; }
        public string BannerName { get; set; }
        public string BannerLink { get; set; }
        public long ImpressionCount { get; set; }
        public long ClickCount { get; set; }

        public IEnumerator<Banner> GetEnumerator()
        {
            foreach(Banner i in bannerItems)
            {
                if(i == null)
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