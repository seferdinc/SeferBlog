using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyBlogSefer.Models
{
    public class Post
    {
        [Key]
        public int id_post { get; set; }
        public string title_post { get; set; }
        public string author { get; set; }
        public string content_post { get; set; }
        public string preview { get; set; }
        public DateTime date_post { get; set; }
        public virtual ICollection<Img> Imgs { get; set; }
        public virtual ICollection<PathofImg> PathofImgs { get; set; }
    }

    public class Img
    {
        [Key]
        public int id_Img { get; set; }
        [StringLength(255)]

        public string name_Img { get; set; }
        [StringLength(100)]
        public string img_contentType { get; set; }
        public byte[] content { get; set; }
        public ImgType Img_Type { get; set; }
        public int id_post { get; set; }
        public virtual Post post { get; set; }
    }

    public class PathofImg
    {
        [Key]
        public int id_PathofImg { get; set; }
        [StringLength(255)]
        public string name_Img { get; set; }

        public ImgType Img_Type { get; set; }
        public int id_post { get; set; }
        public virtual Post post { get; set; }
    }
    public enum ImgType
    {
        Avatar = 1, Photo
    }

}