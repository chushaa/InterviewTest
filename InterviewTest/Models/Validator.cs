using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace InterviewTest.Models
{
    public class Validator : IEnumerable<Validator>
    {
        Validator[] validList = null;

        [Range(0,1)]
        public int ID { get; set; }
        
        [StringLength(2, MinimumLength = 0)]
        public string state { get; set; }

        [StringLength(100, MinimumLength = 0)]
        public string textBox { get; set; }

        public string message { get; set; }

        public IEnumerator<Validator> GetEnumerator()
        {
            foreach (Validator i in validList)
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