using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Node
    {
        public Node()
        {
            subNodes = new HashSet<Node>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }

        public int? ParentId { get; set; }

        public virtual Node ParentNode { get; set; }

        public virtual ICollection<Node> subNodes { get; set; }
    }

    public class NodeDTO
    {
        public NodeDTO()
        {
            id = -1;
            ParentId = -1;
        }
        public int id { get; set; }

        [Required(ErrorMessage ="Name is Required")]
        public string name { get; set; }

        public int? ParentId { get; set; }

    }
}
