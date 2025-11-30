using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBook.Dto.CommentDtos
{
    public class ResultCommentDto
    {
        public int CommentID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public int BlogID { get; set; }

        public string NameInitials

        {

            get

            {

                if (string.IsNullOrWhiteSpace(Name))

                    return "";



                var parts = Name.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                return string.Join("", parts.Select(x => x[0])).Substring(0, Math.Min(2, parts.Count())).ToUpper();

            }

        }

    }

}
