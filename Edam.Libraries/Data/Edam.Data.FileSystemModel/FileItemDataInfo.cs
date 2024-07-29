using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Data.FileSystemModel;

[Table("FileItemData")]
public class FileItemDataInfo
{
   public const string PARTITION_DEFAULT = "default";

   [Key]
   public Guid Id { get; set; } = Guid.NewGuid();

   [ForeignKey(nameof(FileItem))]
   public Guid FileItemId { get; set; }

   [Required]
   public FileItemInfo FileItem { get; set; }

   public long FileId { get; set; }

   [MaxLength(80)]
   public string PartitionId { get; set; } = PARTITION_DEFAULT;

   [MaxLength(128)]
   public string Name { get; set; }

   [ForeignKey(nameof(ContentType))]
   public string ContentTypeId { get; set; }

   public virtual ContentTypeInfo ContentType { get; set; } =
      new ContentTypeInfo()
      {
         TypeId = ContainerInfo.TEXT_CONTENT_TYPE,
         Description = ContainerInfo.TEXT_CONTENT_TYPE.Replace("/", " ")
      };

   public string Data { get; set; }

   public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;
   public DateTimeOffset RecordStatusDate { get; set; } = DateTimeOffset.Now;

   [MaxLength(20)]
   public string RecordStatusCode { get; set; } = "Active";
}
